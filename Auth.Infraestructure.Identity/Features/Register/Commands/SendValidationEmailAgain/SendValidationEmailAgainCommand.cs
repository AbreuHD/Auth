using Auth.Core.Application.DTOs.Generic;
using Auth.Infraestructure.Identity.Entities;
using Auth.Infraestructure.Identity.Extra;
using Auth.Infraestructure.Identity.Features.Email.Commands.SendEmail;
using Auth.Infraestructure.Identity.Features.Register.Commands.CreateAccount;
using MediatR;
using Microsoft.AspNetCore.Identity;


namespace Auth.Infraestructure.Identity.Features.Register.Commands.SendValidationEmailAgain
{
    public class SendValidationEmailAgainCommand : IRequest<GenericApiResponse<string>>
    {
        public required string Email { get; set; }
        public required string Origin { get; set; }
    }

    public class SendValidationEmailAgainCommandHandler : IRequestHandler<SendValidationEmailAgainCommand, GenericApiResponse<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IMediator Mediator { get; }

        public SendValidationEmailAgainCommandHandler(UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            _userManager = userManager;
            Mediator = mediator;
        }

        public async Task<GenericApiResponse<string>> Handle(SendValidationEmailAgainCommand request, CancellationToken cancellationToken)
        {
            var response = new GenericApiResponse<string>();

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                response.Success = false;
                response.Message = $"Email {request.Email} not exist";
                response.Statuscode = 404;
                return response;
            }

            if (user.EmailConfirmed)
            {
                response.Success = false;
                response.Message = $"Email {request.Email} is already confirmed";
                response.Statuscode = 200;
                return response;
            }

            var verificationUrl = await ExtraMethods.SendVerificationEMailUrl(user, request.Origin, _userManager);
            await Mediator.Send(new SendEmailCommand
            {
                To = user.Email,
                Body = $"Please confirm your account visiting this URL {verificationUrl}",
                Subject = "Confirm registration"
            }, cancellationToken);

            response.Success = true;
            response.Message = $"Email to {request.Email} sent";
            response.Statuscode = 200;
            return response;
        }
    }
}
