using System;
using System.Runtime.CompilerServices;

namespace NLog.GoogleChat.NetCore31.Demo.Helpers
{
    public static class LogHelper
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 寫入Info紀錄
        /// </summary>
        /// <param name="siteName">網站名稱</param>
        /// <param name="logger">logger名稱</param>
        /// <param name="message">訊息</param>
        /// <param name="userId">使用者編號</param>
        /// <param name="jsonData">額外資料</param>
        /// <param name="callerFilePath">呼叫的method路徑(不須填寫,自動帶入)</param>
        /// <param name="callerLineNumber">呼叫的method行數(不須填寫,自動帶入)</param>
        /// <param name="callerMemberName">呼叫的method名稱(不須填寫,自動帶入)</param>
        public static void LogInfo(
            string siteName,
            string logger,
            string message,
            string userId,
            string jsonData = null,
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] long callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "")
        {
            var logEvenInfo = new LogEventInfo()
            {
                LoggerName = logger,
                Level = NLog.LogLevel.Info,
                Message = message,
            };
            logEvenInfo.Properties["Application"] = siteName;
            logEvenInfo.Properties["UserId"] = userId;
            logEvenInfo.Properties["ExtraData"] = jsonData;
            logEvenInfo.Properties["Caller"] = $"{callerMemberName}({callerFilePath}:{callerLineNumber})";
            logEvenInfo.Properties["Environment"] = Startup.StaticWebEnv.EnvironmentName;

            _logger.Log(logEvenInfo);
        }

        /// <summary>
        /// 寫入Error紀錄
        /// </summary>
        /// <param name="siteName">網站名稱</param>
        /// <param name="logger">logger名稱</param>
        /// <param name="message">訊息</param>
        /// <param name="exception">Exception物件</param>
        /// <param name="userId">使用者編號</param>
        /// <param name="jsonData">額外資料</param>
        /// <param name="callerFilePath">呼叫的method路徑(不須填寫,自動帶入)</param>
        /// <param name="callerLineNumber">呼叫的method行數(不須填寫,自動帶入)</param>
        /// <param name="callerMemberName">呼叫的method名稱(不須填寫,自動帶入)</param>
        public static void LogError(
            string siteName,
            string logger,
            string message,
            Exception exception,
            string userId,
            string jsonData = null,
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] long callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "")
        {
            var logEvenInfo = new LogEventInfo()
            {
                LoggerName = logger,
                Level = NLog.LogLevel.Error,
                Message = message,
                Exception = exception
            };
            logEvenInfo.Properties["Application"] = siteName;
            logEvenInfo.Properties["UserId"] = userId;
            logEvenInfo.Properties["ExtraData"] = jsonData;
            logEvenInfo.Properties["Caller"] = $"{callerMemberName}({callerFilePath}:{callerLineNumber})";
            logEvenInfo.Properties["Environment"] = Startup.StaticWebEnv.EnvironmentName;

            _logger.Log(logEvenInfo);
        }

        public static string GetMethodName([System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            return memberName;
        }
    }
}
