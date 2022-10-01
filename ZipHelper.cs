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
            if (s.database_name == "test") return 0;

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

            string[] file_list = Directory.GetFiles(s.local_folder);
            int file_num = file_list.Length;
           
            int files_per_zip = 10000;
            // int division, usually add 1 for the remainder
            int zip_files_needed = (file_num % files_per_zip == 0) 
                                   ? file_num / files_per_zip 
                                   : (file_num/files_per_zip) + 1;
            int start_file_num, end_file_num;
            string start_file, end_file;

            for (int j = 0; j < zip_files_needed; j++)
            {
                start_file_num = (j * files_per_zip);
                start_file = (start_file_num + 1).ToString();

                if ((j + 1) * files_per_zip >= file_num)
                {
                    end_file_num = file_num;
                }
                else
                {
                    end_file_num = (j * files_per_zip) + files_per_zip;
                }
                end_file = (end_file_num).ToString();


                string file_name = s.database_name + " " + today + " "
                                          + start_file + " to " + end_file;
                string zip_file_path = Path.Combine(zip_folder_path, file_name + ".zip");

                using (ZipArchive zip = ZipFile.Open(zip_file_path, ZipArchiveMode.Create))
                {
                    int last_backslash = 0;
                    string source_file_path = "";
                    string entry_name = "";

                    for (int i = start_file_num; i < end_file_num; i++)
                    {
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

            int file_num = 0;
            string[] folder_list = Directory.GetDirectories(s.local_folder);
            string source_folder_path, folder_name, previous_folder_name, zip_file_path;
            string source_file_path, entry_name, first_folder, final_zip_name;
            int folder_backslash, file_backslash;

            // produce a zip for each group of folders
            // checking that the max size has not been exceeded
            // after each folder
            int folder_num = folder_list.Length;      // total folders in source directory
            long max_zip_zize = 18 * 1024 * 1024;     // 18 MB
            long zip_file_zize;                       // Used for current legth of current zip file    
            zip_file_path = "";                       // Used for current zip
            first_folder = ""; previous_folder_name = "";
            bool new_zip_required = false;            // set true if current file size greater than max size

            int k = -1;                               // k is the index of the source folders in the source directory
            while (k < folder_num)
            {
                k++;

                // if the very last folder called the file size to be exceeded
                // k now equals hew total folder number and 
                //  there is no need for an additional zip file

                if (k == folder_num) break;

                new_zip_required = false;

                // this code run at the beginning and each time inner loop is exited
                // need to create zip file path using the first folder in this 'batch'

                source_folder_path = folder_list[k];
                folder_backslash = source_folder_path.LastIndexOf("\\") + 1;
                folder_name = source_folder_path.Substring(folder_backslash);
                first_folder = folder_name;
                zip_file_path = Path.Combine(zip_folder_path, s.database_name + " " +
                                        today + " " + first_folder + " onwards.zip");


                // add the files to the archive, as long as it stays within the size limit
                // initial k value is the sme as in the outer loop

                using (ZipArchive zip = ZipFile.Open(zip_file_path, ZipArchiveMode.Create))
                {

                    while (k < folder_num && !new_zip_required)
                    {
                        source_folder_path = folder_list[k];
                        folder_backslash = source_folder_path.LastIndexOf("\\") + 1;
                        folder_name = source_folder_path.Substring(folder_backslash);
                        previous_folder_name = folder_name;

                        string[] file_list = Directory.GetFiles(source_folder_path);
                        {
                            for (int i = 0; i < file_list.Length; i++)
                            {
                                source_file_path = file_list[i];
                                file_backslash = source_file_path.LastIndexOf("\\");
                                entry_name = source_file_path.Substring(file_backslash);
                                zip.CreateEntryFromFile(source_file_path, entry_name);
                            }
                        }
                        file_num += file_list.Length;
                        logging_repo.LogLine("Zipped " + folder_name);

                        zip_file_zize = new FileInfo(zip_file_path).Length;

                        // A new zip file may be required
                        new_zip_required = zip_file_zize > max_zip_zize;
                                          
                        if (!new_zip_required)
                        {
                            k++;
                        }
                    }
                }

                // rename the zip file that has just been completed
                final_zip_name = Path.Combine(zip_folder_path, s.database_name + " " +
                                             today + " " + first_folder + " to " + previous_folder_name + ".zip");

                File.Move(zip_file_path, final_zip_name);

            }

            return file_num;
        }
    }
}
