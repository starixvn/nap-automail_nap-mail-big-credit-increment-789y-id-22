using Fanex.MailDelivery.Core.Models;
using Fanex.BigCreditIncrement789Y.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.BigCreditIncrement789Y.Service.Controllers
{
    using Microsoft.Extensions.DependencyInjection;

    [ApiController]
    [Route("[controller]")]
    public class BigCreditIncrement789YController : ControllerBase
    {
        private readonly MainService service;

        public BigCreditIncrement789YController()
        {
            var services = ServiceCollectionExtensions.Services;
            var provider = services.BuildServiceProvider();
            service = provider.GetService<MainService>();
        }

        [HttpGet]
        public JobMessage Get()
        {
            return service.Execute();
        }
    }
}