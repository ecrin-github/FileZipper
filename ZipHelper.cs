using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace FileZipper
{
    class ZipHelper
    {

        string today;
        LoggingDataLayer logging_repo;

        public ZipHelper(Source s, LoggingDataLayer _logging_repo)
        {
            today = DateTime.Now.ToString("yyyyMMdd");
            logging_repo = _logging_repo;
        }


        public int ZipFilesSingleFolder(Source s, string zips_folder_base)
        {
            string zip_folder_path = Path.Combine(zips_folder_base, s.database_name);

            // check if folder exists, but if folder already exists needs 
            // to be emptied, otherwise create a new folder.

            if (Directory.Exists(zip_folder_path))
            {
                string[] zip_filePaths = Directory.GetFiles(zip_folder_path);
                foreach (string filePath in zip_filePaths)
                {
                    File.Delete(filePath);
                }
            }
            else
            {
                Directory.CreateDirectory(zip_folder_path);
            }

            // if folder already exists needs to be emptied
            
            string[] file_list = Directory.GetFiles(s.local_folder);
            int file_num = file_list.Length;
            int last_backslash = 0;

            string zip_file_path = Path.Combine(zip_folder_path, s.database_name + " " + today + ".zip");
            using (ZipArchive zip = ZipFile.Open(zip_file_path, ZipArchiveMode.Create))
            {
                string source_file_path = "";
                string entry_name = "";
                for (int i = 0; i < file_num; i++)
                {
                    source_file_path = file_list[i];
                    last_backslash = source_file_path.LastIndexOf("\\") + 1;
                    entry_name = source_file_path.Substring(last_backslash);
                    zip.CreateEntryFromFile(source_file_path, entry_name);
                }
            }
            logging_repo.LogLine("Zipped " + zip_file_path);
            return file_num;
        }


        public int ZipFilesMultipleFolders(Source s, string zips_folder_base)
        {
            string zip_folder_path = Path.Combine(zips_folder_base, s.database_name);

            // check if folder exists, but if folder already exists needs 
            // to be emptied, otherwise create a new folder.

            if (Directory.Exists(zip_folder_path))
            {
                string[] zip_filePaths = Directory.GetFiles(zip_folder_path);
                foreach (string filePath in zip_filePaths)
                {
                    File.Delete(filePath);
                }
            }
            else
            {
                Directory.CreateDirectory(zip_folder_path);
            }

            string[] folder_list = Directory.GetDirectories(s.local_folder);
            string source_folder_path = "";
            string folder_name = "";
            int file_num = 0;
            int last_backslash = 0;

            for (int j = 0; j < folder_list.Length; j++)
            {
                source_folder_path = folder_list[j];
                last_backslash = source_folder_path.LastIndexOf("\\") + 1;
                folder_name = source_folder_path.Substring(last_backslash);

                string zip_file_path = Path.Combine(zip_folder_path, s.database_name + " " + today + "-" + folder_name + ".zip");
                string[] file_list = Directory.GetFiles(source_folder_path);
                file_num += file_list.Length;

                using (ZipArchive zip = ZipFile.Open(zip_file_path, ZipArchiveMode.Create))
                {
                    string source_file_path = "";
                    for (int i = 0; i < file_list.Length; i++)
                    {
                        string entry_name = "";
                        source_file_path = file_list[i];
                        last_backslash = source_file_path.LastIndexOf("\\") + 1;
                        entry_name = source_file_path.Substring(last_backslash);
                        zip.CreateEntryFromFile(source_file_path, entry_name);
                    }
                }
                logging_repo.LogLine("Zipped " + zip_file_path);
            }
            return file_num;
        }
    }
}
