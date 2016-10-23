using SoLoud.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace SoLoud.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<SoLoudContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
            MigrationsDirectory = @"Migrations";
        }

        protected override void Seed(SoLoudContext context)
        {
            base.Seed(context);

            //Add Company Role
            var CompanyRoleExists = context.Roles.Any(x => x.Name == "Company");
            if (!CompanyRoleExists)
                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("Company"));

            //Add User Role
            var UserRoleExists = context.Roles.Any(x => x.Name == "User");
            if (!UserRoleExists)
                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("User"));
        }
    }
}