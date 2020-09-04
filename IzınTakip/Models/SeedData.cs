using IzinTakip.DataAccess.Concrete.EntityFramework;
using IzinTakip.Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IzinTakip.UI.Models
{
    public static class SeedData
    {
        public static void CreateRolesAndAdminUser(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<IzinTakipContext>();
            const string adminRoleName = "Administrator";
            string[] roleNames = { adminRoleName, "Manager", "Subscriber" };

            foreach (string roleName in roleNames)
            {
                CreateRole(serviceProvider, roleName);
            }

            // Get these value from "appsettings.json" file.
            string adminUserEmail = "ismailardic@mkm.com.tr";
            string adminPwd = "Admin?123";
            string userName = "Admin";
            string userId = "bc68af64-5675-4a5b-b6b2-92b2fd282cbf";

            AddUser(serviceProvider, adminUserEmail, userName, userId, adminPwd, adminRoleName);
        }

        /// <summary>
        /// Create a role if not exists.
        /// </summary>
        /// <param name="serviceProvider">Service Provider</param>
        /// <param name="roleName">Role Name</param>
        private static void CreateRole(IServiceProvider serviceProvider, string roleName)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

            Task<bool> roleExists = roleManager.RoleExistsAsync(roleName);
            roleExists.Wait();

            if (!roleExists.Result)
            {
                Role role = new Role
                {
                    Name = roleName
                };

                Task<IdentityResult> roleResult = roleManager.CreateAsync(role);
                roleResult.Wait();
            }
        }

        private static void AddUser(IServiceProvider serviceProvider, string userEmail, string userName, string userId, string userPwd, string roleName)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            Task<User> checkAppUser = userManager.FindByEmailAsync(userEmail);
            checkAppUser.Wait();

            User appUser = checkAppUser.Result;

            if (checkAppUser.Result == null)
            {
                User newAppUser = new User
                {
                    Id = userId,
                    Email = userEmail,
                    UserName = userName,
                    Biography = "Yıllık izin takip sistemi yönetim sorumlusu ",
                    Image = "admin-image.jpg",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                Task<IdentityResult> taskCreateAppUser = userManager.CreateAsync(newAppUser, userPwd);
                taskCreateAppUser.Wait();

                if (taskCreateAppUser.Result.Succeeded)
                {
                    appUser = newAppUser;
                }
            }

            Task<IdentityResult> newUserRole = userManager.AddToRoleAsync(appUser, roleName);
            newUserRole.Wait();
        }
    }
}
