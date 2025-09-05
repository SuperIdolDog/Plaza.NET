using Microsoft.EntityFrameworkCore;
using Plaza.Net.IRepository;
using Plaza.Net.IRepository.Sys;
using Plaza.Net.IServices.Sys;
using Plaza.Net.IServices.User;
using Plaza.Net.Model.Entities.Sys;
using Plaza.Net.Model.Entities.User;
using Plaza.Net.Model.ViewModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Services.Sys
{
    public class SysMenuService : BaseServices<SysMenuEntity>, ISysMenuService
    {
        private readonly ISysMenuRepository _repository;
        public SysMenuService(ISysMenuRepository repository) : base(repository)
        {
            _repository = repository;

        }


        public async Task<List<SysMenuEntity>> GetAllMenusByRoleIdAsync(int roleId)
        {
            return await _repository.GetAllMenusByRoleIdAsync(roleId);
        }
    }
}
