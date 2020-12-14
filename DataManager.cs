using System.ServiceProcess;
using ServiceLibrary;

namespace Lab4
{
    public partial class DataManager : ServiceBase
    {
        readonly DataIO appInsights;

        readonly DataOptions dataOptions;

        public DataManager(DataOptions dataOptions, DataIO appInsights)
        {
            InitializeComponent();

            this.dataOptions = dataOptions;

            this.appInsights = appInsights;
        }

        protected override void OnStart(string[] args)
        {
			//подключение к базе данных
            DataIO reader = new DataIO(dataOptions.ConnectionString);

            FileTransfer fileTransfer = new FileTransfer(dataOptions.OutputFolder, dataOptions.SourcePath);

            string customersFileName = "customers";

            reader.GetCustomers(dataOptions.OutputFolder, appInsights, customersFileName);

            fileTransfer.SendFileToFtp($"{customersFileName}.xml");
            fileTransfer.SendFileToFtp($"{customersFileName}.xsd");

            appInsights.InsertInsight("Files were sent to FTP successfully");
        }

        protected override void OnStop()
        {
            appInsights.InsertInsight("Service was successfully stopped");

            appInsights.WriteInsightsToXml(dataOptions.OutputFolder);
        }
    }
}
