using Fanex.BigCreditIncrement789Y.Business.Services;
using Fanex.MailDelivery.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.BigCreditIncrement789Y.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BigCreditIncrement789YController : ControllerBase
    {
        private readonly MainService service;

        public BigCreditIncrement789YController(MainService service)
        {
            this.service = service;
        }

        [HttpGet]
        public JobMessage Get()
        {
            return service.Execute();
        }
    }
}