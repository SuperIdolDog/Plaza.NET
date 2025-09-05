using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model
{
    public interface IBaseEntity
    {
       int Id { get; set; }

        string? Code { get; set; } 

        bool IsDeleted { get; set; } 

    }
}
