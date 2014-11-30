using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.WebClient.Models.Account
{
    public class NewUserViewModel
    {
        [Required(ErrorMessage = "A felhasználó név megadása kötelező.")]
        [StringLength(30, ErrorMessage = "A felhasználó név maximum 30 karakter lehet.")]
        [MinLength(3, ErrorMessage = "A felhasználó név minimum 3 karakter lehet.")]
        [RegularExpression("^[A-Za-z0-9_-]{3,30}$", ErrorMessage = "A felhasználónév formátuma nem megfelelő.")]
        public String UserName { get; set; }

        [Required(ErrorMessage = "A név megadása kötelező.")]
        [StringLength(50, ErrorMessage = "A név maximum 50 karakter lehet.")]
        public String Name { get; set; }

        [Required(ErrorMessage = "Az e-mail cím megadása kötelező.")]
        [EmailAddress(ErrorMessage = "Az e-mail cím nem megfelelő formátumú.")]
        [DataType(DataType.EmailAddress)]
        public String Email { get; set; }

        [Required(ErrorMessage = "A jelszó megadása kötelező.")]
        [StringLength(40, ErrorMessage = "A jelszó maximum 40 karakter lehet.")]
        [RegularExpression("^[A-Za-z0-9_-]{5,40}$", ErrorMessage = "A jelszó formátuma nem megfelelő.")]
        [DataType(DataType.Password)]
        public String UserPassword { get; set; }

        [Required(ErrorMessage = "A jelszó ismételt megadása kötelező.")]
        [Compare("UserPassword", ErrorMessage = "A két jelszó nem egyezik.")]
        [DataType(DataType.Password)]
        public String UserConfirmPassword { get; set; }
    }
}