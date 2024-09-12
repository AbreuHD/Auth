using Auth.Core.Application.DTOs.Generic;
using Auth.Core.Application.Enums;
using Auth.Infraestructure.Identity.Entities;
using Auth.Infraestructure.Identity.Features.Email.Commands.SendEmail;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Auth.Infraestructure.Identity.Extra;

namespace Auth.Infraestructure.Identity.Features.Register.Commands.CreateAccount
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateAccountCommand : IRequest<GenericApiResponse<string>>
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string ImageProfile { get; set; } = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_960_720.png";
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required string Phone { get; set; }
        public required string ORIGIN { get; set; }
    }
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, GenericApiResponse<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private IMediator Mediator { get; }

        public CreateAccountCommandHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMediator mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            Mediator = mediator;
        }

        public async Task<GenericApiResponse<string>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var response = new GenericApiResponse<string>();

            var UserNameExist = await _userManager.FindByNameAsync(request.UserName);
            
            if (UserNameExist != null)
            {
                response.Success = false;
                response.Message = $"Username {request.UserName} is already taken";
                response.Statuscode = 406;
                return response;
            }

            var EmailExist = await _userManager.FindByEmailAsync(request.Email);
            if (EmailExist != null)
            {
                response.Success = false;
                response.Message = $"Email {request.Email} is already registered";
                response.Statuscode = 406;
                return response;
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                Name = request.Name,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.Phone,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                response.Success = false;
                response.Message = "A error occurred trying to register the user.";
                response.Statuscode = 400;
                return response;

            }

            var regiteredUser = await _userManager.FindByEmailAsync(user.Email);
            response.Payload = regiteredUser.Id;
            await _userManager.AddToRoleAsync(user, Roles.User.ToString());

            var verificationUrl = await ExtraMethods.SendVerificationEMailUrl(user, request.ORIGIN, _userManager);

            await Mediator.Send(new SendEmailCommand 
            {
                To = user.Email,
                Body = $"Please confirm your account visiting this URL {verificationUrl}",
                Subject = "Confirm registration"
            }, cancellationToken);

            return response;
        }
    }
}
