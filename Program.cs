

using Grpc.Core;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace NBKFiletransferTest
{
    class Program
    {
        public static string element { get; private set; }
        static string path = AppDomain.CurrentDomain.BaseDirectory + @"\Config.xml";
        static string localFilePath = Path.GetFullPath(Utility.GetConfigData("localFilePath"));
        static string destinationpath = Path.GetFullPath(Utility.GetConfigData("destinationpath"));
        static string inputfileEncryptor = Path.GetFullPath(Utility.GetConfigData("inputfileEncryptor"));
        static string OriginalfilePathFromNav = Path.GetFullPath(Utility.GetConfigData("OriginalfilePathFromNav"));
        static string backupdirectory = Path.GetFullPath(Utility.GetConfigData("backupdirectory"));

        //public static string destinationpath { get; private set; }

        /// <summary>
        /// justus kasyoki-4/03/2022
        /// the main class the entry point a C# program from where the execution starts. 
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string host = Utility.GetConfigData("host");
            string username = Utility.GetConfigData("Username");
            int port = 22;
            string password = Utility.GetConfigData("Password");

            try
            {
                try
                {
                    string[] filePathFromNav = Directory.GetFiles(OriginalfilePathFromNav, "*.xlsx");
                    List<string> file1 = filePathFromNav.ToList();
                    foreach (var source in file1)
                    {
                        var filename = Path.GetFileName(source);
                        File.Copy(source, backupdirectory + filename);


                    }
                    List<string> file = filePathFromNav.ToList();
                    foreach (var source in file)
                    {
                        var filename = Path.GetFileName(source);
                        File.Copy(source, inputfileEncryptor + filename);
                        File.Delete(source);
                    }
                }
                catch (Exception es)
                {
                    Logs.WriteLog(es.Message);
                }

                    Process cmd = new Process();

                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.UseShellExecute = false;

                cmd.Start();

                using (StreamWriter sw = cmd.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.WriteLine("cd ..");
                        sw.WriteLine("cd ..");
                        sw.WriteLine("cd ..");
                        sw.WriteLine("cd ..");
                        sw.WriteLine("cd ..");
                        sw.WriteLine(@"cd Downloads\EncyptionTool\EncyptionTool");
                       // sw.WriteLine("java -jar KeyGeneratorForEncryption.jar");
                        sw.WriteLine("java -jar FileEncryptor.jar");
                        Logs.WriteLog("Encryption successful!");
                    }
                }
                SendPaymentFile(host, username, password, port);
            }
            catch (Exception es)
            {
                Logs.WriteLog(es.Message);
                Console.ReadLine();
               Logs.WriteLog(es.InnerException.ToString());
            }
            Console.ReadLine();
        }

        ///<summary>
        /// justus kasyoki- 4/03/2022.
        ///
        /// Encrypts a file using Rijndael algorithm.
        ///</summary>
        ///<param name="inputFile"></param>
        ///<param name="outputFile"></param>

        public static void SendPaymentFile(string host, string username, string password, int port)
        {
            try
            {
                using (var client = new SftpClient(host, port, username, password))
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        Console.WriteLine("**********************************");
                        Console.WriteLine("I'm connected to the client");
                        string[] filePaths = Directory.GetFiles(localFilePath, "*.xlsx");
                        
                        List<string> lst = filePaths.ToList();
                        foreach (var element in lst)
                        {
                            var filename = Path.GetFileName(element);
                            File.Copy(element, destinationpath + filename);
                            using (var fileStream = new FileStream(element, FileMode.Open))
                            {
                                client.BufferSize = 4 * 1024; // bypass Payload error large files
                                client.UploadFile(fileStream, Path.GetFileName(element));
                                Console.WriteLine("**********************************************");
                                Console.WriteLine("File Uploaded successfully!");
                            }
                            File.Delete(element);
                        }
                       
                    }
                    else
                    {
                        Console.WriteLine("I couldn't connect");
                    }
                }
            }
            catch (Exception es)
            {
               Logs.WriteLog(es.Message);
            }
        }
     
    }
}  


