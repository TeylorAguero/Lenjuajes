using ApiWebSecurityCloud.Models;

namespace ApiWebSecurityCloud.Services
{
    public interface IAuthorizationServices
    {
        AuthorizationResponse DevolverToken(LoginRequest authorization);
    }
}
