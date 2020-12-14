using System;

namespace Library
{
    public class ConfigManager : IParser
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
                parser = new JSON_Parser(path, config);
            }
            else
            {
                throw new ArgumentNullException($"Cannot work with files of such extension((");
            }
        }

        public T GetOptions<T>() => parser.GetOptions<T>();
    }
}
