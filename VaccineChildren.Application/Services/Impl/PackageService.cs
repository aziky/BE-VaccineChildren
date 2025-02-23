    using AutoMapper;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VaccineChildren.Application.DTOs.Request;
    using VaccineChildren.Application.DTOs.Response;
    using VaccineChildren.Domain.Abstraction;
    using VaccineChildren.Domain.Entities;

    public class PackageService : IPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PackageService> _logger;
        private readonly IGenericRepository<Package> _packageRepository;
        private readonly IGenericRepository<Vaccine> _vaccineRepository;

        public PackageService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PackageService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _packageRepository = _unitOfWork.GetRepository<Package>();
            _vaccineRepository = _unitOfWork.GetRepository<Vaccine>();
        }

        public async Task CreatePackage(PackageReq packageReq)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                _logger.LogInformation("Creating new package");

                var package = _mapper.Map<Package>(packageReq);
                package.PackageId = Guid.NewGuid();
                package.CreatedAt = DateTime.UtcNow.ToLocalTime();
                package.IsActive = true;

                decimal totalVaccinePrice = 0;

                if (packageReq.VaccineIds != null && packageReq.VaccineIds.Any())
                {
                    var vaccines = await _vaccineRepository.GetAllAsync("VaccineManufactures");
                    vaccines = vaccines.Where(v => packageReq.VaccineIds.Contains(v.VaccineId)).ToList();
                    totalVaccinePrice = vaccines.Sum(v => v.VaccineManufactures.Sum(vm => vm.Price ?? 0));
                    package.Vaccines = vaccines;
                }

                package.Price = totalVaccinePrice * (1 - (packageReq.Discount / 100));

                await _packageRepository.InsertAsync(package);
                await _unitOfWork.SaveChangeAsync();

                _unitOfWork.CommitTransaction();
                _logger.LogInformation("Package created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.RollBack();
                _logger.LogError(ex, "Error while creating package");
                throw;
            }
        }


        public async Task<PackageRes?> GetPackageById(Guid packageId)
        {
            try
            {
                _logger.LogInformation("Retrieving package with ID: {PackageId}", packageId);
                // Lấy package với thông tin Vaccine
                var package = await _packageRepository.FindAsync(p => p.PackageId == packageId, "Vaccines");

                return package != null ? _mapper.Map<PackageRes>(package) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving package with ID: {PackageId}", packageId);
                throw;
            }
        }


        public async Task<List<PackageRes>> GetAllPackages()
        {
            try
            {
                _logger.LogInformation("Retrieving all packages");
                var packages = await _packageRepository.GetAllAsync(includeProperties: "Vaccines");
                
                return _mapper.Map<List<PackageRes>>(packages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all packages");
                throw;
            }
        }


        public async Task UpdatePackage(Guid packageId, PackageReq packageReq)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                _logger.LogInformation("Updating package with ID: {PackageId}", packageId);
                var package = await _packageRepository.FindAsync(p => p.PackageId == packageId, "Vaccines");
                if (package == null) throw new KeyNotFoundException("Package not found");

                _mapper.Map(packageReq, package);
                package.UpdatedAt = DateTime.UtcNow.ToLocalTime();

                decimal totalVaccinePrice = 0;
                if (packageReq.VaccineIds != null)
                {
                    var vaccines = await _vaccineRepository.GetAllAsync("VaccineManufactures");
                    vaccines = vaccines.Where(v => packageReq.VaccineIds.Contains(v.VaccineId)).ToList();
                    totalVaccinePrice = vaccines.Sum(v => v.VaccineManufactures.Sum(vm => vm.Price ?? 0));
                    package.Vaccines = vaccines;
                }

                package.Price = totalVaccinePrice * (1 - (packageReq.Discount / 100));

                await _packageRepository.UpdateAsync(package);
                await _unitOfWork.SaveChangeAsync();

                _unitOfWork.CommitTransaction();
                _logger.LogInformation("Package updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.RollBack();
                _logger.LogError(ex, "Error updating package with ID: {PackageId}", packageId);
                throw;
            }
        }
        
        public async Task<PackageRes> UpdatePackageVaccines(Guid packageId, UpdatePackageVaccinesReq request)
{
    _unitOfWork.BeginTransaction();
    try
    {
        _logger.LogInformation("Updating vaccines for package with ID: {PackageId}", packageId);

        // 1. Lấy package hiện tại từ database
        var package = await _packageRepository.FindAsync(p => p.PackageId == packageId, "Vaccines.VaccineManufactures");
        if (package == null) throw new KeyNotFoundException("Package not found");

        var currentVaccines = package.Vaccines.ToList();
        var allVaccines = await _vaccineRepository.GetAllAsync("VaccineManufactures");

        // 2. Kiểm tra danh sách vaccine mới không bị thêm hoặc xóa
        if (request.VaccineIds.Count != currentVaccines.Count)
        {
            throw new InvalidOperationException("You can only change manufacturers, not add or remove vaccines.");
        }

        // 3. Lấy danh sách vaccine mới từ request
        var newVaccines = allVaccines.Where(v => request.VaccineIds.Contains(v.VaccineId)).ToList();

        // 4. Kiểm tra vaccine có cùng tên và khác manufacturer hay không
        var updatedVaccines = new List<Vaccine>();
        bool hasChanges = false;

        foreach (var currentVaccine in currentVaccines)
        {
            var matchingNewVaccine = newVaccines.FirstOrDefault(v => v.VaccineName == currentVaccine.VaccineName);
            if (matchingNewVaccine == null)
            {
                throw new InvalidOperationException($"Vaccine {currentVaccine.VaccineName} must be included in the update.");
            }

            var currentManufacturerId = currentVaccine.VaccineManufactures.FirstOrDefault()?.ManufacturerId;
            var newManufacturerId = matchingNewVaccine.VaccineManufactures.FirstOrDefault()?.ManufacturerId;

            if (currentManufacturerId != newManufacturerId)
            {
                hasChanges = true;
                updatedVaccines.Add(matchingNewVaccine);
            }
            else
            {
                updatedVaccines.Add(currentVaccine);
            }
        }

        // 5. Nếu không có thay đổi, trả về package hiện tại
        if (!hasChanges)
        {
            _logger.LogInformation("No manufacturer changes detected, returning existing package.");
            return _mapper.Map<PackageRes>(package);
        }

        // 6. Tính lại tổng giá trước khi áp dụng giảm giá
        decimal totalPrice = updatedVaccines.Sum(v => v.VaccineManufactures.FirstOrDefault()?.Price ?? 0);
        decimal discount = package.Discount ?? 0;
        decimal newTotalPrice = totalPrice * (1 - discount / 100);

        // 7. Kiểm tra package "Custom" đã tồn tại hay chưa
        var existingPackage = await _packageRepository.GetAllAsync("Vaccines");

        var existingCustomPackage = existingPackage.FirstOrDefault(p =>
            p.PackageName.Contains("Custom") &&
            p.Vaccines.OrderBy(v => v.VaccineId).Select(v => v.VaccineId).SequenceEqual(
                updatedVaccines.OrderBy(v => v.VaccineId).Select(v => v.VaccineId))
        );

        if (existingCustomPackage != null)
        {
            _logger.LogInformation("Custom package with same vaccines already exists. Returning existing package.");
            return _mapper.Map<PackageRes>(existingCustomPackage);
        }

        // 8. Tạo package mới nếu chưa có package như vậy
        var newPackage = new Package
        {
            PackageId = Guid.NewGuid(),
            PackageName = package.PackageName + " Custom",
            Vaccines = updatedVaccines,
            Price = newTotalPrice,
            Description = package.Description,
            Discount = package.Discount,
            MinAge = package.MinAge,
            MaxAge = package.MaxAge,
            Unit = package.Unit,
            IsActive = package.IsActive
        };

        await _packageRepository.InsertAsync(newPackage);
        await _unitOfWork.SaveChangeAsync();
        _unitOfWork.CommitTransaction();

        _logger.LogInformation("Created new custom package with ID: {PackageId}", newPackage.PackageId);
        return _mapper.Map<PackageRes>(newPackage);
    }
    catch (Exception ex)
    {
        _unitOfWork.RollBack();
        _logger.LogError(ex, "Error updating vaccines for package with ID: {PackageId}", packageId);
        throw;
    }
}
        
        public async Task DeletePackage(Guid packageId)
        {
            try
            {
                _logger.LogInformation("Deleting package with ID: {PackageId}", packageId);
                var package = await _packageRepository.GetByIdAsync(packageId);
                if (package == null) throw new KeyNotFoundException("Package not found");

                package.IsActive = false;
                await _packageRepository.UpdateAsync(package);
                await _unitOfWork.SaveChangeAsync();

                _logger.LogInformation("Package deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting package with ID: {PackageId}", packageId);
                throw;
            }
            
            
        }
    }
