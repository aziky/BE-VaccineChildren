
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;

namespace VaccineChildren.Application.Services;

public interface IStaffService
{
    Task CreateStaff(StaffReq staffReq);
    Task DeleteStaff(Guid staffId);
    Task UpdateStaff(Guid staffId, StaffReq staffReq);
    Task<List<StaffRes>> GetAllStaff();
    Task<StaffRes> GetStaffById(Guid staffId);
}