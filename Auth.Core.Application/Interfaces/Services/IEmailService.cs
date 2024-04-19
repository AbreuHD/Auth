using Auth.Core.Application.DTOs.Email;

namespace Auth.Core.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);
    }
}
