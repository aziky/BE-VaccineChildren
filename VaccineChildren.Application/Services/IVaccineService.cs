using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Application.Services
{
    public interface IVaccineService
    {
        Task CreateVaccine(VaccineReq vaccineReq);
        Task<VaccineRes> GetVaccineById(Guid vaccineId);
        Task<List<VaccineRes>> GetAllVaccines();
        Task UpdateVaccine(Guid vaccineId, VaccineReq vaccineReq);
        Task DeleteVaccine(Guid vaccineId);
        Task<IEnumerable<VaccineRes>> GetAllVaccinesForEachAge(int minAge, int maxAge, string unit);

    }
}
