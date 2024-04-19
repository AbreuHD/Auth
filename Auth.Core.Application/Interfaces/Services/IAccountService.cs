
using Auth.Core.Application.DTOs.Account;
using Auth.Core.Application.DTOs.Generic;

namespace Auth.Core.Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task<GenericApiResponse<AuthenticationResponse>> Authentication(AuthenticationRequest request);
        Task<string> ConfirmEmail(string userId, string token);
        Task<GenericApiResponse<string>> ForgotPassword(ForgotPasswordRequest request, string origin);
        Task<GenericApiResponse<RegisterResponse>> Register(RegisterRequest request, string origin);
        Task<GenericApiResponse<string>> UpdateUser(string userId, RegisterRequest request);
        Task<GenericApiResponse<string>> ResetPassword(ResetPasswordRequest request);
        Task SignOut();
    }
}
