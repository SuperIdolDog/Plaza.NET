using Plaza.Net.IRepository.Sys;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Repository.Sys
{
    internal class LoginLogRepository : BaseRepository<LoginLogEntity>, ILoginLogRepository
    {
        private readonly EFDbContext _dbContext;
        public LoginLogRepository(EFDbContext context) : base(context)
        {
            _dbContext = context;
        }
        public async Task LogSuccessfulLoginAsync(int userId, string ipAddress, string deviceInfo, string code)
        {
            var loginLog = new LoginLogEntity
            {
                LoginTime = DateTime.Now,
                IPAddress = ipAddress,
                DeviceInfo = deviceInfo,
                Status = 1, // 成功状态
                UserId = userId,
                Code = code,
                IsDeleted = false,
                UpdateTime = DateTime.Now
            };

            await CreateAsync(loginLog);
        }

        public async Task LogFailedLoginAsync(int userId, string ipAddress, string deviceInfo, string failureReason, string code)
        {
            var loginLog = new LoginLogEntity
            {
                LoginTime = DateTime.Now,
                IPAddress = ipAddress,
                DeviceInfo = deviceInfo,
                Status = 0, // 失败状态
                FailureReason = failureReason,
                UserId = userId,
                Code = code,
                IsDeleted = false,
                UpdateTime = DateTime.Now
            };
            await CreateAsync(loginLog);

        }

        public async Task LogLogoutAsync(int userId)
        {
            var loginLog = await _dbContext.LoginLog.FindAsync(userId);
            if (loginLog != null && !loginLog.LogoutTime.HasValue)
            {
                loginLog.LogoutTime = DateTime.Now;
                loginLog.UpdateTime = DateTime.Now;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
