using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Logger
{
    public enum LogType
    {
        Account,
        Bug,
        Critical
    }

    public static class Logger
    {
        private static object _lockObject = new object();
        private static String _currentLogName = "log1";

        public static void Log(LogType t, String msg, Func<String, String> MapPath)
        {
            String filename = MapPath("~/App_Data/Log/" + _currentLogName + ".log");

            String LineToWrite = DateTime.Now.ToString() + " - " + t.ToString() + " : " + msg;

            lock (_lockObject)
            {
                if (!System.IO.File.Exists(filename))
                    System.IO.File.Create(filename);

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename,true))
                {
                    sw.WriteLine(LineToWrite);
                    sw.Close();
                }
            }
            
            System.IO.FileInfo fi = new System.IO.FileInfo(filename);
            if (fi.Length > 10485760) //Get default value (10 MB)
            {
                lock(_lockObject)
                {
                    String targetfile = MapPath("~/App_Data/Log/Archive/" + _currentLogName + ".log");
                    System.IO.File.Copy(filename, targetfile);
                    _currentLogName = "log" + (Convert.ToInt32(_currentLogName.Substring(3)) + 1).ToString();
                    filename = MapPath("~/App_Data/Log/" + _currentLogName + ".log");
                    System.IO.File.Create(filename);
                }
            }
        }
    }
}