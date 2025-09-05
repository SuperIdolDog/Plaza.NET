using Plaza.Net.Model.Entities.Sys;
using Plaza.Net.Model.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<SysMenuEntity> MenuItems { get; set; } = null!;
        public IEnumerable<NotificationEntity> Notifications { get; set; } = null!;
        public int NotificationCount { get; set; }
        public string? Avatar { get; set; }
    }
}
