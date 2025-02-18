using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;

public interface IPackageService
{
    Task CreatePackage(PackageReq packageReq);
    Task<PackageRes?> GetPackageById(Guid packageId);
    Task<List<PackageRes>> GetAllPackages();
    Task UpdatePackage(Guid packageId, PackageReq packageReq);
    Task DeletePackage(Guid packageId);
}
