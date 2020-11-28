using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    class ConfigManager : IParser
    {
        IParser parser;
        public ConfigManager(string path, string config)
        {
            if (path.EndsWith(".xml"))
            {
                parser = new XML_Parser(path, config);
            }
            else if (path.EndsWith(".json"))
            {
                parser = new JsonParser(path, config);
            }
            else
            {
                throw new ArgumentNullException($"Cannot work with files of such extension((");
            }
        }
        public new Options ParseOptions() => parser.ParseOptions();

    }
}
