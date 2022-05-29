using Microsoft.EntityFrameworkCore;
using Nivara.Data.Models.NivaraDbContext;
using Nivara.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nivara.Core.Common
{
    public class CommonServices: ICommonServices
    {
        private readonly NivaraDbContext context;
        public CommonServices(NivaraDbContext nivaraDb)
        {
            context = nivaraDb;
        }
        public async Task<List<CitiesModel>> GetCitiesByStateId(int stateId)
        {
            return await context.Cities.Where(x => x.StateId == stateId).Select(x => new CitiesModel { Id = x.Id, Name = x.Name }).ToListAsync();
        }

        public async Task<List<StatesModel>> GetStatesByCountryId(int countryId)
        {
            return await context.States.Where(x => x.CounteryId == countryId).Select(x => new StatesModel { Id = x.Id, Name = x.Name }).ToListAsync();
        }

        public async Task<List<CountriesModel>> GetCountries()
        {
            return await context.Countries.Select(x => new CountriesModel { Id = x.Id, Name = x.Name }).ToListAsync();
        }

        public async Task<List<TaskStatusModel>> GetTaskStatus()
        {
            return await context.TaskStatus.Select(x => new TaskStatusModel { Id = x.Id, Name = x.Name }).ToListAsync();
        }
    }
}
