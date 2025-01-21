using AutoMapper;
using Microsoft.Extensions.Logging;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Application.Services.Impl;

public class UserService : IUserService
{
    private readonly ILogger<IUserService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(ILogger<IUserService> logger, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task RegisterUser(UserReq userReq)
    {
        try
        {
            _logger.LogInformation("Start registering user");
            _unitOfWork.BeginTransaction();
            
            var userRepository = _unitOfWork.GetRepository<User>();
            await userRepository.InsertAsync(_mapper.Map<User>(userReq));
            await _unitOfWork.SaveChangeAsync();

            
            _unitOfWork.CommitTransaction();
            _logger.LogInformation("User register success");
        }
        catch (Exception e)
        {
            _logger.LogError("Error at register user: {}", e.Message);
            _unitOfWork.RollBack();
            throw;
        }
        finally
        {
            _unitOfWork.Dispose();
        }
    }

    public async Task<UserRes> Login(UserReq userReq)
    {
        try
        {
            var userRepository = _unitOfWork.GetRepository<User>();
            var request = _mapper.Map<User>(userReq);
            var user = await userRepository.FindByConditionAsync(u => u.UserName == request.UserName && u.Password == request.Password);
            if (user == null)
            {
                _logger.LogInformation("Login failed");
                throw new KeyNotFoundException("Invalid username or password");
            }

            return _mapper.Map<UserRes>(user);
        }
        catch (Exception e)
        {
            _logger.LogError("Error at login");
            throw;
        }
        finally
        {
            _unitOfWork.Dispose();
        }
    }
}