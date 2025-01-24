using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Application.Services.Impl
{
    public class StaffService : IStaffService
    {
        private readonly ILogger<IStaffService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StaffService(ILogger<IStaffService> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateStaff(StaffReq staffReq)
        {
            try
            {
                _logger.LogInformation("Start creating staff");

                // Begin the transaction
                _unitOfWork.BeginTransaction();

                // Create a new User for the staff
                var user = new User
                {
                    UserId = Guid.NewGuid(),
                    UserName = staffReq.Email,
                    Password = staffReq.Password,
                    FullName = staffReq.FullName,
                    Phone = staffReq.Phone,
                    Email = staffReq.Email,
                    Address = staffReq.Address,
                    CreatedAt = DateTime.UtcNow.ToLocalTime(),
                };

                // Insert the user into the database
                var userRepository = _unitOfWork.GetRepository<User>();
                await userRepository.InsertAsync(user);
                await _unitOfWork.SaveChangeAsync();

                // Create the staff linked to the user
                var staff = new Staff
                {
                    StaffId = Guid.NewGuid(),
                    UserId = user.UserId,
                    RoleId = staffReq.Role == "Staff" ? 1 : 2, // Assuming 1 is Admin and 2 is Staff (change this according to your role logic)
                    Status = true,
                    CreatedAt = DateTime.UtcNow.ToLocalTime(),

                };

                // Insert the staff into the database
                var staffRepository = _unitOfWork.GetRepository<Staff>();
                await staffRepository.InsertAsync(staff);
                await _unitOfWork.SaveChangeAsync();

                // Commit the transaction
                _unitOfWork.CommitTransaction();
                _logger.LogInformation("Staff created successfully");
            }
            catch (Exception e)
            {
                _logger.LogError("Error while creating staff: {Error}", e.Message);
                _unitOfWork.RollBack();
                throw;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public async Task DeleteStaff(Guid staffId)
        {
            try
            {
                _logger.LogInformation("Start deleting staff with ID: {StaffId}", staffId);
                _unitOfWork.BeginTransaction();

                var staffRepository = _unitOfWork.GetRepository<Staff>();
                var staff = await staffRepository.GetByIdAsync(staffId);

                if (staff == null)
                {
                    _logger.LogInformation("Staff not found with ID: {StaffId}", staffId);
                    throw new KeyNotFoundException("Staff not found");
                }

                staff.Status = false; // Soft delete by changing status
                await staffRepository.UpdateAsync(staff);
                await _unitOfWork.SaveChangeAsync();

                _unitOfWork.CommitTransaction();
                _logger.LogInformation("Staff deleted successfully");
            }
            catch (Exception e)
            {
                _logger.LogError("Error while deleting staff: {Error}", e.Message);
                _unitOfWork.RollBack();
                throw;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public async Task UpdateStaff(Guid staffId, StaffReq staffReq)
        {
            try
            {
                _logger.LogInformation("Start updating staff with ID: {StaffId}", staffId);
                _unitOfWork.BeginTransaction();

                var staffRepository = _unitOfWork.GetRepository<Staff>();
                var staff = await staffRepository.GetByIdAsync(staffId);

                if (staff == null)
                {
                    _logger.LogInformation("Staff not found with ID: {StaffId}", staffId);
                    throw new KeyNotFoundException("Staff not found");
                }

                // Update Staff entity
                staff.Status = true; // You may adjust status update logic as needed
                staff.UpdatedAt = DateTime.UtcNow.ToLocalTime();

                var userRepository = _unitOfWork.GetRepository<User>();
                var user = await userRepository.GetByIdAsync(staff.UserId);

                if (user != null)
                {
                    // Update User info
                    user.FullName = staffReq.FullName;
                    user.Phone = staffReq.Phone;
                    user.Email = staffReq.Email;
                    user.Address = staffReq.Address;
                    user.UpdatedAt = DateTime.UtcNow.ToLocalTime();


                    await userRepository.UpdateAsync(user);
                }

                await staffRepository.UpdateAsync(staff);
                await _unitOfWork.SaveChangeAsync();

                _unitOfWork.CommitTransaction();
                _logger.LogInformation("Staff updated successfully");
            }
            catch (Exception e)
            {
                _logger.LogError("Error while updating staff: {Error}", e.Message);
                _unitOfWork.RollBack();
                throw;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public async Task<StaffRes> GetStaffById(Guid staffId)
        {
            try
            {
                _logger.LogInformation("Retrieving staff with ID: {StaffId}", staffId);

                var staffRepository = _unitOfWork.GetRepository<Staff>();
                var staff = await staffRepository.GetByIdAsync(staffId);

                if (staff == null)
                {
                    _logger.LogInformation("Staff not found with ID: {StaffId}", staffId);
                    throw new KeyNotFoundException("Staff not found");
                }

                return _mapper.Map<StaffRes>(staff);
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving staff: {Error}", e.Message);
                throw;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public async Task<List<StaffRes>> GetAllStaff()
        {
            try
            {
                _logger.LogInformation("Retrieving all staff");

                var staffRepository = _unitOfWork.GetRepository<Staff>();

                // Eager loading để tải đầy đủ thông tin User và Role
                var staffList = await staffRepository.GetAllAsync(include =>
                    include.Where(s => s.Status == true)
                           .Include(s => s.User)  // Eager load User
                           .Include(s => s.Role)); // Eager load Role

                // Ánh xạ từ Staff sang StaffRes, sử dụng AutoMapper
                return _mapper.Map<List<StaffRes>>(staffList);
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving staff: {Error}", e.Message);
                throw;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

    }
}
