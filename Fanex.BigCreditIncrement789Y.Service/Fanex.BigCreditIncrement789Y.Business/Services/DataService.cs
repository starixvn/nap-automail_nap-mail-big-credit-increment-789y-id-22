using Fanex.BigCreditIncrement789Y.Business.Models;
using Fanex.Data.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.BigCreditIncrement789Y.Business.Services
{
    public interface DataService
    {
        List<PunterModel> QueryBigCreditIncrement();
    }

    public class DataServiceImp(IDynamicRepository repository, DateTimeServiceProvider dateService)
        : DataService
    {
        public List<PunterModel> QueryBigCreditIncrement()
        {
            var criteria = new Criteria.Criteria
            {
                FromDate = dateService.DateTime.FromDate,
                ToDate = dateService.DateTime.ToDate,
                IsTest = AppConfigService.Config.IsTest
            };

            return repository.Fetch<PunterModel>(criteria).ToList();
        }
    }
}