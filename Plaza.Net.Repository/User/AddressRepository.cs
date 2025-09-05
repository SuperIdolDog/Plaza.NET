using Plaza.Net.IRepository.User;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Repository.User
{
    internal class AddressRepository : BaseRepository<AddressEntity>, IAddressRepository
    {
        public AddressRepository(EFDbContext context) : base(context)
        {
        }
    }
}
