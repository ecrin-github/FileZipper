using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace FileZipper
{
    class JSONHelper
    {
        string folder_base, zip_folder_base;
        string folder_path, file_path;
        string[] folder_list;
        int number_zip_files_needed;
        string today;
        LoggingDataLayer logging_repo;

        public JSONHelper(LoggingDataLayer _logging_repo)
        {
            today = DateTime.Now.ToString("yyyyMMdd");
            logging_repo = _logging_repo;
        }


        public void ZipStudyFiles()
        {
            folder_base = logging_repo.StudyJSONFolder;
            zip_folder_base = logging_repo.StudyJSONZipsFolder;
            folder_list = Directory.GetDirectories(folder_base);

            // Batch size of 10 folders, each of about 10,000 records - each zip therefore 100,000 files
            // should produce about 6 zips
            int folder_batch = 10;

            number_zip_files_needed = (folder_list.Length % folder_batch == 0) ? (folder_list.Length / folder_batch) : (folder_list.Length / folder_batch) + 1;
            for (int n = 0; n < number_zip_files_needed; n++)
            {
                int start_number = n * folder_batch;
                int end_limit = ((n + 1) * folder_batch);
                if (end_limit > folder_list.Length) end_limit = folder_list.Length;

                // Folder name lengths could change (not common)

                string first_folder = folder_list[start_number];
                string last_folder = folder_list[end_limit - 1];
                int ff_studies = first_folder.LastIndexOf("\\studies ") + 9;
                int lf_studies = last_folder.LastIndexOf("\\studies ") + 9;
                string first_id = first_folder.Substring(ff_studies).Substring(0, 7);
                int last_id_pos = last_folder.Substring(lf_studies).LastIndexOf(" ") + 1;
                string last_id = last_folder.Substring(lf_studies).Substring(last_id_pos, 7);
                string zip_file_suffix = today + " " + first_id + " to " + last_id;

                // get total number of files in this batch
                int file_num = 0;
                for (int i = start_number; i < end_limit; i++)
                {
                    folder_path = folder_list[i];
                    string[] file_list = Directory.GetFiles(folder_path);
                    file_num += file_list.Length;
                }
                string file_num_suffix = " [" + file_num.ToString() + " files]";

                string zip_file_path = Path.Combine(zip_folder_base, "study ids " +
                                            zip_file_suffix + file_num_suffix + ".zip");
                using (ZipArchive zip = ZipFile.Open(zip_file_path, ZipArchiveMode.Create))
                {
                    for (int i = start_number; i < end_limit; i++)
                    {
                        folder_path = folder_list[i];
                        string[] file_list = Directory.GetFiles(folder_path);
                        int last_backslash = 0;
                        string entry_name = "";
                        for (int j = 0; j < file_list.Length; j++)
                        {
                            file_path = file_list[j];
                            last_backslash = file_path.LastIndexOf("\\") + 1;
                            entry_name = file_path.Substring(last_backslash);
                            zip.CreateEntryFromFile(file_path, entry_name);
                        }
                    }
                }
            }
        }


        public void ZipObjectFiles()
        {
            folder_base = logging_repo.ObjectJSONFolder;
            zip_folder_base = logging_repo.ObjectJSONZipsFolder;
            folder_list = Directory.GetDirectories(folder_base);

            // Batch size of 10 folders, each of about 10,000 records - each zip therefore 100,000 files
            // should produce about 9 zips
            int folder_batch = 10;
            int k = 0;
            number_zip_files_needed = (folder_list.Length % folder_batch == 0) ? (folder_list.Length / folder_batch) : (folder_list.Length / folder_batch) + 1;
            for (int n = 0; n < number_zip_files_needed; n++)
            {
                int start_number = n * folder_batch;
                int end_limit = ((n + 1) * folder_batch);
                if (end_limit > folder_list.Length) end_limit = folder_list.Length;

                // Folder name lengths could change (not common)

                string first_folder = folder_list[start_number];
                string last_folder = folder_list[end_limit - 1];
                int ff_objects = first_folder.LastIndexOf("\\objects ") + 9;
                int lf_objects = last_folder.LastIndexOf("\\objects ") + 9;
                string first_id = first_folder.Substring(ff_objects).Substring(0, 8);
                int last_id_pos = last_folder.Substring(lf_objects).LastIndexOf(" ") + 1;
                string last_id = last_folder.Substring(lf_objects).Substring(last_id_pos, 8);
                string zip_file_suffix = today + " " + first_id + " to " + last_id;

                // get total number of files in this batch
                int file_num = 0;
                for (int i = start_number; i < end_limit; i++)
                {
                    folder_path = folder_list[i];
                    string[] file_list = Directory.GetFiles(folder_path);
                    file_num += file_list.Length;
                }
                string file_num_suffix = " [" + file_num.ToString() + " files]";

                string zip_file_path = Path.Combine(zip_folder_base, "object ids " +
                                            zip_file_suffix + file_num_suffix + ".zip");
                using (ZipArchive zip = ZipFile.Open(zip_file_path, ZipArchiveMode.Create))
                {
                    for (int i = start_number; i < end_limit; i++)
                    {
                        folder_path = folder_list[i];
                        string[] file_list = Directory.GetFiles(folder_path);
                        int last_backslash = 0;
                        string entry_name = "";
                        for (int j = 0; j < file_list.Length; j++)
                        {
                            file_path = file_list[j];
                            last_backslash = file_path.LastIndexOf("\\") + 1;
                            entry_name = file_path.Substring(last_backslash);
                            zip.CreateEntryFromFile(file_path, entry_name);
                            k++;
                            if (k % 100 == 0) logging_repo.LogLine(k.ToString() + "files zipped");
                        }
                    }
                }
            }
        }
    }
}
