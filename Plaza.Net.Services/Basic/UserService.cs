
using Plaza.Net.IRepository;
using Plaza.Net.IRepository.Basic;
using Plaza.Net.IRepository.User;
using Plaza.Net.IServices.User;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Repository.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Services.Basic
{
    public class UserService : BaseServices<UserEntity>, IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository) : base(repository)
        {
            _repository = repository;
        }
        public async Task<bool> UpdateUserAsync(UserEntity user)
        {
            return await _repository.UpdateAsync(user);
        }
    }
}
