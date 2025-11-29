using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Corelib.Distributed.interfaces;
using Corelib.Distributed.RedisDistributedSemaphoreHandle;
using StackExchange.Redis;

namespace Corelib.Distributed.RedisDistributedSemaphore;
/// <inheritdoc />
public sealed class RedisDistributedSemaphore : IDistributedSemaphore
{
    private readonly IConnectionMultiplexer _mux;
    private readonly IDatabase _db;
    private readonly string _key;
    private readonly int _maxCount;
    private readonly TimeSpan _expiry;

    private static readonly string AcquireScript = @"
        local key = KEYS[1]
        local lockId = ARGV[1]
        local maxCount = tonumber(ARGV[2])
        local now = tonumber(ARGV[3])
        local ttl = tonumber(ARGV[4])

        redis.call('zremrangebyscore', key, 0, now - ttl)

        local count = redis.call('zcard', key)
        if count < maxCount then
            redis.call('zadd', key, now, lockId)
            redis.call('expire', key, ttl)
            return 1
        else
            return 0
        end
    ";

    public string Name { get; }
    public int MaxCount => _maxCount;

    public RedisDistributedSemaphore(
        IConnectionMultiplexer mux,
        string name,
        int maxCount,
        TimeSpan? expiry = null)
    {
        _mux = mux;
        _db = mux.GetDatabase();
        _maxCount = maxCount;
        Name = name;
        _key = $"semaphore:{name}";
        _expiry = expiry ?? TimeSpan.FromSeconds(30);
    }

    public IDistributedSynchronizationHandle? TryAcquire(TimeSpan timeout = default, CancellationToken cancellationToken = default)
        => TryAcquireAsync(timeout, cancellationToken).GetAwaiter().GetResult();

    public IDistributedSynchronizationHandle Acquire(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var handle = TryAcquire(timeout ?? Timeout.InfiniteTimeSpan, cancellationToken);
        return handle ?? throw new TimeoutException($"Failed to acquire '{Name}'");
    }

    public async ValueTask<IDistributedSynchronizationHandle?> TryAcquireAsync(TimeSpan timeout = default, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var infinite = timeout == Timeout.InfiniteTimeSpan;

        while (!cancellationToken.IsCancellationRequested && (infinite || sw.Elapsed < timeout))
        {
            var lockId = $"{Environment.MachineName}:{Guid.NewGuid()}";
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var res = await _db.ScriptEvaluateAsync(
                AcquireScript,
                new RedisKey[] { _key },
                new RedisValue[] { lockId, _maxCount, now, (int)_expiry.TotalSeconds });

            int result = 0;
            try
            {
                if (!res.IsNull)
                    result = Convert.ToInt32((long)res);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RedisDistributedSemaphore: failed to parse script result: {ex}");
            }

            if (result == 1)
            {
                var handle = new 
                RedisDistributedSemaphoreHandle.RedisDistributedSemaphoreHandle(
                    _mux,
                    _db,
                    _key,
                    lockId,
                    _expiry);

                handle.Start();
                return handle;
            }
            await Task.Delay(100, cancellationToken);
        }
        return null;
    }

    public async ValueTask<IDistributedSynchronizationHandle> AcquireAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        var handle = await TryAcquireAsync(timeout ?? Timeout.InfiniteTimeSpan, cancellationToken);
        return handle ?? throw new TimeoutException($"Failed to acquire '{Name}'");
    }
}

