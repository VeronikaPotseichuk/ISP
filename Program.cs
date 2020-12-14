﻿using System;
using System.ServiceProcess;
using System.IO;
using ServiceLibrary;
using System.Xml.Schema;
using System.Xml.Linq;

namespace Lab4
{
    static class Program
    {
        static readonly string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dataOptions", "dataOptions.xml");
        static readonly string xsdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dataOptions", "dataOptions.xsd");
        static readonly string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dataOptions", "dataOptions.json");

        static void Main()
        {
            ConfigManager configManager;
            DataOptions dataOptions;

            DataIO appInsights;

            try
            {
                if (File.Exists(xmlPath) && File.Exists(xsdPath))
                {
                    XmlSchemaSet schema = new XmlSchemaSet();

                    schema.Add(string.Empty, xsdPath);

                    XDocument xdoc = XDocument.Load(xmlPath);

                    xdoc.Validate(schema, ValidationEventHandler);

                    configManager = new ConfigManager(xmlPath, typeof(DataOptions));
                }
                else if (File.Exists(jsonPath))
                {
                    configManager = new ConfigManager(jsonPath, typeof(DataOptions));
                }
                else
                {
                    throw new Exception("File with data options was not found.");
                }

                dataOptions = configManager.GetOptions<DataOptions>();

                appInsights = new DataIO(dataOptions.LoggerConnectionString);

                appInsights.ClearInsights();

                appInsights.InsertInsight("Connection was successfully established");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exceptions.txt"), true))
                {
                    sw.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} Exception: {ex.Message}");
                }

                return;
            }

            try
            {
                DataManager service = new DataManager(dataOptions, appInsights);

                ServiceBase.Run(service);
            }
            catch (Exception ex)
            {
                appInsights.InsertInsight("EXCEPTION: " + ex.Message);

                appInsights.WriteInsightsToXml(dataOptions.OutputFolder);
            }
        }

        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (Enum.TryParse("Error", out XmlSeverityType type) && type == XmlSeverityType.Error)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
