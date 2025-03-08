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
        Task<(IEnumerable<VaccineRes> Vaccines, int TotalCount)> GetAllVaccines(int pageIndex = 1, int pageSize = 10);
        Task UpdateVaccine(Guid vaccineId, VaccineReq vaccineReq);
        Task DeleteVaccine(Guid vaccineId);
        Task<IEnumerable<VaccineRes>> GetAllVaccinesForEachAge(int minAge, int maxAge, string unit);

        Task<IEnumerable<VaccineRes>> GetVaccinesByNameDifferentManufacturers(string vaccineName);
        Task<IList<VaccineRes>> GetAllVaccines();
    }
}
