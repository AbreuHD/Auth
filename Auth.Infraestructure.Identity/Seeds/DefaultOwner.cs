﻿using Auth.Core.Application.Enums;
using Auth.Infraestructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Auth.Infraestructure.Identity.Seeds
{
    public static class DefaultOwner
    {
        public static async Task Seed(UserManager<ApplicationUser> userManager)
        {
            var user = new ApplicationUser
            {
                Name = "Jefferson",
                LastName = "Abreu",
                UserName = "AbreuHD",
                Email = "AbrueMartinezJefferson@gmail.com",
                PhoneNumber = "809-491-0570",
                EmailConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != user.Id))
            {
                var userEmail = await userManager.FindByEmailAsync(user.Email);
                if (userEmail == null)
                {
                    await userManager.CreateAsync(user, "123Pa$$word!");
                    await userManager.AddToRoleAsync(user, Roles.Owner.ToString());
                }
            }
        }
    }
}
