using AutoMapper;
using Microsoft.Extensions.Logging;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Domain.Entities;
using VaccineChildren.Domain.Abstraction;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VaccineChildren.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace VaccineChildren.Application.Services.Impl
{
    public class VaccineService : IVaccineService
    {
        private readonly ILogger<IVaccineService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        private readonly IGenericRepository<Vaccine> _vaccineRepository;

        public VaccineService(ILogger<IVaccineService> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _vaccineRepository = _unitOfWork.GetRepository<Vaccine>();
        }

        // 1. Create Vaccine
        public async Task CreateVaccine(VaccineReq vaccineReq)
        {
            try
            {
                _logger.LogInformation("Start creating vaccine");
                if (vaccineReq.MinAge > vaccineReq.MaxAge)
                {
                    throw new CustomExceptions.ValidationException("MinAge cannot be greater than MaxAge.");
                }



                // Ensure ManufacturerId is valid
                var manufacturer = await _unitOfWork.GetRepository<Manufacturer>().GetByIdAsync(Guid.Parse(vaccineReq.ManufacturerId));
                if (manufacturer == null)
                {
                    throw new CustomExceptions.EntityNotFoundException("Manufacturer", vaccineReq.ManufacturerId);
                }

                // Create new Vaccine entity
                var vaccine = new Vaccine
                {
                    VaccineId = Guid.NewGuid(),
                    VaccineName = vaccineReq.VaccineName,
                    MinAge = vaccineReq.MinAge,
                    MaxAge = vaccineReq.MaxAge,
                    IsActive = true,
                    NumberDose = vaccineReq.NumberDose,
                    Unit = vaccineReq.Unit,
                    Duration = vaccineReq.Duration,
                    Image = vaccineReq.Image,
                    CreatedAt = DateTime.UtcNow.ToLocalTime(),
                };
                vaccine.Description = System.Text.Json.JsonSerializer.Serialize(vaccineReq.Description);
                // Create VaccineManufacture and assign it to Vaccine
                var vaccineManufacture = new VaccineManufacture
                {
                    ManufacturerId = manufacturer.ManufacturerId,
                    VaccineId = vaccine.VaccineId,
                    Price = vaccineReq.Price
                };

                // Assign the VaccineManufacture to Vaccine
                vaccine.VaccineManufactures.Add(vaccineManufacture);

                // Insert Vaccine and related VaccineManufacture into database
                await _vaccineRepository.InsertAsync(vaccine);
                await _unitOfWork.SaveChangeAsync();

                _logger.LogInformation("Vaccine created successfully");
            }
            catch (Exception e)
            {
                _logger.LogError("Error while creating vaccine: {Error}", e.Message);
                throw;
            }
        }

        // 2. Get Vaccine By Id
        public async Task<VaccineRes> GetVaccineById(Guid vaccineId)
        {
            try
            {
                _logger.LogInformation("Retrieving vaccine with ID: {VaccineId}", vaccineId);

                var vaccine = await _vaccineRepository.GetByIdAsync(vaccineId);
                if (vaccine == null)
                {
                    _logger.LogInformation("Vaccine not found with ID: {VaccineId}", vaccineId);
                    throw new KeyNotFoundException("Vaccine not found");
                }

                var firstVaccineManufacture = vaccine.VaccineManufactures?.FirstOrDefault();
                var manufacturer = firstVaccineManufacture?.Manufacturer;
                var price = firstVaccineManufacture?.Price ?? 0;

                var vaccineRes = _mapper.Map<VaccineRes>(vaccine);
                vaccineRes.Description = System.Text.Json.JsonSerializer.Deserialize<DTOs.Response.DescriptionDetail>(vaccine.Description);
                vaccineRes.Price = price;

                // Ánh xạ Manufacturer vào DTO
                if (manufacturer != null)
                {
                    vaccineRes.Manufacturer = new ManufacturerRes
                    {
                        ManufacturerId = manufacturer.ManufacturerId,
                        Name = manufacturer.Name,
                        ShortName = manufacturer.ShortName,
                        Description = manufacturer.Description,
                        CountryName = manufacturer.CountryName,
                        CountryCode = manufacturer.CountryCode,
                        IsActive = manufacturer.IsActive
                    };
                }

                return vaccineRes;
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving vaccine: {Error}", e.Message);
                throw;
            }
        }



        public async Task<List<VaccineRes>> GetAllVaccines()
        {
            try
            {
                _logger.LogInformation("Retrieving all vaccines");

                var vaccines = await _vaccineRepository.GetAllAsync(query => query
                    .Where(v => v.IsActive == true));
                if (vaccines == null || vaccines.Count == 0)
                {
                    _logger.LogInformation("No vaccines found");
                    return new List<VaccineRes>();
                }

                var vaccineResList = vaccines.Select(vaccine =>
                {
                    var firstVaccineManufacture = vaccine.VaccineManufactures?.FirstOrDefault();
                    var manufacturer = firstVaccineManufacture?.Manufacturer;
                    var price = firstVaccineManufacture?.Price ?? 0;

                    var vaccineRes = _mapper.Map<VaccineRes>(vaccine);
                    vaccineRes.Description = System.Text.Json.JsonSerializer.Deserialize<DTOs.Response.DescriptionDetail>(vaccine.Description);
                    vaccineRes.Price = price;

                    // Ánh xạ Manufacturer vào DTO
                    if (manufacturer != null)
                    {
                        vaccineRes.Manufacturer = new ManufacturerRes
                        {
                            ManufacturerId = manufacturer.ManufacturerId,
                            Name = manufacturer.Name,
                            ShortName = manufacturer.ShortName,
                            Description = manufacturer.Description,
                            CountryName = manufacturer.CountryName,
                            CountryCode = manufacturer.CountryCode,
                            IsActive = manufacturer.IsActive
                        };
                    }

                    return vaccineRes;
                }).ToList();

                return vaccineResList;
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving vaccines: {Error}", e.Message);
                throw;
            }
        }



        // 4. Update Vaccine
        public async Task UpdateVaccine(Guid vaccineId, VaccineReq vaccineReq)
        {
            try
            {
                _logger.LogInformation("Start updating vaccine with ID: {VaccineId}", vaccineId);
                if (vaccineReq.MinAge > vaccineReq.MaxAge)
                {
                    throw new CustomExceptions.ValidationException("MinAge cannot be greater than MaxAge.");
                }

                var vaccine = await _vaccineRepository.GetByIdAsync(vaccineId);

                if (vaccine == null)
                {
                    _logger.LogInformation("Vaccine not found with ID: {VaccineId}", vaccineId);
                    throw new KeyNotFoundException("Vaccine not found");
                }

                // Update vaccine properties
                vaccine.VaccineName = vaccineReq.VaccineName;
                vaccine.Description = System.Text.Json.JsonSerializer.Serialize(vaccineReq.Description);
                vaccine.MinAge = vaccineReq.MinAge;
                vaccine.MaxAge = vaccineReq.MaxAge;
                vaccine.IsActive = vaccineReq.IsActive;
                vaccine.Duration = vaccineReq.Duration;
                vaccine.NumberDose = vaccineReq.NumberDose;
                vaccine.Unit = vaccineReq.Unit;
                vaccine.Image = vaccineReq.Image;


                // Ensure Manufacturer is valid
                var manufacturer = await _unitOfWork.GetRepository<Manufacturer>().GetByIdAsync(Guid.Parse(vaccineReq.ManufacturerId));
                if (manufacturer == null)
                {
                    _logger.LogError("Manufacturer not found with ID: {ManufacturerId}", vaccineReq.ManufacturerId);
                    throw new KeyNotFoundException("Manufacturer not found");
                }

                // Update the VaccineManufacture for the vaccine
                if (vaccine.VaccineManufactures == null)
                {
                    vaccine.VaccineManufactures = new List<VaccineManufacture>(); // Khởi tạo danh sách nếu null
                }

// Kiểm tra xem vaccine đã có VaccineManufacture từ nhà sản xuất này chưa
                var existingVaccineManufacture = vaccine.VaccineManufactures
                    .FirstOrDefault(vm => vm.ManufacturerId == manufacturer.ManufacturerId);

                if (existingVaccineManufacture == null)
                {
                    // Thêm mới VaccineManufacture vào danh sách
                    vaccine.VaccineManufactures.Add(new VaccineManufacture
                    {
                        ManufacturerId = manufacturer.ManufacturerId,
                        VaccineId = vaccine.VaccineId,
                        Price = vaccineReq.Price
                    });
                }
                else
                {
                    // Cập nhật giá cho VaccineManufacture đã tồn tại
                    existingVaccineManufacture.Price = vaccineReq.Price;
                }

                vaccine.UpdatedAt = DateTime.UtcNow.ToLocalTime();

                await _vaccineRepository.UpdateAsync(vaccine);
                await _unitOfWork.SaveChangeAsync();

                _logger.LogInformation("Vaccine updated successfully");
            }
            catch (Exception e)
            {
                _logger.LogError("Error while updating vaccine: {Error}", e.Message);
                throw;
            }
        }

        // 5. Delete Vaccine
        public async Task DeleteVaccine(Guid vaccineId)
        {
            try
            {
                _logger.LogInformation("Start deleting vaccine with ID: {VaccineId}", vaccineId);

                var vaccine = await _vaccineRepository.GetByIdAsync(vaccineId);

                if (vaccine == null)
                {
                    _logger.LogInformation("Vaccine not found with ID: {VaccineId}", vaccineId);
                    throw new KeyNotFoundException("Vaccine not found");
                }

                vaccine.IsActive = false; // Đánh dấu vaccine là đã bị xóa (inactive)
                await _vaccineRepository.UpdateAsync(vaccine);
                await _unitOfWork.SaveChangeAsync();

                _logger.LogInformation("Vaccine deleted successfully");
            }
            catch (Exception e)
            {
                _logger.LogError("Error while deleting vaccine: {Error}", e.Message);
                throw;
            }
        }

        public async Task<IEnumerable<VaccineRes>> GetAllVaccinesForEachAge(int minAge, int maxAge, string unit)
    {
        try
        {
            _logger.LogInformation("Retrieving vaccines for age range: {MinAge}-{MaxAge} {Unit}", minAge, maxAge, unit);

            var vaccines = await _vaccineRepository.GetAllAsync(q => q
                .Include(v => v.VaccineManufactures) 
                .ThenInclude(vm => vm.Manufacturer) 
                .Where(v => v.MinAge <= maxAge && v.MaxAge >= minAge));


            vaccines = vaccines.Where(v => v.Unit.Equals(unit, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!vaccines.Any())
            {
                _logger.LogInformation("No vaccines found for the specified age range.");
                return new List<VaccineRes>();
            }

            var vaccineResList = vaccines.Select(vaccine =>
            {
                var firstVaccineManufacture = vaccine.VaccineManufactures?.FirstOrDefault();
                var manufacturer = firstVaccineManufacture?.Manufacturer;
                var price = firstVaccineManufacture?.Price ?? 0;

                var vaccineRes = _mapper.Map<VaccineRes>(vaccine);
                vaccineRes.Description = System.Text.Json.JsonSerializer.Deserialize<DTOs.Response.DescriptionDetail>(vaccine.Description);
                vaccineRes.Price = price;

                if (manufacturer != null)
                {
                    vaccineRes.Manufacturer = new ManufacturerRes
                    {
                        ManufacturerId = manufacturer.ManufacturerId,
                        Name = manufacturer.Name,
                        ShortName = manufacturer.ShortName,
                        Description = manufacturer.Description,
                        CountryName = manufacturer.CountryName,
                        CountryCode = manufacturer.CountryCode,
                        IsActive = manufacturer.IsActive
                    };
                }

                return vaccineRes;
            }).ToList();

            return vaccineResList;
        }
        catch (Exception e)
        {
            _logger.LogError("Error while retrieving vaccines for age range {MinAge}-{MaxAge} {Unit}: {Error}", minAge, maxAge, unit, e.Message);
            throw;
        }
    }

    }
}