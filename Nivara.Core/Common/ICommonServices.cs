using Nivara.Models;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nivara.Core.Common
{
    public interface ICommonServices
    {
        Task<List<CitiesModel>> GetCitiesByStateId(int stateId);
        Task<List<StatesModel>> GetStatesByCountryId(int countryId);
        Task<List<CountriesModel>> GetCountries();
        Task<List<TaskStatusModel>> GetTaskStatus();
    }
}
