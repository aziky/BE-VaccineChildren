using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;

namespace VaccineChildren.Application.Services
{
    public interface IPackageService
    {
        Task<List<PackageRes>> GetAllPackages();
        Task<PackageRes?> GetPackageById(Guid packageId);
        Task<PackageRes> CreatePackage(PackageReq packageReq);
        Task<PackageRes?> UpdatePackage(Guid packageId, PackageReq packageReq);
        Task<bool> DeletePackage(Guid packageId);
    }
}
