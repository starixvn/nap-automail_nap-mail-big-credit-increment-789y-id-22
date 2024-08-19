using Fanex.MailDelivery.Core.Models;
using Fanex.BigCreditIncrement789Y.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.BigCreditIncrement789Y.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BigCreditIncrement789YController : ControllerBase
    {
        private readonly MainService service;

        public BigCreditIncrement789YController()
        {
            var container = RegisterService.Container;
            service = container.GetInstance<MainService>();
        }

        [HttpGet]
        public JobMessage Get()
        {
            return service.Execute();
        }
    }
}