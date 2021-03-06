﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class OrganizationViewModel
    {
        [Required(ErrorMessage = "A név megadása kötelező.")]
        [StringLength(50, ErrorMessage = "A név hossza legfeljebb 50 karakter lehet.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Az alapítás évének megadása kötelező.")]
        public int FoundationYear { get; set; }

        [Required(ErrorMessage = "Az ország megadása kötelező.")]
        [StringLength(50, ErrorMessage = "A ország nevének hossza legfeljebb 50 karakter lehet.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "A város megadása kötelező.")]
        [StringLength(50, ErrorMessage = "A város nevévek hossza legfeljebb 50 karakter lehet.")]
        public string City { get; set; }

        [StringLength(50, ErrorMessage = "A tevékenység leírása maximum 50 karakter lehet.")]
        public string Activity { get; set; }

        [Required(ErrorMessage = "A leírás megadása kötelező.")]
        public string Description { get; set; }

    }
}