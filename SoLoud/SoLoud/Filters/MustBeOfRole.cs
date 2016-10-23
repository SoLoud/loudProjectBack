using SoLoud.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoLoud.Filters
{
    public enum RoleType
    {
        User, Company
    }
    public class MustBe : ValidationAttribute
    {
        private string _userRoleID;
        private string _companyRoleID;
        private string userRoleID
        {
            get
            {

                if (this._userRoleID != null)
                    return this._userRoleID;
                else
                {
                    var context = new SoLoudContext();
                    this._userRoleID = context.Roles.FirstOrDefault(x => x.Name == "User").Id;

                    return this._userRoleID;
                }
            }
        }
        private string companyRoleID
        {
            get
            {

                if (this._companyRoleID != null)
                    return this._companyRoleID;
                else
                {
                    var context = new SoLoudContext();
                    this._companyRoleID = context.Roles.FirstOrDefault(x => x.Name == "Company").Id;

                    return this._companyRoleID;
                }
            }
        }
        public RoleType Role { get; set; }
        public MustBe(RoleType Role)
        {
            this.Role = Role;
        }

        public override bool IsValid(object value)
        {
            ApplicationUser oadskd = (ApplicationUser)value;

            bool isOfAskedRole = false;
            if (this.Role == RoleType.Company)
                isOfAskedRole = oadskd.Roles.Any(x => x.RoleId == this.companyRoleID);
            else if (this.Role == RoleType.User)
                isOfAskedRole = oadskd.Roles.Any(x => x.RoleId == this.userRoleID);

            return isOfAskedRole;
        }
    }

    public class StringLengthRangeAttribute : ValidationAttribute
    {
        public int Minimum { get; set; }
        public int Maximum { get; set; }

        public StringLengthRangeAttribute()
        {
            this.Minimum = 0;
            this.Maximum = int.MaxValue;
        }

        public override bool IsValid(object value)
        {
            string strValue = value as string;
            if (!string.IsNullOrEmpty(strValue))
            {
                int len = strValue.Length;
                return len >= this.Minimum && len <= this.Maximum;
            }
            return true;
        }
    }
}