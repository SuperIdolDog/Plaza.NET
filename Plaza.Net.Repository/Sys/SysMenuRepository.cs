using Microsoft.EntityFrameworkCore;
using Plaza.Net.IRepository.Sys;
using Plaza.Net.IRepository.User;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Sys;
using Plaza.Net.Model.Entities.User;
using Plaza.Net.Model.ViewModels;
using Plaza.Net.Model.ViewModels.DTO;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Repository.Sys
{
    public class SysMenuRepository : BaseRepository<SysMenuEntity>, ISysMenuRepository
    {
        private readonly EFDbContext _dbContext;
        public SysMenuRepository(EFDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task<List<SysMenuEntity>> GetAllMenusByRoleIdAsync(int roleId)
        {
            // 获取角色下的所有权限
            var permissions = await _dbContext.Permission
                .Where(p => p.RoleId == roleId)
                .Include(p => p.MenuPermissions)
                .ToListAsync();

            // 获取这些权限对应的所有菜单ID
            var menuIds = permissions
                .SelectMany(p => p.MenuPermissions.Select(mp => mp.MenuId))
                .Distinct();

            // 如果没有找到菜单ID，返回空列表
            if (!menuIds.Any())
            {
                return new List<SysMenuEntity>();
            }

            // 获取这些菜单ID对应的所有菜单
            var menus = await _dbContext.SysMenu
                .Where(m => menuIds.Contains(m.Id))
                .ToListAsync();

            // 构建菜单树
            var menuDictionary = menus.ToDictionary(m => m.Id);
            var rootMenus = new List<SysMenuEntity>();

            foreach (var menu in menus)
            {
                if (menu.ParentId.HasValue && menu.ParentId != 0)
                {
                    if (menuDictionary.TryGetValue(menu.ParentId.Value, out var parentMenu))
                    {
                        parentMenu.Children.Add(menu);
                    }
                }
                else
                {
                    rootMenus.Add(menu);
                }
            }

            return rootMenus;
        }

    }
}

