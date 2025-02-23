using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Domain.Entities;

public interface IPackageService
{
    Task CreatePackage(PackageReq packageReq);
    Task<PackageRes?> GetPackageById(Guid packageId);
    Task<List<PackageRes>> GetAllPackages();
    Task UpdatePackage(Guid packageId, PackageReq packageReq);
    Task DeletePackage(Guid packageId);
    Task<PackageRes> UpdatePackageVaccines(Guid packageId, UpdatePackageVaccinesReq request);
}
