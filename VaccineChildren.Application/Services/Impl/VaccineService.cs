using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Core.Exceptions;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;

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
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _vaccineRepository = _unitOfWork.GetRepository<Vaccine>();
        }

        // 1. Create Vaccine with Transaction
        public async Task CreateVaccine(VaccineReq vaccineReq)
        {
            try
            {
                _logger.LogInformation("Start creating vaccine");

                if (vaccineReq.MinAge > vaccineReq.MaxAge)
                {
                    throw new ValidationException("MinAge cannot be greater than MaxAge.");
                }

                _unitOfWork.BeginTransaction();

                var manufacturerId = Guid.Parse(vaccineReq.ManufacturerId);
                var manufacturerRepo = _unitOfWork.GetRepository<Manufacturer>();
                var manufacturer = await manufacturerRepo.GetByIdAsync(manufacturerId);
                if (manufacturer == null)
                {
                    throw new CustomExceptions.EntityNotFoundException("Manufacturer", vaccineReq.ManufacturerId);
                }

                var existingVaccine = await _vaccineRepository.FindAsync(v =>
                    v.VaccineName == vaccineReq.VaccineName &&
                    v.VaccineManufactures.Any(vm => vm.ManufacturerId == manufacturerId));
                if (existingVaccine != null)
                {
                    throw new ValidationException("A vaccine with the same name and manufacturer already exists.");
                }

                var vaccine = _mapper.Map<Vaccine>(vaccineReq);
                vaccine.VaccineId = Guid.NewGuid();
                vaccine.IsActive = true;
                vaccine.CreatedAt = DateTime.UtcNow.ToLocalTime();

                var vaccineManufacture = new VaccineManufacture
                {
                    ManufacturerId = manufacturerId,
                    VaccineId = vaccine.VaccineId,
                    Price = vaccineReq.Price
                };
                vaccine.VaccineManufactures = new List<VaccineManufacture> { vaccineManufacture };

                await _vaccineRepository.InsertAsync(vaccine);
                await _unitOfWork.SaveChangeAsync();

                _unitOfWork.CommitTransaction();
                _logger.LogInformation("Vaccine created successfully");
            }
            catch (Exception e)
            {
                if (_unitOfWork.HasActiveTransaction())
                {
                    _unitOfWork.RollBack();
                }
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

                // Truy vấn vaccine với tất cả manufacturers liên quan
                var vaccine = await _vaccineRepository.Entities
                    .AsNoTracking()
                    .Include(v => v.VaccineManufactures)
                    .ThenInclude(vm => vm.Manufacturer)
                    .FirstOrDefaultAsync(v => v.VaccineId == vaccineId);

                if (vaccine == null)
                {
                    _logger.LogInformation("Vaccine not found with ID: {VaccineId}", vaccineId);
                    throw new KeyNotFoundException("Vaccine not found");
                }

                // Map vaccine sang response DTO
                var vaccineRes = _mapper.Map<VaccineRes>(vaccine);

                // Đảm bảo Manufacturers được map đầy đủ từ VaccineManufactures
                if (vaccine.VaccineManufactures != null && vaccine.VaccineManufactures.Any())
                {
                    vaccineRes.Manufacturers = vaccine.VaccineManufactures
                        .Where(vm => vm.Manufacturer != null) // Đảm bảo Manufacturer không null
                        .Select(vm => _mapper.Map<ManufacturerRes>(vm.Manufacturer))
                        .Distinct() // Loại bỏ trùng lặp nếu có
                        .ToList();
                }
                else
                {
                    vaccineRes.Manufacturers = new List<ManufacturerRes>(); // Trả về list rỗng nếu không có manufacturers
                }

                _logger.LogInformation("Successfully retrieved vaccine with ID: {VaccineId} with {Count} manufacturers", 
                    vaccineId, vaccineRes.Manufacturers.Count);

                return vaccineRes;
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving vaccine {VaccineId}: {Error}", vaccineId, e.Message);
                throw;
            }
        }

        // 3. Get All Vaccines with Pagination
        public async Task<(IEnumerable<VaccineRes> Vaccines, int TotalCount)> GetAllVaccines(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Retrieving all vaccines, page {PageIndex}, size {PageSize}", pageIndex, pageSize);

                // Validate input parameters
                if (pageIndex < 1) pageIndex = 1;
                if (pageSize < 1) pageSize = 10;

                // Build the base query
                var query = _vaccineRepository.Entities
                    .AsNoTracking()
                    .Include(v => v.VaccineManufactures)
                    .ThenInclude(vm => vm.Manufacturer);

                // Get total count
                var totalCount = await query.CountAsync();

                // Retrieve paginated vaccines and map manually
                var vaccines = await query
                    .OrderBy(v => v.VaccineName)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Map to VaccineRes manually to ensure Manufacturers are included
                var vaccineResList = vaccines.Select(vaccine =>
                {
                    var vaccineRes = _mapper.Map<VaccineRes>(vaccine);
                    vaccineRes.Manufacturers = vaccine.VaccineManufactures?
                        .Where(vm => vm.Manufacturer != null)
                        .Select(vm => _mapper.Map<ManufacturerRes>(vm.Manufacturer))
                        .Distinct()
                        .ToList() ?? new List<ManufacturerRes>();
                    return vaccineRes;
                }).ToList();

                _logger.LogInformation("Retrieved {Count} vaccines out of {TotalCount}", vaccineResList.Count, totalCount);

                return (vaccineResList ?? new List<VaccineRes>(), totalCount);
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving vaccines: {Error}", e.Message);
                throw;
            }
        }

        // 4. Update Vaccine with Transaction
        public async Task UpdateVaccine(Guid vaccineId, VaccineReq vaccineReq)
        {
            try
            {
                _logger.LogInformation("Start updating vaccine with ID: {VaccineId}", vaccineId);

                if (vaccineReq.MinAge > vaccineReq.MaxAge)
                {
                    throw new ValidationException("MinAge cannot be greater than MaxAge.");
                }

                _unitOfWork.BeginTransaction();

                var vaccine = await _vaccineRepository.Entities
                    .Include(v => v.VaccineManufactures)
                    .FirstOrDefaultAsync(v => v.VaccineId == vaccineId);

                if (vaccine == null)
                {
                    throw new KeyNotFoundException("Vaccine not found");
                }

                _mapper.Map(vaccineReq, vaccine);
                vaccine.UpdatedAt = DateTime.UtcNow.ToLocalTime();

                var manufacturerId = Guid.Parse(vaccineReq.ManufacturerId);
                var manufacturerRepo = _unitOfWork.GetRepository<Manufacturer>();
                var manufacturer = await manufacturerRepo.GetByIdAsync(manufacturerId);
                if (manufacturer == null)
                {
                    throw new KeyNotFoundException("Manufacturer not found");
                }

                var existingManufacture = vaccine.VaccineManufactures
                    ?.FirstOrDefault(vm => vm.ManufacturerId == manufacturerId);
                if (existingManufacture == null)
                {
                    vaccine.VaccineManufactures ??= new List<VaccineManufacture>();
                    vaccine.VaccineManufactures.Add(new VaccineManufacture
                    {
                        ManufacturerId = manufacturerId,
                        VaccineId = vaccine.VaccineId,
                        Price = vaccineReq.Price
                    });
                }
                else
                {
                    existingManufacture.Price = vaccineReq.Price;
                }

                await _vaccineRepository.UpdateAsync(vaccine);
                await _unitOfWork.SaveChangeAsync();

                _unitOfWork.CommitTransaction();
                _logger.LogInformation("Vaccine updated successfully");
            }
            catch (Exception e)
            {
                if (_unitOfWork.HasActiveTransaction())
                {
                    _unitOfWork.RollBack();
                }
                _logger.LogError("Error while updating vaccine {VaccineId}: {Error}", vaccineId, e.Message);
                throw;
            }
        }

        // 5. Delete Vaccine with Transaction
        public async Task DeleteVaccine(Guid vaccineId)
        {
            try
            {
                _logger.LogInformation("Start deleting vaccine with ID: {VaccineId}", vaccineId);

                _unitOfWork.BeginTransaction();

                var vaccine = await _vaccineRepository.GetByIdAsync(vaccineId);
                if (vaccine == null || !vaccine.IsActive.HasValue || !vaccine.IsActive.Value)
                {
                    throw new KeyNotFoundException("Vaccine not found or already inactive");
                }

                vaccine.IsActive = false;
                await _vaccineRepository.UpdateAsync(vaccine);
                await _unitOfWork.SaveChangeAsync();

                _unitOfWork.CommitTransaction();
                _logger.LogInformation("Vaccine deleted successfully");
            }
            catch (Exception e)
            {
                if (_unitOfWork.HasActiveTransaction())
                {
                    _unitOfWork.RollBack();
                }
                _logger.LogError("Error while deleting vaccine {VaccineId}: {Error}", vaccineId, e.Message);
                throw;
            }
        }

        // 6. Get Vaccines by Age Range and Unit
        public async Task<IEnumerable<VaccineRes>> GetAllVaccinesForEachAge(int minAge, int maxAge, string unit)
        {
            try
            {
                _logger.LogInformation("Retrieving vaccines for age range: {MinAge}-{MaxAge} {Unit}", minAge, maxAge, unit);

                // Validate input parameters
                if (minAge < 0 || maxAge < 0)
                    throw new ArgumentException("MinAge and MaxAge must be non-negative.");
                if (minAge > maxAge)
                    throw new ArgumentException("MinAge cannot be greater than MaxAge.");
                if (string.IsNullOrWhiteSpace(unit))
                    throw new ArgumentException("Unit cannot be null or empty.");

                // Build the query with EF Core-compatible string comparison
                var vaccines = await _vaccineRepository.Entities
                    .AsNoTracking()
                    .Where(v => v.MinAge.HasValue && v.MinAge.Value <= maxAge &&
                                v.MaxAge.HasValue && v.MaxAge.Value >= minAge &&
                                v.Unit != null && v.Unit.ToLower() == unit.ToLower()) // Thay Equals bằng so sánh đơn giản
                    .Include(v => v.VaccineManufactures)
                    .ThenInclude(vm => vm.Manufacturer)
                    .ToListAsync();

                // Map to VaccineRes manually to ensure Manufacturers are included
                var vaccineResList = vaccines.Select(vaccine =>
                {
                    var vaccineRes = _mapper.Map<VaccineRes>(vaccine);
                    vaccineRes.Manufacturers = vaccine.VaccineManufactures?
                        .Where(vm => vm.Manufacturer != null)
                        .Select(vm => _mapper.Map<ManufacturerRes>(vm.Manufacturer))
                        .Distinct()
                        .ToList() ?? new List<ManufacturerRes>();
                    return vaccineRes;
                }).ToList();

                _logger.LogInformation("Retrieved {Count} vaccines for age range: {MinAge}-{MaxAge} {Unit}", 
                    vaccineResList.Count, minAge, maxAge, unit);

                return vaccineResList ?? new List<VaccineRes>();
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving vaccines for age range {MinAge}-{MaxAge} {Unit}: {Error}", 
                    minAge, maxAge, unit, e.Message);
                throw;
            }
        }

        // 7. Get Vaccines by Name with Different Manufacturers
        public async Task<IEnumerable<VaccineRes>> GetVaccinesByNameDifferentManufacturers(string vaccineName)
        {
            try
            {
                _logger.LogInformation("Retrieving vaccines with name: {VaccineName}", vaccineName);

                var vaccines = await _vaccineRepository.Entities
                    .AsNoTracking()
                    .Where(v => v.VaccineName == vaccineName)
                    .Include(v => v.VaccineManufactures)
                    .ThenInclude(vm => vm.Manufacturer)
                    .ToListAsync();

                if (!vaccines.Any())
                {
                    _logger.LogInformation("No vaccines found with name: {VaccineName}", vaccineName);
                    return new List<VaccineRes>();
                }

                // Map the vaccines to VaccineRes objects
                var vaccineResList = vaccines.Select(vaccine => {
                    var vaccineRes = _mapper.Map<VaccineRes>(vaccine);
                    // Ensure each vaccine has its manufacturers correctly mapped
                    vaccineRes.Manufacturers = vaccine.VaccineManufactures
                        .Select(vm => _mapper.Map<ManufacturerRes>(vm.Manufacturer))
                        .ToList();
                    return vaccineRes;
                }).ToList();

                // Filter vaccines with different manufacturers
                var result = vaccineResList
                    .GroupBy(v => v.VaccineName)
                    .Where(g => g.SelectMany(v => v.Manufacturers.Select(m => m.ManufacturerId))
                        .Distinct()
                        .Count() > 1)
                    .SelectMany(g => g)
                    .ToList();

                _logger.LogInformation("Found {ResultCount} vaccines with different manufacturers", result.Count);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving vaccines with name {VaccineName}: {Error}", vaccineName, e.Message);
                throw;
            }
        }

        public async Task<IList<VaccineRes>> GetAllVaccines()
        {
            var listVaccine =  await _vaccineRepository.GetAllAsync();
            return _mapper.Map<IList<VaccineRes>>(listVaccine);
        }
    }
}