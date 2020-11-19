using System;
using System.Threading.Tasks;
using SayWhat.Bll.Services;
using SayWhat.MongoDAL.Users;

namespace Chotiskazal.Bot.Services
{
    public class AuthorizeService
    {
        private readonly UserService _userService;

        public AuthorizeService(UserService userService)=> _userService = userService;

        public async Task<User> AuthorizeAsync(long telegramId,string name)
        {
            var user = await LoginUserAsync(telegramId) ?? await CreateUserAsync(telegramId,name);
            if(user==null)
                throw  new Exception("I can't add user!");
            return user;
        }
        
        private async Task<User> CreateUserAsync(long telegramID, string name)
        {
            try
            {
                var user = new User(telegramID, name);
                await _userService.AddUserAsync(user);
                return user;
            }
            catch
            {
                return null;
            }
        }
        
        private async Task<User> LoginUserAsync(long telegramId)=>
            await  _userService.GetUserByTelegramIdAsync(telegramId);
    }
}