using System;
using System.ServiceProcess;
using System.Threading;
using System.IO;
using System.Text;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Threading.Tasks;
using MyServiceLibrary;

namespace Lab4
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            string dataOptionsXml = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dataoptions.xml");
            string dataOpConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dataoptions.xsd");
            string XmlOp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "options.xml");
            string XmlConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "options.xsd");

            DataParser parser = new DataParser(dataOptionsXml, dataOpConfig);
            var DataOp = parser.ParseOptions();
            DataIO dataIO = new DataIO(DataOp);
            dataIO.GenerateToXml();

            ConfigManager configManager = new ConfigManager(XmlOp, XmlConfig);
            Options op = configManager.ParseOptions();

            if(File.Exists(DataOp.BuffDirectory + "BasicInfo.xml"))
            {
                if(!File.Exists(DataOp.SourceDirectory + "BasicInfo.xml"))
                {
                    File.Create(DataOp.SourceDirectory + "BasicInfo.xml");
                    File.Move(DataOp.BuffDirectory + "BasicInfo.xml", DataOp.SourceDirectory + "BasicInfo.xml");
                }
                else
                {
                    File.Move(DataOp.BuffDirectory + "BasicInfo.xml", DataOp.SourceDirectory + "BasicInfo.xml");
                }

                if(op.NeedToEncrypt)
                {
                    byte[] arr = File.ReadAllBytes(DataOp.SourceDirectory + "BasicInfo.xml");
                    byte[] text = Crypt(arr);
                    string encryptedFile = op.TargetPath + @"\encrypted.txt";
                    File.WriteAllBytes(encryptedFile, text);
                    if(op.ArchiveOptions)
                    {
                        string compressedFile = encryptedFile.Substring(0, encryptedFile.Length - 4) + ".gz";
                        Compress(DataOp.SourceDirectory + "BasicInfo.xml", compressedFile);
                        string decompressed = op.TargetPath + @"\decompressed.xml";
                        Decompress(compressedFile, decompressed);
                        File.Delete(encryptedFile);
                    }
                    else
                    {
                        string copy = op.TargetPath + @"\copy.xml";
                        File.Move(DataOp.SourceDirectory + "BasicInfo.xml", copy);
                        if(op.ArchiveOptions)
                        {
                            string compressedFile = copy.Substring(0, copy.Length - 4) + ".gz";
                            Compress(DataOp.SourceDirectory + "BasicInfo.xml", compressedFile);
                            string decompressed = op.TargetPath + @"\decompressed.xml";
                            Decompress(compressedFile, decompressed);
                            File.Delete(copy);
                        }
                    }
                }
            }
        }

        protected override void OnStop()
        {
            Thread.Sleep(1000);
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
