using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Lab3
{
    class JSON_Parser : IParser
    {
        private string Path { get; }
        private string ConfigPath { get; }
        public JSON_Parser(string path, string configPath)
        {
            Path = path;
            ConfigPath = configPath;
        }
        public Options ParseOptions()
        {
            try
            {
                string file;
                JSON_Options jOp;
                using (StreamReader sr = new StreamReader(Path))
                {
                    file = sr.ReadToEnd();
                    jOp = JSON_Serializer.Deserialize<JSON_Options>(file);
                }
                return new Options(jOp.SourcePath, jOp.TargetPath, jOp.LoggerPath, jOp.NeedToEncrypt, jOp.ArchiveOptions);
            }
            catch
            {
                throw new Exception($"Smth went wrong with json file.");
            }
        }

    }
}
