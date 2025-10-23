using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using CoreLib.HttpServiceV2.Services.Interfaces;
using CoreLib.HttpLogic.Services.Interfaces;
using CoreLib.TraceLogic.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;

namespace CoreLib.HttpLogic.Services;

public enum ContentType
{
    ///
    Unknown = 0,

    ///
    ApplicationJson = 1,

    ///
    XWwwFormUrlEncoded = 2,

    ///
    Binary = 3,

    ///
    ApplicationXml = 4,

    ///
    MultipartFormData = 5,

    /// 
    TextXml = 6,

    /// 
    TextPlain = 7,

    ///
    ApplicationJwt = 8
}

public record HttpRequestData
{
    /// <summary>
    /// Тип метода
    /// </summary>
    public required HttpMethod Method { get; set; }

    /// <summary>
    /// Адрес запроса
    /// </summary>\
    public required Uri Uri { set; get; }

    /// <summary>
    /// Тело метода
    /// </summary>
    public required object Body { get; set; }

    /// <summary>
    /// content-type, указываемый при запросе
    /// </summary>
    public ContentType ContentType { get; set; } = ContentType.ApplicationJson;

    /// <summary>
    /// Заголовки, передаваемые в запросе
    /// </summary>
    public IDictionary<string, string> HeaderDictionary { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Коллекция параметров запроса
    /// </summary>
    public ICollection<KeyValuePair<string, string>> QueryParameterList { get; set; } =
        new List<KeyValuePair<string, string>>();
}

public record BaseHttpResponse
{
    /// <summary>
    /// Статус ответа
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// Заголовки, передаваемые в ответе
    /// </summary>
    public required HttpResponseHeaders Headers { get; set; }

    /// <summary>
    /// Заголовки контента
    /// </summary>
    public required HttpContentHeaders ContentHeaders { get; init; }

    /// <summary>
    /// Является ли статус код успешным
    /// </summary>
    public bool IsSuccessStatusCode
    {
        get
        {
            var statusCode = (int)StatusCode;

            return statusCode >= 200 && statusCode <= 299;
        }
    }
}

public record HttpResponse<TResponse> : BaseHttpResponse
{
    /// <summary>
    /// Тело ответа
    /// </summary>
    public required TResponse Body { get; set; }
}

/// <inheritdoc />
internal class HttpRequestService : IHttpRequestService
{
    private readonly IHttpConnectionService _httpConnectionService;
    private readonly IEnumerable<ITraceWriter> _traceWriterList;

    ///
    public HttpRequestService(
        IHttpConnectionService httpConnectionService,
        IEnumerable<ITraceWriter> traceWriterList)
    {
        _httpConnectionService = httpConnectionService;
        _traceWriterList = traceWriterList;
    }

    /// <inheritdoc />
    public async Task<HttpResponse<TResponse>> SendRequestAsync<TResponse>(HttpRequestData requestData,
        HttpConnectionData connectionData)
    {
        var client = _httpConnectionService.CreateHttpClient(connectionData);

        var httpRequestMessage = new HttpRequestMessage(requestData.Method, BuildUriWithQueryParams(requestData.Uri, requestData.QueryParameterList));

        foreach (var traceWriter in _traceWriterList)
            httpRequestMessage.Headers.Add(traceWriter.Name, traceWriter.GetValue());

        foreach (var header in requestData.HeaderDictionary)
            httpRequestMessage.Headers.Add(header.Key, header.Value);

        if (requestData.Method != HttpMethod.Get && requestData.Body is not null)
            httpRequestMessage.Content = PrepairContent(requestData.Body, requestData.ContentType);

        var retryPolicy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 4,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(2 * attempt),
                onRetryAsync: (result, retryCount, _) =>
                {
                    Console.WriteLine($"Начало {retryCount} Попытки повтора");
                    return Task.CompletedTask;
                });

        var responceMessage = await retryPolicy
            .ExecuteAsync(() => _httpConnectionService.SendRequestAsync(httpRequestMessage, client, connectionData.CancellationToken));

        var contentHeaders = (responceMessage.Content ?? new StringContent(string.Empty)).Headers;

        var result = new HttpResponse<TResponse>
        {
            StatusCode = responceMessage.StatusCode,
            Headers = responceMessage.Headers,
            ContentHeaders = contentHeaders,
            Body = default!
        };

        if (responceMessage.Content != null)
        {
            var responseText = await responceMessage.Content.ReadAsStringAsync();

            if (typeof(TResponse) == typeof(string))
                result.Body = (TResponse)(object)responseText!;
            else if (!string.IsNullOrWhiteSpace(responseText))
                result.Body = JsonConvert.DeserializeObject<TResponse>(responseText)!;
        }
        
        return result;
    }

    private static Uri BuildUriWithQueryParams(Uri uri, ICollection<KeyValuePair<string, string>> queryParameterList)
    {
        if (queryParameterList.Count == 0 || queryParameterList is null) { return uri; }

        var queryParams = string.Join("&", queryParameterList.Select(p =>
            $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));

        var uriBuilder = new UriBuilder(uri);
        uriBuilder.Query = string.IsNullOrWhiteSpace(uriBuilder.Query)
            ? queryParams : uriBuilder.Query.TrimStart('?') + "&" + queryParams;

        return uriBuilder.Uri;
    }

    private static HttpContent PrepairContent(object body, ContentType contentType)
    {
        switch (contentType)
        {
            case ContentType.ApplicationJson:
            {
                if (body is string stringBody)
                {
                    body = JToken.Parse(stringBody);
                }

                var serializeSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };
                var serializedBody = JsonConvert.SerializeObject(body, serializeSettings);
                var content = new StringContent(serializedBody, Encoding.UTF8, MediaTypeNames.Application.Json);
                return content;
            }

            case ContentType.XWwwFormUrlEncoded:
            {
                if (body is not IEnumerable<KeyValuePair<string, string>> list)
                {
                    throw new Exception(
                        $"Body for content type {contentType} must be {typeof(IEnumerable<KeyValuePair<string, string>>).Name}");
                }

                return new FormUrlEncodedContent(list);
            }
            case ContentType.ApplicationXml:
            {
                if (body is not string s)
                {
                    throw new Exception($"Body for content type {contentType} must be XML string");
                }

                return new StringContent(s, Encoding.UTF8, MediaTypeNames.Application.Xml);
            }
            case ContentType.Binary:
            {
                if (body.GetType() != typeof(byte[]))
                {
                    throw new Exception($"Body for content type {contentType} must be {typeof(byte[]).Name}");
                }

                return new ByteArrayContent((byte[])body);
            }
            case ContentType.TextXml:
            {
                if (body is not string s)
                {
                    throw new Exception($"Body for content type {contentType} must be XML string");
                }

                return new StringContent(s, Encoding.UTF8, MediaTypeNames.Text.Xml);
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null);
        }
    }
}