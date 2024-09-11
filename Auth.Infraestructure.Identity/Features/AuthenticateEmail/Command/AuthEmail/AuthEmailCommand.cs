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
    public class AuthEmailCommand : IRequest<GenericApiResponse<bool>>
    {
        public string USERID { get; set; }
        public string TOKEN { get; set; }
    }

    public class AuthEmailCommandHandler : IRequestHandler<AuthEmailCommand, GenericApiResponse<bool>>
    {
        private readonly UserManager<ApplicationUser> _userManager;


        public AuthEmailCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<GenericApiResponse<bool>> Handle(AuthEmailCommand request, CancellationToken cancellationToken)
        {
            var response = new GenericApiResponse<bool>();

            var user = await _userManager.FindByIdAsync(request.USERID);
            if (user == null)
            {
                response.Message = $"Not account registered with this user";
                response.Statuscode = 404;
                response.Payload = false;
                response.Success = false;
                return response;
            }

            request.TOKEN = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.TOKEN));
            
            var result = await _userManager.ConfirmEmailAsync(user, request.TOKEN);
            
            if (!result.Succeeded)
            {
                response.Message = $"An error occurred while confirming {user.Email}";
                response.Statuscode = 400;
                response.Payload = false;
                response.Success = false;
                return response;
            }
            
            response.Success = true;
            response.Message = $"Account confirmed for {user.Email}. You can now use the App";
            response.Statuscode = 200;
            response.Payload = true;
            return response;
        }
    }
}
