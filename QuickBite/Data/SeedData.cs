using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuickBite.Data;
using QuickBite.Models;

namespace QuickBite.Data
{
    public static class SeedData
    {
        public enum Roles
        {
            SiteAdmin,
        }
        
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("SoftwareAdmin").Result)
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.SiteAdmin.ToString()));
            }
        }

        public static async Task SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByEmailAsync("siteadmin@example.com").Result == null)
            {
                const string siteAdminEmail = "siteadmin@example.com";
                const string siteAdminPassword = "Admin@123";

                ApplicationUser siteAdmin = new ApplicationUser
                {
                    Email = siteAdminEmail,
                    UserName = siteAdminPassword,
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(siteAdmin, siteAdminEmail);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(siteAdminEmail);
                    var r1 = await userManager.AddToRoleAsync(user, Roles.SiteAdmin.ToString());
                }

            }
        }
    }
}