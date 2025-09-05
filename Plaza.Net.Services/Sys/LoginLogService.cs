using Microsoft.AspNetCore.Http;
using Plaza.Net.IRepository;
using Plaza.Net.IRepository.Sys;
using Plaza.Net.IServices.Sys;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Services.Sys
{
    public class LoginLogService : BaseServices<LoginLogEntity>, ILoginLogService
    {

        private readonly ILoginLogRepository _loginLogRepository;
        public LoginLogService(
            IBaseRepository<LoginLogEntity> repository,
            ILoginLogRepository loginLogRepository) : base(repository)
        {

            _loginLogRepository = loginLogRepository;
        }

        public async Task LogFailedLoginAsync(int userId, string ipAddress, string deviceInfo, string failureReason, string code = null)
        {
            await _loginLogRepository.LogFailedLoginAsync(
                userId,
                ipAddress,
                deviceInfo,
                failureReason,
                code);
        }

        public async Task LogLogoutAsync(int userId)
        {
            await _loginLogRepository.LogLogoutAsync(userId);
        }

        public async Task LogSuccessfulLoginAsync(int userId, string ipAddress, string deviceInfo, string code = null)
        {
           

            await _loginLogRepository.LogSuccessfulLoginAsync(
                userId,
                ipAddress,
                deviceInfo,
                code);
        }
    }
}
