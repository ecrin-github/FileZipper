using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileZipper
{
    class Program
    {
        static void Main(string[] args)
        {
            var parsedArguments = Parser.Default.ParseArguments<Options>(args)
            .WithParsed(RunOptions)
            .WithNotParsed(HandleParseError);
        }

        private static void RunOptions(Options opts)
        {
            LoggingDataLayer logging_repo = new LoggingDataLayer();
            Zipper zip = new Zipper(logging_repo, opts);
            zip.ZipFiles(opts);
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            // handle errors
        }
    }


    public class Options
    {
        // Lists the command line arguments and options

        [Option('s', "source_ids", Required = false, Separator = ',', HelpText = "Comma separated list of Integer ids of data sources.")]
        public IEnumerable<int> source_ids { get; set; }

        [Option('A', "zip all sources", Required = false, HelpText = "If present, zips the files from all source folders")]
        public bool all_sources { get; set; }

        [Option('J', "zip json files", Required = false, HelpText = "If present, zips the json files produced by aggregation")]
        public bool zip_json { get; set; }

    }
}
