﻿
using ICNAPP.DataAccess.CSAccess.Interface;
using ICNAPP.DataAccess.CSAccess.Repository;
using ICNAPP.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace ICNAPPHelpers
{
    public static class LogHelper
    {

        public static void WriteExceptionLog(Exception ex)
        {
            ILoggingRepository logRepository = new LoggingRepository();
            logRepository.Add(new Logging()
            {
                CreatedDate = DateTime.Now,
                ExceptionMessage = ex.Message,
                InnerExceptionMessage = ex.InnerException.Message,
                LineNumber = GetLineNumber(ex),
                MethodName = GetMethodName(ex),
                Type = Convert.ToString((int)ICNAPP.Models.Enumeration.LogType.Debug)
            });
            logRepository.Save();
        }

        public static void WriteDebugLog(string log)
        {
            ILoggingRepository logRepository = new LoggingRepository();
            logRepository.Add(new Logging()
            {
                CreatedDate = DateTime.Now,
                Message = log,
                Type = Convert.ToString((int)ICNAPP.Models.Enumeration.LogType.Debug)
            });
            logRepository.Save();
        }

        private static int GetLineNumber(Exception ex)
        {
            var lineNumber = 0;
            const string lineSearch = ":line ";
            var index = ex.StackTrace.LastIndexOf(lineSearch);
            if (index != -1)
            {
                var lineNumberText = ex.StackTrace.Substring(index + lineSearch.Length);
                if (int.TryParse(lineNumberText, out lineNumber))
                {
                }
            }
            return lineNumber;
        }

        private static string GetMethodName(Exception ex)
        {
            StackTrace stackTrace = new StackTrace(ex);
            Assembly thisasm = Assembly.GetExecutingAssembly();
            string methodname = stackTrace.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == thisasm).Name;

            return methodname;
        }

    }
}
