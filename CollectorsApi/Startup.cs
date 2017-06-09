using CollectorsApi.Managers;
using CollectorsApi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Newtonsoft.Json.Serialization;
using Owin;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

[assembly: OwinStartup(typeof(CollectorsApi.Startup))]

namespace CollectorsApi
{
    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();

            ConfigureOAuthTokenGeneration(app);

            ConfigureWebApi(httpConfig);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseWebApi(httpConfig);

            CreateRoles();

        }

        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            app.CreatePerOwinContext(PatternsContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
        }

        private void ConfigureWebApi(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        //http://www.c-sharpcorner.com/UploadFile/asmabegam/Asp-Net-mvc-5-security-and-creating-user-role/ took what I needed
        private void CreateRoles()
        {
            var context = new PatternsContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<User>(new UserStore<User>(context));
    
            if (!roleManager.RoleExists("Admin"))
            {  
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                var user = new User()
                {
                    UserName = "Chupachups",
                    Email = "catalina.ionita@gmail.com"
                };

                string userPWD = "Password1!";

                var chkUser = UserManager.Create(user, userPWD);

                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");

                }
            }
 
            if (!roleManager.RoleExists("Teacher"))
            {
                var role = new IdentityRole()
                {
                    Name = "Teacher"
                };

                roleManager.Create(role);
            }
  
            if (!roleManager.RoleExists("Student"))
            {
                var role = new IdentityRole();
                role.Name = "Student";
                roleManager.Create(role);

            }
        }
    }
}