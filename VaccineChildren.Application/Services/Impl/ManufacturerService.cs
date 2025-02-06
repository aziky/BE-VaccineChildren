using AutoMapper;
using Microsoft.Extensions.Logging;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VaccineChildren.Application.Services.Impl
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly ILogger<IManufacturerService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Manufacturer> _manufacturerRepository;

        public ManufacturerService(ILogger<IManufacturerService> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _manufacturerRepository = _unitOfWork.GetRepository<Manufacturer>();
        }

        // 1. Create Manufacturer
        public async Task CreateManufacturer(ManufacturerReq manufacturerReq)
        {
            try
            {
                _logger.LogInformation("Creating new manufacturer");

                var manufacturer = _mapper.Map<Manufacturer>(manufacturerReq);
                manufacturer.ManufacturerId = Guid.NewGuid();
                manufacturer.IsActive = true;
                manufacturer.CreatedAt = DateTime.Now;

                await _manufacturerRepository.InsertAsync(manufacturer);
                await _unitOfWork.SaveChangeAsync();

                _logger.LogInformation("Manufacturer created successfully");
            }
            catch (Exception e)
            {
                _logger.LogError("Error while creating manufacturer: {Error}", e.Message);
                throw;
            }
        }

        // 2. Get Manufacturer By Id
        public async Task<ManufacturerRes> GetManufacturerById(Guid manufacturerId)
        {
            try
            {
                _logger.LogInformation("Retrieving manufacturer with ID: {ManufacturerId}", manufacturerId);

                var manufacturer = await _manufacturerRepository.GetByIdAsync(manufacturerId);
                if (manufacturer == null)
                {
                    _logger.LogInformation("Manufacturer not found with ID: {ManufacturerId}", manufacturerId);
                    throw new KeyNotFoundException("Manufacturer not found");
                }

                return _mapper.Map<ManufacturerRes>(manufacturer);
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving manufacturer: {Error}", e.Message);
                throw;
            }
        }

        // 3. Get All Manufacturers
        public async Task<List<ManufacturerRes>> GetAllManufacturers()
        {
            try
            {
                _logger.LogInformation("Retrieving all manufacturers");

                var manufacturers = await _manufacturerRepository.GetAllAsync();
                if (manufacturers == null || manufacturers.Count == 0)
                {
                    _logger.LogInformation("No manufacturers found");
                    return new List<ManufacturerRes>();
                }

                return _mapper.Map<List<ManufacturerRes>>(manufacturers);
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving manufacturers: {Error}", e.Message);
                throw;
            }
        }

        // 4. Update Manufacturer
        public async Task UpdateManufacturer(Guid manufacturerId, ManufacturerReq manufacturerReq)
        {
            try
            {
                _logger.LogInformation("Updating manufacturer with ID: {ManufacturerId}", manufacturerId);

                var manufacturer = await _manufacturerRepository.GetByIdAsync(manufacturerId);
                if (manufacturer == null)
                {
                    _logger.LogInformation("Manufacturer not found with ID: {ManufacturerId}", manufacturerId);
                    throw new KeyNotFoundException("Manufacturer not found");
                }

                manufacturer.Name = manufacturerReq.Name;
                manufacturer.ShortName = manufacturerReq.ShortName;
                manufacturer.Description = manufacturerReq.Description;
                manufacturer.CountryName = manufacturerReq.CountryName;
                manufacturer.CountryCode = manufacturerReq.CountryCode;
                manufacturer.IsActive = manufacturerReq.IsActive;   
                manufacturer.UpdatedAt = DateTime.Now;

                await _manufacturerRepository.UpdateAsync(manufacturer);
                await _unitOfWork.SaveChangeAsync();

                _logger.LogInformation("Manufacturer updated successfully");
            }
            catch (Exception e)
            {
                _logger.LogError("Error while updating manufacturer: {Error}", e.Message);
                throw;
            }
        }

        // 5. Delete Manufacturer
        public async Task DeleteManufacturer(Guid manufacturerId)
        {
            try
            {
                _logger.LogInformation("Deleting manufacturer with ID: {ManufacturerId}", manufacturerId);

                var manufacturer = await _manufacturerRepository.GetByIdAsync(manufacturerId);
                if (manufacturer == null)
                {
                    _logger.LogInformation("Manufacturer not found with ID: {ManufacturerId}", manufacturerId);
                    throw new KeyNotFoundException("Manufacturer not found");
                }

                manufacturer.IsActive = false; // Đánh dấu là bị xóa (inactive)
                await _manufacturerRepository.UpdateAsync(manufacturer);
                await _unitOfWork.SaveChangeAsync();

                _logger.LogInformation("Manufacturer deleted successfully");
            }
            catch (Exception e)
            {
                _logger.LogError("Error while deleting manufacturer: {Error}", e.Message);
                throw;
            }
        }
    }
}
