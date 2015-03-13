using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Account
{
    public class AccountSettingsViewModel
    {
        public string Id { get; set; }

        [EmailAddress(ErrorMessage = "Az e-mail cím nem megfelelő formátumú.")]
        [DataType(DataType.EmailAddress)]
        public String Email { get; set; }

        [StringLength(40, ErrorMessage = "A jelszó maximum 40 karakter lehet.")]
        [RegularExpression("^[A-Za-z0-9_-]{5,40}$", ErrorMessage = "A jelszó formátuma nem megfelelő.")]
        [DataType(DataType.Password)]
        public String UserPassword { get; set; }

        [Compare("UserPassword", ErrorMessage = "A két jelszó nem egyezik.")]
        [DataType(DataType.Password)]
        public String UserConfirmPassword { get; set; }

        [StringLength(50, ErrorMessage = "Maximum 50 karakter hosszú lehet.")]
        public String Name { get; set; }

        public AccountSettingsViewModel()
        {

        }

        public AccountSettingsViewModel(User p_usr)
        {
            Id = p_usr.Id;
            Email = p_usr.Email;
            Name = p_usr.Name;
        }

    }
}