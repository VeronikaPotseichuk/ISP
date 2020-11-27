using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    class JSON_Options
    {
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }
        public string LoggerPath { get; set; }
        public bool NeedToEncrypt { get; set; }
        public bool ArchiveOptions { get; set; }
        public JSON_Options() { }
        public JSON_Options(string sourcePath, string targetPath, string loggerPath, bool needToEncrypt, bool archiveOptions)
        {
            SourcePath = sourcePath;
            TargetPath = targetPath;
            LoggerPath = loggerPath;
            NeedToEncrypt = needToEncrypt;
            ArchiveOptions = archiveOptions;
        }
    }
}
