using Lab2Expense.Models;
using Lab2Expense.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Threading.Tasks;

namespace Lab2Expense.Services
{
    public interface IUsersService
    {
        UserGetModel Authenticate(string username, string password);
        UserGetModel Register(RegisterPostModel registerInfo);
        User GetCurrentUser(HttpContext httpContext);
        IEnumerable<UserGetModelWithRole> GetAll();
        User Delete(int id, User user);
        User Upsert(int id, User user, User userCurrent);


    }

    public class UsersService : IUsersService
    {
        private ExpensesDbContext context;
        private readonly AppSettings appSettings;

        public UsersService(ExpensesDbContext context, IOptions<AppSettings> appSettings)
        {
            this.context = context;
            this.appSettings = appSettings.Value;
        }

        public UserGetModel Authenticate(string username, string password)
        {
            var user = context.Users
                .SingleOrDefault(x => x.Username == username &&
                                 x.Password == ComputeSha256Hash(password));

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimTypes.Role, user.UserRole.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var result = new UserGetModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Token = tokenHandler.WriteToken(token)
            };
            // remove password before returning

            return result;
        }

        private string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            // TODO: also use salt
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public UserGetModel Register(RegisterPostModel registerInfo)
        {
            User existing = context.Users.FirstOrDefault(u => u.Username == registerInfo.Username);
            if (existing != null)
            {
                return null;
            }

            context.Users.Add(new User
            {
                Email = registerInfo.Email,
                LastName = registerInfo.LastName,
                FirstName = registerInfo.FirstName,
                Password = ComputeSha256Hash(registerInfo.Password),
                Username = registerInfo.Username,
                UserRole = UserRole.Regular,
                isRemoved = false,
                DateAdded = DateTime.Now


            });
            context.SaveChanges();
            return Authenticate(registerInfo.Username, registerInfo.Password);
        }


        public User GetCurrentUser(HttpContext httpContext)
        {
            string username = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            //string accountType = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.AuthenticationMethod).Value;
            //return _context.Users.FirstOrDefault(u => u.Username == username && u.AccountType.ToString() == accountType);
            return context.Users.FirstOrDefault(u => u.Username == username);
        }

        public IEnumerable<UserGetModelWithRole> GetAll()
        {
            // return users without passwords
            return context.Users.Select(user => new UserGetModelWithRole
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                UserRole = user.UserRole
            });
        }
        //public UserGetModelWithRole ChangeRole(int id, string Role)
        //{
        //    var user = context.Users.Find(id);
        //    if (Role == "admin")
        //    {
        //        user.UserRole = Models.UserRole.Admin;
        //    }
        //    if (Role == "manager")
        //    {
        //        user.UserRole = Models.UserRole.UserManager;
        //    }

        //    return UserGetModelWithRole.FromUser(user);

        //}
        public User Delete(int id, User userCurrent)
        {



            var existing = context.Users
            .FirstOrDefault(user => user.Id == id);
            if (existing == null)
            {
                return null;
            }

            DateTime dateCurrent = DateTime.Now;
            TimeSpan diferenta = dateCurrent.Subtract(userCurrent.DateAdded);
            if ((userCurrent.UserRole == Models.UserRole.Admin || diferenta.Days > 190) && existing.UserRole != Models.UserRole.Admin)
            {

                existing.isRemoved = true;
            }
            if (existing.UserRole == UserRole.Admin || existing.UserRole == UserRole.UserManager)
            {
                existing.isRemoved = false;
            }

            context.Update(existing);
            context.SaveChanges();
            return existing;
        }

        public User Upsert(int id, User user, User userCurrent)
        {
            var existing = context.Users.AsNoTracking().FirstOrDefault(f => f.Id == id);
            if (existing == null)
            {
                user.Password = ComputeSha256Hash(user.Password);
                context.
                    Users.Add(user);
                context.SaveChanges();
                return user;
            }
            DateTime dateCurrent = DateTime.Now;
            TimeSpan diferenta = dateCurrent.Subtract(userCurrent.DateAdded);

            user.Id = id;
            if ((userCurrent.UserRole == Models.UserRole.Admin || diferenta.Days > 190) && existing.UserRole != Models.UserRole.Admin)
            {
                user.Password = ComputeSha256Hash(user.Password);
                context.Users.Update(user);
                context.SaveChanges();
                return user;
            }
            user.UserRole = existing.UserRole;
            user.Password = ComputeSha256Hash(user.Password);
            context.Users.Update(user);
            context.SaveChanges();
            return user;

        }




    }

}

