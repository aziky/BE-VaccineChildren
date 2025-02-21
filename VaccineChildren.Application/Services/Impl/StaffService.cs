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
        private readonly IRsaService _rsaService;

        private readonly IGenericRepository<User> _userRepository;
        public StaffService(ILogger<IStaffService> logger, IUnitOfWork unitOfWork, IMapper mapper, IRsaService rsaService)
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
                    UserName = staffReq.Username,
                    Password = encryptedPassword,
                    FullName = staffReq.FullName,
                    Phone = staffReq.Phone,
                    Email = staffReq.Email,
                    Address = staffReq.Address,
                    CreatedAt = DateTime.UtcNow.ToLocalTime(),
                    RoleId = 2,  
                };
                
                await userRepository.InsertAsync(user);
                await _unitOfWork.SaveChangeAsync();
                
                var staff = new Staff
                {
                    StaffId = user.UserId,
                    Dob = staffReq.Dob,
                    Gender = staffReq.Gender,
                    BloodType = staffReq.BloodType,
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
        _logger.LogInformation("Updating staff with ID: {StaffId}", staffId);
        _unitOfWork.BeginTransaction();

        var staffRepository = _unitOfWork.GetRepository<Staff>();
        var staff = await staffRepository.GetByIdAsync(staffId);

        if (staff == null)
        {
            _logger.LogWarning("Staff not found with ID: {StaffId}", staffId);
            throw new KeyNotFoundException("Staff not found");
        }

        // Vì StaffId trùng với UserId, ta có thể dùng staffId để lấy thông tin User
        var userRepository = _unitOfWork.GetRepository<User>();
        var user = await userRepository.GetByIdAsync(staffId);

        if (user == null)
        {
            _logger.LogWarning("User associated with staff not found for ID: {StaffId}", staffId);
            throw new KeyNotFoundException("User associated with staff not found");
        }

        // Cập nhật thông tin Staff
        staff.Gender = staffReq.Gender;
        staff.Dob = staffReq.Dob;
        staff.BloodType = staffReq.BloodType;
        staff.UpdatedAt = DateTime.UtcNow.ToLocalTime();

        // Cập nhật thông tin User (bỏ qua password)
        user.FullName = staffReq.FullName;
        user.Phone = staffReq.Phone;
        user.Email = staffReq.Email;
        user.Address = staffReq.Address;
        user.UpdatedAt = DateTime.UtcNow.ToLocalTime();

        // Lưu thay đổi
        await staffRepository.UpdateAsync(staff);
        await userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangeAsync();

        // Commit transaction
        _unitOfWork.CommitTransaction();
        _logger.LogInformation("Staff updated successfully with ID: {StaffId}", staffId);
    }
    catch (Exception e)
    {
        _logger.LogError("Error while updating staff (ID: {StaffId}): {Error}", staffId, e.Message);
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
                var staff = await staffRepository.Entities
                    .Include(s => s.User) // Load thông tin User
                    .FirstOrDefaultAsync(s => s.StaffId == staffId);

                if (staff == null)
                {
                    _logger.LogInformation("Staff not found with ID: {StaffId}", staffId);
                    throw new KeyNotFoundException("Staff not found");
                }

                var staffRes = _mapper.Map<StaffRes>(staff);
        
                if (staff.User != null)
                {
                    staffRes.UserName = staff.User.UserName;
                    staffRes.FullName = staff.User.FullName;
                    staffRes.Email = staff.User.Email;
                    staffRes.Phone = staff.User.Phone;
                    staffRes.Address = staff.User.Address;
                }

                return staffRes;
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
                _logger.LogInformation("Retrieving all staff, including inactive staff");

                var staffRepository = _unitOfWork.GetRepository<Staff>();

                var staffList = await staffRepository.Entities
                    .Include(s => s.User) // Load thông tin User
                    .ThenInclude(u => u.Role) // Load thông tin Role
                    .Where(s => s.User.Role.RoleName.ToLower() == StaticEnum.RoleEnum.Staff.ToString().ToLower())
                    .OrderByDescending(s => s.Status == StaticEnum.StatusEnum.Active.ToString())
                    .ThenBy(s => s.Status)
                    .ToListAsync();

                if (!staffList.Any())
                {
                    _logger.LogInformation("No staff found");
                    return new List<StaffRes>();
                }

                var staffResList = _mapper.Map<List<StaffRes>>(staffList);
        
                // Gán thêm UserName
                staffResList.ForEach(staff => staff.UserName = staffList.First(s => s.StaffId == Guid.Parse(staff.StaffId)).User?.UserName);

                return staffResList;
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving staff list: {Error}", e.Message);
                throw;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }


    }
}
