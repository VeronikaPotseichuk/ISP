using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;

namespace Lab3
{
    class XML_Parser
    {
        private string Path { get; }
        private string ConfigPath { get; }
        public XML_Parser(string path, string config)
        {
            Path = path;
            ConfigPath = config;
        }

        public Options ParseOptions()
        {
            try
            {
                XmlSchemaSet schema = new XmlSchemaSet();
                schema.Add("", ConfigPath);
                XmlDocument document = new XmlDocument();
                document.LoadXml(Path);
                document.Schemas = schema;
                ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationEventHandler);
                document.Validate(eventHandler);
                var op = DeserializeObj(Path);
                return new Options(op.SourcePath, op.TargetPath, op.LoggerPath, op.NeedToEncrypt, op.ArchiveOptions);
            }
            catch
            {
                throw new Exception(@"Something went wrong while trying to parse your xml file");
            }
        }
        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            XmlSeverityType type = XmlSeverityType.Warning;
            if (!Enum.TryParse<XmlSeverityType>("Error", out type))
            {
                if (type == XmlSeverityType.Error)
                {
                    throw new Exception(e.Message);
                }
            }
        }
        public XML_Options DeserializeObj(string xPath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(XML_Options));
                XML_Options op;
                using (Stream sr = new FileStream(xPath, FileMode.Open))
                {
                    op = (XML_Options)serializer.Deserialize(sr);
                }
                return op;
            }
            catch
            {
                throw new Exception(@"Couldn't deserialize your xml file");
            }
        }
    }
}
