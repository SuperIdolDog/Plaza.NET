using Plaza.Net.IRepository;
using Plaza.Net.IRepository.Basic;
using Plaza.Net.IRepository.User;
using Plaza.Net.IServices.Basic;
using Plaza.Net.Model.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Services.Basic
{
    public class UserRoleService : BaseServices<UserRoleEntity>, IUserRoleService
    {

        private readonly IUserRoleRepository _repository;
        public UserRoleService(IUserRoleRepository repository) : base(repository)
        {
            _repository = repository;
        }
    }
}
