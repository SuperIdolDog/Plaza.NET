using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.IRepository.Sys
{
    public interface ILoginLogRepository:IBaseRepository<LoginLogEntity>
    {
        // 记录成功登录
        Task LogSuccessfulLoginAsync(int userId, string ipAddress, string deviceInfo, string code = null);

        // 记录登录失败
        Task LogFailedLoginAsync(int userId, string ipAddress, string deviceInfo, string failureReason, string code = null);

        // 记录退出登录
        Task LogLogoutAsync(int userId);
    }
}
