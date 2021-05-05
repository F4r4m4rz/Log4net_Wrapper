using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;

namespace Ai.Gene.Logging
{
    public static class AibelLog4NetConfig
    {
        public static void XmlConfig()
        {
            XmlConfigurator.Configure();
        }

        public static ILog GetLogger(Type type)
        {
            return LogManager.GetLogger(type);
        }

        /// <summary>
        /// Instantiate SqlLogger which logs from level WARN
        /// </summary>
        /// <param name="appenderName"></param>
        /// <param name="connectionType"></param>
        /// <param name="connectionString"></param>
        /// <param name="bufferSize"></param>
        /// <param name="commandText">Max. parameters to be
        /// <para>@log_date, @thread, @log_level, @logger, @message, @exception</para></param>
        public static void SqlAppender(string appenderName,
                                       string connectionType,
                                       string connectionString,
                                       int bufferSize,
                                       string commandText,
                                       LogLevelEnum level)
        {
            var appender = new AdoNetAppender();
            appender.Name = appenderName;
            appender.ConnectionType = connectionType;
            appender.ConnectionString = connectionString;
            appender.BufferSize = bufferSize;
            appender.CommandText = commandText;

            appender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@log_date",
                DbType = System.Data.DbType.DateTime,
                Layout = new RawTimeStampLayout()
            });

            appender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@thread",
                DbType = System.Data.DbType.String,
                Size = 255,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%thread"))
            });

            appender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@log_level",
                DbType = System.Data.DbType.String,
                Size = 50,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%level"))
            });

            appender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@logger",
                DbType = System.Data.DbType.String,
                Size = 255,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%loger"))
            });

            appender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@message",
                DbType = System.Data.DbType.String,
                Size = 4000,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%message"))
            });

            appender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@exception",
                DbType = System.Data.DbType.String,
                Size = 2000,
                Layout = new Layout2RawLayoutAdapter(new ExceptionLayout())
            });

            appender.ActivateOptions();
            AddLevelFilter(appender, level);
            AddToHierarchy(appender);
        }

        public static void ConsoleAppender(string name, LogLevelEnum level)
        {
            var consolAppender = new ConsoleAppender();
            consolAppender.Name = name;
            consolAppender.Layout = new PatternLayout("[%thread] %-5level %logger - %message%newline%exception");
            consolAppender.ActivateOptions();
            AddLevelFilter(consolAppender, level);
            AddToHierarchy(consolAppender);
        }

        public static void FileAppender(string name, LogLevelEnum level, string path)
        {
            var fileAppender = new FileAppender();
            fileAppender.Name = name;
            fileAppender.Layout = new PatternLayout("[%thread] %-5level %logger - %message%newline%exception");
            fileAppender.File = path;
            fileAppender.AppendToFile = true;
            fileAppender.LockingModel = new FileAppender.MinimalLock();
            fileAppender.ActivateOptions();
            AddLevelFilter(fileAppender, level);
            AddToHierarchy(fileAppender);
        }

        public static void ColorConsoleAppender(string name, LogLevelEnum level)
        {
            var colorConsole = new ColoredConsoleAppender();
            colorConsole.Name = name;
            colorConsole.Layout = new PatternLayout("[%thread] %-5level %logger - %message%newline%exception");
            colorConsole.AddMapping(new ColoredConsoleAppender.LevelColors()
            {
                Level = Level.Error,
                BackColor = ColoredConsoleAppender.Colors.Red,
                ForeColor = ColoredConsoleAppender.Colors.White
            });

            colorConsole.AddMapping(new ColoredConsoleAppender.LevelColors()
            {
                Level = Level.Warn,
                BackColor = ColoredConsoleAppender.Colors.Yellow,
                ForeColor = ColoredConsoleAppender.Colors.Red
            });

            colorConsole.ActivateOptions();
            AddLevelFilter(colorConsole, level);
            AddToHierarchy(colorConsole);
        }

        private static void AddLevelFilter(AppenderSkeleton appender, LogLevelEnum level)
        {
            switch (level)
            {
                case LogLevelEnum.DEBUG:
                    appender.AddFilter(new LevelRangeFilter() { LevelMin = Level.Debug });
                    break;
                case LogLevelEnum.INFO:
                    appender.AddFilter(new LevelRangeFilter() { LevelMin = Level.Info });
                    break;
                case LogLevelEnum.WARN:
                    appender.AddFilter(new LevelRangeFilter() { LevelMin = Level.Warn });
                    break;
                case LogLevelEnum.ERROR:
                    appender.AddFilter(new LevelRangeFilter() { LevelMin = Level.Error });
                    break;
                case LogLevelEnum.FATAL:
                    appender.AddFilter(new LevelRangeFilter() { LevelMin = Level.Fatal });
                    break;
                default:
                    break;
            }
        }

        private static void AddToHierarchy(IAppender appender)
        {
            Hierarchy h = LogManager.GetRepository() as Hierarchy;
            h.Root.AddAppender(appender);
            h.Configured = true;
        }
    }
}
