using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Wiki
{
    public class WikiViewModel
    {
        /// <summary>
        /// Melyik szekció wiki oldala. pl. Organization, Project, stb.
        /// </summary>
        public String Section { get; set; }

        public Dictionary<String, Int32> Nav { get; set; }

        public DocumentsViewModel Body { get; set; }
    }
}