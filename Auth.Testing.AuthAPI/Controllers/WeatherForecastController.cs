using Auth.Core.Application.Features.Login.Queries.AuthLogin;
using Auth.Infraestructure.Identity.Features.AuthenticateEmail.Command.AuthEmail;
using Auth.Infraestructure.Identity.Features.Register.Commands.CreateAccount;
using Auth.Infraestructure.Identity.Features.Register.Commands.SendValidationEmailAgain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Testing.AuthAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public IMediator Mediator { get; }
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IMediator mediator, ILogger<WeatherForecastController> logger)
        {
            Mediator = mediator;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> AuthLogin([FromBody] AuthLoginQuery request)
        {
            var data = await Mediator.Send(request);
            return Ok(data);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateAccountCommand request)
        {
            var data = await Mediator.Send(request);
            return Ok(data);
        }

        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] AuthEmailCommand request)
        {
            var data = await Mediator.Send(request);
            return Ok(data);
        }

        [HttpPost("ResentConfirmation")]
        public async Task<IActionResult> ResentConfirmation([FromBody] SendValidationEmailAgainCommand request)
        {
            var data = await Mediator.Send(request);
            return Ok(data);
        }
    }
}
