namespace WebAPI.Models.DTOs;

public class LoginResponseViewModel
{
    public string? Token { get; set; }
    public DateTime Expiration { get; set; }
}