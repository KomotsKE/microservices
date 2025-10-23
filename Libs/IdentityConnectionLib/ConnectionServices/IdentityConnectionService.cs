using CoreLib.HttpServiceV2.Services.Interfaces;
using IdentityConnectionLib.ConnectionServices.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace IdentityConnectionLib.ConnectionService;

public class IdentityConnectionService : IIdentityConnectionService
{
    private readonly IHttpRequestService _httpClientFactory;

    public IdentityConnectionService(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        if (configuration.GetSection("dsgf").Value == "http")
        {
            _httpClientFactory = serviceProvider.GetRequiredService<IHttpRequestService>();
        }
        else
        {
            // RPC по rabbit
        }
    }

    public Task<bool> CheckUserExistsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}