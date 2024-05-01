using Auth.Core.Application.Features.Login.Queries.AuthLogin;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpPost("login")]
        public async Task<IActionResult> AuthLogin([FromBody] AuthLoginQuery request)
        {
            var data = await Mediator.Send(request);
            return Ok(data);
        }
    }
}
