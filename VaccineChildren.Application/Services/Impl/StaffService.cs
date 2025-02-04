using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;
using VaccineChildren.Core.Store;



namespace VaccineChildren.Application.Services.Impl
{
    public class StaffService : IStaffService
    {
        private readonly ILogger<IStaffService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly RsaService _rsaService;

        private readonly IGenericRepository<User> _userRepository;
        public StaffService(ILogger<IStaffService> logger, IUnitOfWork unitOfWork, IMapper mapper, RsaService rsaService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _rsaService = rsaService; 
            _userRepository = _unitOfWork.GetRepository<User>();
        }


        public async Task CreateStaff(StaffReq staffReq)
        {
            try
            {
                _logger.LogInformation("Start creating staff");

                // Bắt đầu giao dịch
                _unitOfWork.BeginTransaction();

                var userRepository = _unitOfWork.GetRepository<User>();
                var existingEmail = await userRepository.FindByConditionAsync(u => u.Email == staffReq.Email);
                var existingPhone = await userRepository.FindByConditionAsync(u => u.Phone == staffReq.Phone);
                if (existingEmail != null)
                {
                    throw new InvalidOperationException("Email already exists.");
                }
                if (existingPhone != null)
                {
                    throw new InvalidOperationException("Phone number already exists.");
                }
                string encryptedPassword = _rsaService.Encrypt(staffReq.Password);
                // Tạo mới User
                var user = new User
                {
                    UserId = Guid.NewGuid(),
                    UserName = staffReq.Email,
                    Password = encryptedPassword,
                    FullName = staffReq.FullName,
                    Phone = staffReq.Phone,
                    Email = staffReq.Email,
                    Address = staffReq.Address,
                    CreatedAt = DateTime.UtcNow.ToLocalTime(),
                };

                // Chèn User vào cơ sở dữ liệu
                await userRepository.InsertAsync(user);
                await _unitOfWork.SaveChangeAsync();

                // Tạo mới Staff
                var staff = new Staff
                {
                    StaffId = Guid.NewGuid(),
                    UserId = user.UserId,
                    RoleId = staffReq.Role == "staff" ? 8 : 0,  // Giả sử "Staff" có RoleId là 1, còn lại là 0
                    Status = StaticEnum.StatusEnum.Active.ToString(),
                    CreatedAt = DateTime.UtcNow.ToLocalTime(),
                };

                // Chèn Staff vào cơ sở dữ liệu
                var staffRepository = _unitOfWork.GetRepository<Staff>();
                await staffRepository.InsertAsync(staff);
                await _unitOfWork.SaveChangeAsync();

                // Cam kết giao dịch
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
                staff.Status = StaticEnum.StatusEnum.Inactive.ToString();
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

                // Cập nhật thông tin Staff
                staff.Status = StaticEnum.StatusEnum.Active.ToString();
                staff.UpdatedAt = DateTime.UtcNow.ToLocalTime();

                var userRepository = _unitOfWork.GetRepository<User>();
                var user = await userRepository.GetByIdAsync(staff.UserId);
                var existingEmail = await userRepository.FindByConditionAsync(u => u.Email == staffReq.Email);
                var existingPhone = await userRepository.FindByConditionAsync(u => u.Phone == staffReq.Phone);
                if (existingEmail != null)
                {
                    throw new InvalidOperationException("Email already exists.");
                }
                if (existingPhone != null)
                {
                    throw new InvalidOperationException("Phone number already exists.");
                }

                // Cập nhật thông tin User
                if (user != null)
                {
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
                _logger.LogInformation("Đang lấy danh sách tất cả nhân viên (bao gồm cả nhân viên đã bị xóa)");

                var staffRepository = _unitOfWork.GetRepository<Staff>();

                // Tạo truy vấn cơ bản cho tất cả nhân viên và bao gồm các thực thể liên quan (User, Role)
                var query = staffRepository.Entities  // Sử dụng thuộc tính Entities (IQueryable<Staff>)
                    .Include(s => s.User)  // Tải dữ liệu liên quan đến User
                    .Include(s => s.Role)  // Tải dữ liệu liên quan đến Role
                    .OrderByDescending(s => s.Status == StaticEnum.StatusEnum.Active.ToString())  // Sắp xếp theo trạng thái (Active lên trên)
                    .ThenBy(s => s.Status); // Sắp xếp các nhân viên `Inactive` xuống dưới

                // Thực thi truy vấn và lấy kết quả
                var staffList = await query.ToListAsync();

                if (staffList == null || !staffList.Any())
                {
                    _logger.LogInformation("Không tìm thấy nhân viên nào");
                    return new List<StaffRes>();
                }

                // Chuyển đổi các thực thể Staff thành các DTO phản hồi
                return _mapper.Map<List<StaffRes>>(staffList);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi khi lấy danh sách nhân viên: {Error}", e.Message);
                throw;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }


    }
}
