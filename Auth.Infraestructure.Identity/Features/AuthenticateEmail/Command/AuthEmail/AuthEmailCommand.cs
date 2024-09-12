using Auth.Core.Application.DTOs.Generic;
using Auth.Infraestructure.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Infraestructure.Identity.Features.AuthenticateEmail.Command.AuthEmail
{
    public class AuthEmailCommand : IRequest<GenericApiResponse<string>>
    {
        public required string userid { get; set; }
        public required string token { get; set; }
    }

    public class AuthEmailCommandHandler : IRequestHandler<AuthEmailCommand, GenericApiResponse<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;


        public AuthEmailCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<GenericApiResponse<string>> Handle(AuthEmailCommand request, CancellationToken cancellationToken)
        {
            var response = new GenericApiResponse<string>();

            var user = await _userManager.FindByIdAsync(request.userid);
            if (user == null)
            {
                response.Message = $"Not account registered with this user";
                response.Statuscode = 404;
                response.Payload = "N/A";
                response.Success = false;
                return response;
            }

            request.token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.token));
            
            var result = await _userManager.ConfirmEmailAsync(user, request.token);
            
            if (!result.Succeeded)
            {
                response.Message = $"An error occurred while confirming {user.Email}";
                response.Statuscode = 400;
                response.Payload = result.Errors.FirstOrDefault().Description;
                response.Success = false;
                return response;
            }
            
            response.Success = true;
            response.Message = $"Account confirmed for {user.Email}. You can now use the App";
            response.Statuscode = 200;
            response.Payload = "OK";
            return response;
        }
    }
}
