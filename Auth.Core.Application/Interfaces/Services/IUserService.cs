using Auth.Core.Application.DTOs.Account;
using Auth.Core.Application.DTOs.Generic;

namespace Auth.Core.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<GenericApiResponse<AuthenticationResponse>> Login(AuthenticationRequest login);
        Task SignOut();
        Task<GenericApiResponse<RegisterResponse>> Register(RegisterRequest viewModel, string origin);
        Task UpdateUser(string Id, RegisterRequest viewModel);
        Task<string> EmailConfirm(string userId, string token);
        Task<GenericApiResponse<string>> ForgotPassword(ForgotPasswordRequest request, string origin);
        Task<GenericApiResponse<string>> ResetPassword(ResetPasswordRequest request);
    }
}
