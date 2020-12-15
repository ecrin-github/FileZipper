using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileZipper
{
    [Table("sf.source_parameters")]
    public class Source
    {
        public int id { get; set; }
        public int? preference_rating { get; set; }
        public string database_name { get; set; }
        public int default_harvest_type_id { get; set; }
        public bool requires_file_name { get; set; }
        public bool uses_who_harvest { get; set; }
        public string local_folder { get; set; }
        public bool? local_files_grouped { get; set; }
        public int? grouping_range_by_id { get; set; }
        public string local_file_prefix { get; set; }
    }
}
