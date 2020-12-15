using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileZipper
{
    class LoggingDataLayer
    {
        private string connString;
        private Source source;
        private string logfile_startofpath;
        private string logfile_path;
        private StreamWriter sw;
        private string study_JSON_folder;
        private string object_JSON_folder;
        private string zips_folder;

        /// <summary>
        /// Parameterless constructor is used to automatically build
        /// the connection string, using an appsettings.json file that 
        /// has the relevant credentials (but which is not stored in GitHub).
        /// </summary>
        /// 
        public LoggingDataLayer()
        {
            IConfigurationRoot settings = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();
            builder.Host = settings["host"];
            builder.Username = settings["user"];
            builder.Password = settings["password"];

            builder.Database = "mon";
            connString = builder.ConnectionString;

            logfile_startofpath = settings["logfilepath"];
            study_JSON_folder = settings["study json folder"];
            object_JSON_folder = settings["object json folder"];
            zips_folder = settings["zips folder"];

        }

        public Source SourceParameters => source;
        public string StudyJSONFolder => study_JSON_folder;
        public string ObjectJSONFolder => object_JSON_folder;
        public string ZipsFolder => zips_folder;

        public void OpenLogFile(IEnumerable<int> source_ids, bool all_sources, bool zip_json)
        {
            string dt_string = DateTime.Now.ToString("s", System.Globalization.CultureInfo.InvariantCulture)
                              .Replace("-", "").Replace(":", "").Replace("T", " ");
            string zip_type = "";
            if (all_sources)
            {
                zip_type = "ALL";
            }
            else if (zip_json)
            {
                zip_type = "JSON";
            }
            else if (source_ids != null)
            {
                zip_type = "Specific";
            }
            logfile_path = logfile_startofpath + "ZP " + zip_type + " " + dt_string + ".log";
            sw = new StreamWriter(logfile_path, true, System.Text.Encoding.UTF8);
        }

        public void LogLine(string message, string identifier = "")
        {
            string dt_string = DateTime.Now.ToShortDateString() + " : " + DateTime.Now.ToShortTimeString() + " :   ";
            string feedback = dt_string + message + identifier;
            Transmit(feedback);
        }

        public void LogHeader(string message)
        {
            string dt_string = DateTime.Now.ToShortDateString() + " : " + DateTime.Now.ToShortTimeString() + " :   ";
            string header = dt_string + "**** " + message + " ****";
            Transmit("");
            Transmit(header);
        }

        public void LogError(string message)
        {
            string dt_string = DateTime.Now.ToShortDateString() + " : " + DateTime.Now.ToShortTimeString() + " :   ";
            string error_message = dt_string + "***ERROR*** " + message;
            Transmit("");
            Transmit("+++++++++++++++++++++++++++++++++++++++");
            Transmit(error_message);
            Transmit("+++++++++++++++++++++++++++++++++++++++");
            Transmit("");
        }

        public void CloseLog()
        {
            LogHeader("Closing Log");
            sw.Flush();
            sw.Close();
        }

        private void Transmit(string message)
        {
            sw.WriteLine(message);
            Console.WriteLine(message);
        }

        public Source FetchSourceParameters(int source_id)
        {
            using (NpgsqlConnection Conn = new NpgsqlConnection(connString))
            {
                source = Conn.Get<Source>(source_id);
                return source;
            }
        }

        public IEnumerable<int> RetrieveDataSourceIds()
        {
            string sql_string = @"select id from sf.source_parameters
                                where id > 100115
                                order by preference_rating;";

            using (var conn = new NpgsqlConnection(connString))
            {
                return conn.Query<int>(sql_string);
            }
        }

    }
}

