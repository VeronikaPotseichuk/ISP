using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Lab3
{
    [Serializable]
    [XmlRoot(ElementName = "Options", Namespace = "")]
    class XML_Options
    {
        [XmlElement("SourcePath")]
        public string SourcePath { get; set; }     

        [XmlElement("TargetPath")]
        public string TargetPath { get; set; }      

        [XmlElement("LoggerPath")]
        public string LoggerPath { get; set; }      

        [XmlElement("NeedToEncrypt")]
        public bool NeedToEncrypt { get; set; }     

        [XmlElement("ArchiveOptions")]
        public bool ArchiveOptions { get; set; }      
        public XML_Options() { }
        public XML_Options(string sourcePath, string targetPath, string loggerPath, bool needToEncrypt, bool archiveOptions)
        {
            SourcePath = sourcePath;
            TargetPath = targetPath;
            LoggerPath = loggerPath;
            NeedToEncrypt = needToEncrypt;
            ArchiveOptions = archiveOptions;
        }
    }
}
