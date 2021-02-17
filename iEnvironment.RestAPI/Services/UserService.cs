using iEnvironment.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iEnvironment.RestAPI.Services
{
    public class UserService : BaseService<User>
    {
        public UserService() : base("users")
        {
            
        }
    }
}
