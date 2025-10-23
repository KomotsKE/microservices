
using CoreLib.HttpLogic.Services;
using CoreLib.HttpServiceV2.Services.Interfaces;
using IdentityConnectionLib.ConnectionServices.DtoMidels.CheckUserExists;
using IdentityConnectionLib.ConnectionServices.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityConnectionLib.ConnectionService;

public class IdentityConnectionService : IIdentityConnectionService
{
    private readonly IHttpRequestService _httpClientFactory;

    public IdentityConnectionService(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        if (configuration.GetSection("IdentityService:Transport").Value == "http")
        {
            _httpClientFactory = serviceProvider.GetRequiredService<IHttpRequestService>();
        }
        else
        {
            throw new NotImplementedException("RPC transport not implemented yet");
        }
    }

    public async Task<CheckUserExistIdentityServiceResponce> CheckUserExistsAsync(CheckUserExistIdentityServiceRequest request)
    {
        var connectionData = new HttpConnectionData
        {
            ClientName = "identity",
            Timeout = TimeSpan.FromSeconds(10)
        };

        var requestData = new HttpRequestData
        {
            Method = HttpMethod.Get,
            Uri = new Uri($"identityservice/api/user/{request.UserId}/exists", UriKind.Relative)
        };

        var response = await _httpClientFactory.SendRequestAsync<CheckUserExistIdentityServiceResponce>(
            requestData, connectionData);

        if (!response.IsSuccessStatusCode)
            return new CheckUserExistIdentityServiceResponce { IsExist = false };

        return response.Body;
    }
}