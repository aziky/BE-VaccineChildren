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
                vaccine.VaccineManufacture = vaccineManufacture;

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

                // Get the manufacturer name from the VaccineManufacture relation
                var manufacturerName = vaccine.VaccineManufacture?.Manufacturer?.Name;
                var price = vaccine.VaccineManufacture?.Price ?? 0;

                var vaccineRes = _mapper.Map<VaccineRes>(vaccine);
                vaccineRes.Description = System.Text.Json.JsonSerializer.Deserialize<DTOs.Response.DescriptionDetail>(vaccine.Description);

                vaccineRes.ManufacturerName = manufacturerName;
                vaccineRes.Price = price;

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

                var vaccines = await _vaccineRepository.GetAllAsync();
                if (vaccines == null || vaccines.Count == 0)
                {
                    _logger.LogInformation("No vaccines found");
                    return new List<VaccineRes>();
                }

                // Map to VaccineRes and set ManufacturerName and Price from related tables
                var vaccineResList = new List<VaccineRes>();

                foreach (var vaccine in vaccines)
                {
                    var manufacturerName = vaccine.VaccineManufacture?.Manufacturer?.Name;
                    var price = vaccine.VaccineManufacture?.Price ?? 0;

                    var vaccineRes = _mapper.Map<VaccineRes>(vaccine);
                    vaccineRes.ManufacturerName = manufacturerName;
                    vaccineRes.Price = price;
                    vaccineRes.Description = System.Text.Json.JsonSerializer.Deserialize<DTOs.Response.DescriptionDetail>(vaccine.Description);

                    vaccineResList.Add(vaccineRes);
                }

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
                if (vaccine.VaccineManufacture == null)
                {
                    vaccine.VaccineManufacture = new VaccineManufacture
                    {
                        ManufacturerId = manufacturer.ManufacturerId,
                        VaccineId = vaccine.VaccineId,
                        Price = vaccineReq.Price
                    };
                }
                else
                {
                    vaccine.VaccineManufacture.Price = vaccineReq.Price;
                    vaccine.VaccineManufacture.ManufacturerId = manufacturer.ManufacturerId;
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

        public async Task<List<VaccineRes>> GetAllVaccines9MonthAge()
        {
            try
            {
                _logger.LogInformation("Retrieving vaccines with MinAge = 0, MaxAge = 9, Unit = 'month'");

                var vaccines = await _vaccineRepository.GetAllAsync();
                var filteredVaccines = vaccines
                    .Where(v => v.MinAge >= 0 && v.MaxAge <= 9 && v.Unit == "month")
                    .ToList();

                if (!filteredVaccines.Any())
                {
                    _logger.LogInformation("No matching vaccines found");
                    return new List<VaccineRes>();
                }

                var vaccineResList = filteredVaccines.Select(vaccine =>
                {
                    var vaccineRes = _mapper.Map<VaccineRes>(vaccine);
                    vaccineRes.ManufacturerName = vaccine.VaccineManufacture?.Manufacturer?.Name;
                    vaccineRes.Price = vaccine.VaccineManufacture?.Price ?? 0;
                    return vaccineRes;
                }).ToList();

                _logger.LogInformation("Retrieved {Count} vaccines matching criteria", vaccineResList.Count);
                return vaccineResList;
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving filtered vaccines: {Error}", e.Message);
                throw;
            }
        }

        public async Task<List<VaccineRes>> GetAllVaccines12MonthAge()
        {
            try
            {
                _logger.LogInformation("Retrieving vaccines with MinAge = 0, MaxAge = 12, Unit = 'month'");

                var vaccines = await _vaccineRepository.GetAllAsync();
                var filteredVaccines = vaccines
                    .Where(v => v.MinAge >= 0 && v.MaxAge <= 12 && v.Unit == "month")
                    .ToList();

                if (!filteredVaccines.Any())
                {
                    _logger.LogInformation("No matching vaccines found");
                    return new List<VaccineRes>();
                }

                var vaccineResList = filteredVaccines.Select(vaccine =>
                {
                    var vaccineRes = _mapper.Map<VaccineRes>(vaccine);
                    vaccineRes.ManufacturerName = vaccine.VaccineManufacture?.Manufacturer?.Name;
                    vaccineRes.Price = vaccine.VaccineManufacture?.Price ?? 0;
                    return vaccineRes;
                }).ToList();

                _logger.LogInformation("Retrieved {Count} vaccines matching criteria", vaccineResList.Count);
                return vaccineResList;
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving filtered vaccines: {Error}", e.Message);
                throw;
            }
        }

        public async Task<List<VaccineRes>> GetAllVaccines24MonthAge()
        {
            try
            {
                _logger.LogInformation("Retrieving vaccines with MinAge >= 0, MaxAge <= 24, Unit = 'month'");

                var vaccines = await _vaccineRepository.GetAllAsync();
                var filteredVaccines = vaccines
                    .Where(v => v.MinAge >= 0 && v.MaxAge <= 24 && v.Unit == "month")
                    .ToList();

                if (!filteredVaccines.Any())
                {
                    _logger.LogInformation("No matching vaccines found");
                    return new List<VaccineRes>();
                }

                var vaccineResList = filteredVaccines.Select(vaccine =>
                {
                    var vaccineRes = _mapper.Map<VaccineRes>(vaccine);
                    vaccineRes.ManufacturerName = vaccine.VaccineManufacture?.Manufacturer?.Name;
                    vaccineRes.Price = vaccine.VaccineManufacture?.Price ?? 0;
                    return vaccineRes;
                }).ToList();

                _logger.LogInformation("Retrieved {Count} vaccines matching criteria", vaccineResList.Count);
                return vaccineResList;
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving filtered vaccines: {Error}", e.Message);
                throw;
            }
        }

        public async Task<List<VaccineRes>> GetAllVaccines4To8YearsAge()
        {
            try
            {
                _logger.LogInformation("Retrieving vaccines with MinAge >= 4, MaxAge <= 8, Unit = 'year'");

                var vaccines = await _vaccineRepository.GetAllAsync();
                var filteredVaccines = vaccines
                    .Where(v => v.MinAge >= 4 && v.MaxAge <= 8 && v.Unit == "year")
                    .ToList();

                if (!filteredVaccines.Any())
                {
                    _logger.LogInformation("No matching vaccines found");
                    return new List<VaccineRes>();
                }

                var vaccineResList = filteredVaccines.Select(vaccine =>
                {
                    var vaccineRes = _mapper.Map<VaccineRes>(vaccine);
                    vaccineRes.ManufacturerName = vaccine.VaccineManufacture?.Manufacturer?.Name;
                    vaccineRes.Price = vaccine.VaccineManufacture?.Price ?? 0;
                    return vaccineRes;
                }).ToList();

                _logger.LogInformation("Retrieved {Count} vaccines matching criteria", vaccineResList.Count);
                return vaccineResList;
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving filtered vaccines: {Error}", e.Message);
                throw;
            }
        }

    }
}