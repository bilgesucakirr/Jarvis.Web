using Jarvis.Web.Models;

namespace Jarvis.Web.Services;

public interface IAuthService
{
    Task<AuthResponseModel> Login(LoginModel loginModel);
    Task<AuthResponseModel> Register(RegisterModel registerModel);
    Task Logout();
}