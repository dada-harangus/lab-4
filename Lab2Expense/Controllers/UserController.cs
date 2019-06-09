using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Lab2Expense.Models;
using Lab2Expense.Services;
using Lab2Expense.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab2Expense.Controllers
{
    // https://jasonwatmore.com/post/2018/08/14/aspnet-core-21-jwt-authentication-tutorial-with-example-api
    [Authorize(Roles = "Admin, UserManager")]
    //[Route("api/[controller]/[action]")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUsersService _userService;

        public UsersController(IUsersService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]LoginPostModel login)
        {
            var user = _userService.Authenticate(login.Username, login.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        //[HttpPost]
        public IActionResult Register([FromBody]RegisterPostModel registerModel)
        {
            var user = _userService.Register(registerModel);
            if (user == null)
            {
                return BadRequest(new { ErrorMessage = "Username already exists." });
            }
            return Ok(user);
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        // PUT: api/Users/5
        /// <summary>
        /// Update the User with the given id
        /// </summary>
        ///  /// <remarks>
        /// Sample request:
        /// PUT/users
        /// {
        ///   "first": "cheeseburger2",
        ///   "sum": 10,
        /// "location": "fastfood",
        ///"date": "2019-05-09T12:30:00",
        /// "currency": "EUR",
        /// "expenseType": 5
        /// }
        /// </remarks>
        /// <param name="id">The id of the expense we want to update</param>
        /// <param name="user">The User that contains the new data</param>
        /// <returns>An User object</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Put(int id, [FromBody] User user)
        {

            var userCurrent = _userService.GetCurrentUser(HttpContext);
            var result = _userService.Upsert(id, user, userCurrent);
            return Ok(result);
        }

        // DELETE: api/ApiWithActions/5
        /// <summary>
        /// Delete the User with the given id
        /// </summary>
        /// <param name="id">The id of the user we want to delete</param>
        /// <returns>an User object</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            var userCurrent = _userService.GetCurrentUser(HttpContext);
            var existing = _userService.Delete(id, userCurrent);
            if (existing == null)
            {
                return NotFound();
            }

            return Ok(existing);
        }
        //[HttpPut]
        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public IActionResult ChangeRole([FromQuery]int id, [FromBody] string Role)
        //{
        //    if (User.IsInRole("UserManager"))
        //    {
        //        return NoContent();
        //    }
        //    var existing = _userService.ChangeRole(id, Role);
        //    if (existing == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(existing);

        //}

    }
}