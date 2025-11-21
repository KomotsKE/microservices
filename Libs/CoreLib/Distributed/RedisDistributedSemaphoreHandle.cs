using Corelib.Distributed.interfaces;
using StackExchange.Redis;

namespace Corelib.Distributed.RedisDistrubutedSemaphoreHandle;

/// <summary>
/// Дескриптор распределённой блокировки.
/// Хранит состояние lock'a и следит за его жизнью.
/// </summary>
public sealed class RedisDistributedSemaphoreHandle :
    IDistributedSynchronizationHandle, IAsyncDisposable
{
    private readonly IDatabase _db;
    private readonly IConnectionMultiplexer _mux;
    private readonly string _key;
    private readonly string _lockId;
    private readonly TimeSpan _expiry;

    private readonly CancellationTokenSource _lostCts = new();
    private readonly CancellationTokenSource _internalCts = new();

    private Task? _heartbeatTask;
    private ISubscriber? _subscriber;
    private bool _disposed;

    public CancellationToken HandleLostToken => _lostCts.Token;

    private static readonly string ReleaseScript = @"
        local key = KEYS[1]
        local lockId = ARGV[1]
        local ttl = tonumber(ARGV[2])

        redis.call('zrem', key, lockId)
        if redis.call('exists', key) == 1 then
            redis.call('expire', key, ttl)
        end
        return 1
    ";

    public RedisDistributedSemaphoreHandle(
        IConnectionMultiplexer mux,
        IDatabase db,
        string key,
        string lockId,
        TimeSpan expiry)
    {
        _mux = mux;
        _db = db;
        _key = key;
        _lockId = lockId;
        _expiry = expiry;
        _mux.ConnectionFailed += OnConnectionFailed;
    }

    public void Start()
    {
        _heartbeatTask = Task.Run(HeartbeatLoop);
        Subscribe();
    }

    private void Subscribe()
    {
        try
        {
            _subscriber = _mux.GetSubscriber();
            var dbIndex = _db.Database;
            var channel = RedisChannel.Literal($"__keyspace@{dbIndex}__:{_key}");

            _subscriber.Subscribe(channel, (_, msg) =>
            {
                var evt = msg.ToString();
                if (evt == "expired" || evt == "del")
                    MarkLost();
            });
        }
        catch { }
    }

    private async Task HeartbeatLoop()
    {
        var checkInterval = TimeSpan.FromSeconds(_expiry.TotalSeconds / 3);

        try
        {
            while (!_internalCts.IsCancellationRequested)
            {
                await Task.Delay(checkInterval, _internalCts.Token);

                try
                {
                    var score = await _db.SortedSetScoreAsync(_key, _lockId);
                    if (score == null)
                    {
                        MarkLost();
                        return;
                    }
                    await _db.KeyExpireAsync(_key, _expiry);
                }
                catch
                {
                    MarkLost();
                    return;
                }
            }
        }
        catch{}
    }

    private void OnConnectionFailed(object? sender, ConnectionFailedEventArgs e) => MarkLost();

    private void MarkLost() { try { _lostCts.Cancel(); } catch {} }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        _internalCts.Cancel();

        try { if (_heartbeatTask != null) await _heartbeatTask; } catch {}

        try
        {
            await _db.ScriptEvaluateAsync(
                ReleaseScript,
                [_key],
                [_lockId, (int)_expiry.TotalSeconds]);
        }
        catch { }

        try { _subscriber?.UnsubscribeAll(); } catch {}

        _mux.ConnectionFailed -= OnConnectionFailed;

        _lostCts.Dispose();
        _internalCts.Dispose();
    }

    public void Dispose() => DisposeAsync().AsTask().GetAwaiter().GetResult();
}