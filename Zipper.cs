using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileZipper
{
    class Zipper
    {
        LoggingDataLayer logging_repo;
        string zips_folder_base;

        public Zipper(LoggingDataLayer _logging_repo, Options opts)
        {
            logging_repo = _logging_repo;
        }

        public void ZipFiles(Options opts)
        {
            logging_repo.OpenLogFile(opts.source_ids, opts.all_sources, opts.zip_json);
            logging_repo.LogHeader("Setup");

            logging_repo.LogLine("zipping all sources =  " + opts.all_sources);
            logging_repo.LogLine("zipping json files =  " + opts.zip_json);
            if (opts.source_ids != null)
            {
                logging_repo.LogLine("Zipping specific source files");
            }

            zips_folder_base = logging_repo.ZipsFolder;

            if (opts.all_sources)
            {
                ZipAllSourceFiles();
            }
            else if (opts.source_ids.Count() > 0)
            {
                foreach (int source_id in opts.source_ids)
                {
                    ZipFilesForSingleSource(source_id);
                }
            }
            else if (opts.zip_json)
            {
                foreach (int source_id in opts.source_ids)
                {
                    ZipJSONFiles();
                }
            }

            logging_repo.CloseLog();

        }

        public void ZipAllSourceFiles()
        {
            // Get sources
            IEnumerable<int> source_ids = logging_repo.RetrieveDataSourceIds();
            logging_repo.LogLine("Source Ids obtained");
            foreach (int si in source_ids)
            {
                ZipFilesForSingleSource(si);
            }
        }

        public void ZipFilesForSingleSource(int source_id)
        {
            Source s = logging_repo.FetchSourceParameters(source_id);
            logging_repo.LogLine("Zipping files from " + s.local_folder);

            int num = 0;
            ZipHelper zh = new ZipHelper(s, logging_repo);
            if ((bool)s.local_files_grouped)
            {
                num = zh.ZipFilesMultipleFolders(s, zips_folder_base);
            }
            else
            {
                num = zh.ZipFilesSingleFolder(s, zips_folder_base);
            }

            logging_repo.LogLine("Zipped " + num.ToString() + " files from " + s.database_name);

        }

        public void ZipJSONFiles()
        {
            JSONHelper jh = new JSONHelper(logging_repo);
            jh.ZipStudyFiles();
            jh.ZipObjectFiles();
        }

    }
}
