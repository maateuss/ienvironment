using iEnvironment.Domain.Models;
using MongoDB.Driver;
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

        public async Task<bool> CreateNew(User user)
        {
            if (!String.IsNullOrWhiteSpace(user.Id))
            {
                var duplicate = Collection.Find(x => x.Id == user.Id).Any();
                if (duplicate)
                {
                    return false;
                }
            }

            var loginExists = Collection.Find(x => x.Login == user.Login).Any();

            if (loginExists)
            {
                return false;
            }

            if (!User.ValidateNewUser(user))
            {
                return false;
            }

            user.Password = CryptoService.HashPassword(user.Password);

            Collection.InsertOne(user);

            return true;
        }

        public async Task<User> GetByLogin(string login)
        {
            return await Collection.Find(x => x.Login == login).FirstOrDefaultAsync();
        }

        public async Task<bool> EditUser(string id, User user)
        {
            var currentUser = await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
            
            if (currentUser == null)
            {
                return false;
            }
            
            var validUser = User.ValidateUserUpdate(user);
            
            if (validUser == null)
            {
                return false;
            }
            
            await Collection.FindOneAndReplaceAsync(x => x.Id == id, validUser);
            return true;
        }


        public async Task<User> Authenticate(LoginAttempt attempt)
        {
            var user = await GetByLogin(attempt.Login);
            if(user == null)
            {
                return null;
            }

            var valid = CryptoService.ValidatePassword(attempt.Password, user.Password);
            
            if (valid) 
            {
                return user; 
            }

            return null;

        }

    }
}
