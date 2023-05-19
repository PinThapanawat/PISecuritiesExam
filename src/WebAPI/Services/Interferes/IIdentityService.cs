using WebAPI.Models.DTOs;

namespace WebAPI.Services.Interferes;

public interface IIdentityService
{
    Task<LoginResponseViewModel> LoginAsync(LoginViewModel loginViewModel);
    Task<ResponseViewModel> RegisterAsync(RegisterViewModel registerViewModel);
}