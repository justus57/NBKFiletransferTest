

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
        /// <summary>
        /// justus kasyoki-4/03/2022
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string host = "172.16.19.24";
            string username = "Redcross@172.16.19.24";
            int port = 22;
            string password = "";
            string localFilePath = @"C:\Users\Admin2\Downloads\EncyptionTool\EncyptionTool\OutputFile";
          
            try
            {
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
                    }
                }
                SendPaymentFile(host, username, password, localFilePath, port);
            }
            catch (Exception es)
            {
                Logs.WriteLog(es.Message);
                Console.ReadLine();
                //Logs.WriteLog(es.InnerException.ToString());
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

        public static void SendPaymentFile(string host, string username, string password, string localFilePath, int port)
        {
            
            try
            {
                using (var client = new SftpClient(host, port, username, password))
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        Console.WriteLine("I'm connected to the client");

                        using (var fileStream = new FileStream(localFilePath, FileMode.Open))
                        {

                            client.BufferSize = 4 * 1024; // bypass Payload error large files
                            client.UploadFile(fileStream, Path.GetFileName(localFilePath));
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


