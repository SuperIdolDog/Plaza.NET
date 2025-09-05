using Plaza.Net.IRepository;
using Plaza.Net.IServices.User;
using Plaza.Net.Model.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Services.User
{
    public class UserFeedbackService : BaseServices<UserFeedbackEntity>, IUserFeedbackService
    {
        public UserFeedbackService(IBaseRepository<UserFeedbackEntity> repository) : base(repository)
        {
        }
    }
}
