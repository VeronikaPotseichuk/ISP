using System.Data;
using System.IO;

namespace Library
{
    public class XML_Generator
    {
        readonly string outputFolder;

        public XML_Generator(string outputFolder)
        {
            this.outputFolder = outputFolder;
        }

        public void WriteToXml(DataSet dataSet, string fileName)
        {
            dataSet.WriteXml(Path.Combine(outputFolder, $"{fileName}.xml"));

            dataSet.WriteXmlSchema(Path.Combine(outputFolder, $"{fileName}.xsd"));
        }
    }
}
