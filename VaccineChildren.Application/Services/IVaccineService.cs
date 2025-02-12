using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VaccineChildren.Application.Services
{
    public interface IVaccineService
    {
        Task CreateVaccine(VaccineReq vaccineReq);
        Task<VaccineRes> GetVaccineById(Guid vaccineId);
        Task<List<VaccineRes>> GetAllVaccines();
        Task UpdateVaccine(Guid vaccineId, VaccineReq vaccineReq);
        Task DeleteVaccine(Guid vaccineId);
        Task<List<VaccineRes>> GetAllVaccines9MonthAge();
        Task<List<VaccineRes>> GetAllVaccines12MonthAge();
        Task<List<VaccineRes>> GetAllVaccines24MonthAge();     
        Task<List<VaccineRes>> GetAllVaccines4To8YearsAge();
    }
}
