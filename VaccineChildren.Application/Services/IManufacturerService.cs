using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;

namespace VaccineChildren.Application.Services
{
    public interface IManufacturerService
    {
        Task CreateManufacturer(ManufacturerReq manufacturerReq);
        Task<ManufacturerRes> GetManufacturerById(Guid manufacturerId);
        Task<List<ManufacturerRes>> GetAllManufacturers();
        Task UpdateManufacturer(Guid manufacturerId, ManufacturerReq manufacturerReq);
        Task DeleteManufacturer(Guid manufacturerId);
    }
}
