using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
 public partial class Service1 : ServiceBase
    {
        Logger logger;
        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            ConfigManager configManager;
            FileInfo info;
            string xmlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "options.xml");
            string xsdConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "options.xsd");
            string jsonFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "options.json");

            if (File.Exists(xmlFile) && File.Exists(xsdConfigFile))
            {
                configManager = new ConfigManager(xmlFile, xsdConfigFile);
                info = new FileInfo(xmlFile);
            }
            else if (File.Exists(jsonFile))
            {
                configManager = new ConfigManager(jsonFile, string.Empty);
                info = new FileInfo(jsonFile);
            }
            else
            {
                throw new ArgumentNullException($"Configuration file was not found,sry");
            }

            Options op = configManager.ParseOptions();
            logger = new Logger(op, info);
            Thread loggerThread = new Thread(new ThreadStart(logger.Start));
            logger.Start();
        }

        protected override void OnStop()
        {
            logger.Stop();
            File.Delete("D:\\.NET\\templog.txt");
            Thread.Sleep(1000);
        }
    }


    class Logger
    {
        private readonly FileSystemWatcher watcher;
        private object obj = new object();
        private bool enabled = true;
        private readonly Options options;
        private readonly FileInfo fileInfo;
        public Logger(Options op, FileInfo info)
        {
            options = op;
            fileInfo = info;
            watcher = new FileSystemWatcher(op.SourcePath);
            watcher.Deleted += WatcherDeleted;
            watcher.Created += WatcherCreated;
            watcher.Changed += WatcherChanged;
            watcher.Renamed += WatcherRenamed;
            CreateInTarget();
        }


        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            while (enabled)
            {
                Thread.Sleep(1000);
            }
        }
        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }

        private void WatcherRenamed(object sender, RenamedEventArgs e)
        {
            string fileEvent = "renamed to " + e.FullPath;
            string filePath = e.OldFullPath;
            RecordEntry(fileEvent, filePath);
        }
        private void WatcherChanged(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "changed";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }
        private void WatcherCreated(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "created";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }
        private void WatcherDeleted(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "deleted";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }

        private void RecordEntry(string fileEvent, string filePath)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter(options.LoggerPath, true))
                {
                    writer.WriteLine(String.Format("{0} file {1} was {2}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
                    writer.Flush();
                }
            }
        }

        public void CreateInTarget()
        {
            string targetPath = options.TargetPath + @"\archives\" + DateTime.Now.ToString(@"yyyy") + @"\" +
                DateTime.Now.ToString(@"MM") + @"\" + DateTime.Now.ToString(@"dd");
            string sourceFile = options.SourcePath + @"\options" + fileInfo.Extension;
            if (options.NeedToEncrypt)
            {
                byte[] arr = File.ReadAllBytes(sourceFile);
                byte[] text = Crypt(arr);
                string encryptedFile = targetPath + @"\encrypted";
                File.WriteAllBytes(encryptedFile, text);
                if (options.ArchiveOptions)
                {
                    string compressedFile = encryptedFile.Substring(0, encryptedFile.Length - fileInfo.Extension.Length) + ".gz";
                    Compress(sourceFile, compressedFile);
                    string decompressed = targetPath + @"\decompressed" + fileInfo.Extension;
                    Decompress(compressedFile, decompressed);
                    File.Delete(encryptedFile);
                }
            }
            else
            {
                string copy = targetPath + @"\copy" + fileInfo.Extension;
                File.Move(sourceFile, copy);
                if (options.ArchiveOptions)
                {
                    string compressedFile = copy.Substring(0,copy.Length - fileInfo.Extension.Length) + ".gz";
                    Compress(sourceFile, compressedFile);
                    string decompressed = targetPath + @"\decompressed" + fileInfo.Extension;
                    Decompress(compressedFile, decompressed);
                    File.Delete(copy);
                }
            }

        }
        void Compress(string sourceFile, string compressedFile)
        {
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open))
            {
                using (FileStream targetStream = new FileStream(compressedFile, FileMode.OpenOrCreate))
                {
                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionLevel.Fastest))
                    {
                        sourceStream.CopyTo(compressionStream);
                    }
                }
            }
        }

        void Decompress(string compressedFile, string targetFile)
        {
            using (FileStream sourceStream = new FileStream(compressedFile, FileMode.Open))
            {
                using (FileStream targetStream = new FileStream(targetFile, FileMode.OpenOrCreate))
                {
                    using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(targetStream);
                    }
                }
            }
        }

        byte[] Crypt(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= 1;
            }
            return bytes;
        }
    }
}
