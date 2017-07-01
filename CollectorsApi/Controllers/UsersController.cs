using CollectorsApi.Models;
using CollectorsApi.Models.BindingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace CollectorsApi.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : BaseApiController
    {
        // GET: Users
        public IHttpActionResult GetUsers(string Id)
        { 
            var users = this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u));
            var userroles = new List<UserReturnModel>();

            foreach (var user in users)
            {
                foreach (var role in user.Roles)
                    if (role == Id)
                        userroles.Add(user);
            }

            return Ok(userroles);
        }

        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await this.AppUserManager.FindByIdAsync(Id);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }

        [Route("getUserByEmail")]
        [HttpPost]
        public async Task<IHttpActionResult> GetUserByEmail(CreateUserBindingModel getUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await this.AppUserManager.FindByEmailAsync(getUserModel.Email);

            if (user != null)
            {
                var check = await AppUserManager.FindAsync(user.UserName, getUserModel.Password);
                if (check != null)
                {
                    return Ok(this.TheModelFactory.Create(user));
                }
                return BadRequest("Incorrect e-mail or password! ");
            }

            return NotFound();

        }

        [Route("create")]
        public async Task<IHttpActionResult> CreateUser(CreateUserBindingModel createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User()
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email,
                FirstName = createUserModel.FirstName,
                LastName = createUserModel.LastName
            };

            var addUserResult = await this.AppUserManager.CreateAsync(user, createUserModel.Password);

            var x = Task.Run(async () =>
            {
                return await this.AppUserManager.FindByNameAsync(user.UserName);
            });

            if (createUserModel.RoleName == "Teacher")
            {
                await AppUserManager.AddToRoleAsync(x.Result.Id, "Teacher");
            }

            if (createUserModel.RoleName == "Student")
            {
                await AppUserManager.AddToRoleAsync(x.Result.Id, "Student");
            }

            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            }

            var locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            return Created(locationHeader, TheModelFactory.Create(user));
        }
    }
}