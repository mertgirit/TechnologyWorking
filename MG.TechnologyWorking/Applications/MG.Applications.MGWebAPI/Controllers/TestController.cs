using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace MG.Applications.MGWebAPI.Controllers
{
    using MG.Settings.Configuration;

    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> logger;
        private readonly ApiAdrressConfiguration ApiAdressConfiguration;

        public TestController(IConfiguration configuration,
                              ILogger<TestController> logger)
        {
            this.logger = logger;

            var ApiAdressConfiguration = configuration.GetSection("ApiAddressConfiguration").Get<ApiAdrressConfiguration>();
            this.ApiAdressConfiguration = ApiAdressConfiguration;
        }

        [HttpGet]
        public string Index()
        {
            logger.Log(LogLevel.Information, "Api Run");
            return $"{nameof(TestController)} is running";
        }
    }
}