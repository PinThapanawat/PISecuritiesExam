using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Models;
using WebAPI.Models.DTOs;
using WebAPI.Services.Interferes;

namespace WebAPI.Services;

public class IdentityService : IIdentityService
{
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtAppSettings _jWtAppSettings;

        public IdentityService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, 
            IOptions<JwtAppSettings> jWtAppSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jWtAppSettings=jWtAppSettings.Value;
        }

        public async Task<LoginResponseViewModel> LoginAsync(LoginViewModel loginViewModel)
        {
            var user = await _userManager.FindByNameAsync(loginViewModel.Username!);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginViewModel.Password!))
                return new LoginResponseViewModel { };
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

            var token = GetToken(authClaims);

            return new LoginResponseViewModel
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };
        }

        public async Task<ResponseViewModel> RegisterAsync(RegisterViewModel registerViewModel)
        {
            var userExists = await _userManager.FindByNameAsync(registerViewModel.Username!);
            if (userExists != null)
                return new ResponseViewModel { Status = "Error", Message = "User already exists!" };

            ApplicationUser user = new()
            {
                Email = registerViewModel.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerViewModel.Username
            };
            var result = await _userManager.CreateAsync(user, registerViewModel.Password!);
            return !result.Succeeded 
                ? new ResponseViewModel { Status = "Error", Message = "User creation failed! Please check user details and try again." } 
                : new ResponseViewModel { Status = "Success", Message = "User created successfully!" };
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWtAppSettings.Secret!));

            var token = new JwtSecurityToken(
                issuer: _jWtAppSettings.ValidIssuer,
                audience: _jWtAppSettings.ValidAudience,
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    
}
