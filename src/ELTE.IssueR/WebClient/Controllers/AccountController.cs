using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebClient.Controllers
{
    public class AccountController : BaseController
    {

        #region General

        private byte[] Salt(byte[] name, byte[] pw)
        {
            byte[] result = pw;

            for (Int32 i = 0; i < pw.Length; i = i + 3)
            {
                result[i] = name[i % name.Length];

                if (i % 5 == 0)
                    --i;
                if (i % 7 == 0)
                    --i;
            }

            byte[] reverse = result.Reverse().ToArray();

            for (Int32 i = 0; i < result.Length; ++i)
            {
                result[i] += reverse[i];
            }

            return result;
        }

        #endregion

        #region Login/Logout



        #endregion

        #region Profile

        public ActionResult ProfileIndex()
        {


            return View("ProfileIndex");
        }

        #endregion

        #region Messages



        #endregion


    }
}