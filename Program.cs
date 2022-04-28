﻿

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
using System.Net;
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
        /// <param name="args">For accepting the zero-indexed command line arguments. 
        /// args is the user-defined name. So you can change it by a valid identifier.
        /// [] must come before the args otherwise compiler will give errors.</param>
        static void Main(string[] args)
        {
            string host = Utility.GetConfigData("host");
            string username = Utility.GetConfigData("Username");
            int port = 22;
            string password = Utility.GetConfigData("Password");

            try
            {
                SendPaymentFile(host, username, password, port);
            }
            catch (Exception es)
            {
                Logs.WriteLog(es.Message);
            }
            Console.ReadLine();
        }


        ///<summary>
        /// justus kasyoki- 4/03/2022.
        ///
        /// </summary>
        /// <param name="host">this is the ip address to use</param>
        /// <param name="username">input for the current username</param>
        /// <param name="password">security code</param>
        /// <param name="port">default fstp port which is 22</param>
        public static void SendPaymentFile(string host, string username, string password, int port)
        {
            try
            {
                var ftpClient = new WebClient();
                ftpClient.Headers.Add(HttpRequestHeader.ContentType, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ftpClient.Credentials = new NetworkCredential(username, password);
                using (var client = new SftpClient(host, port, username, password))
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        Console.WriteLine("I'm connected to the client");

                        string[] filePaths = Directory.GetFiles(localFilePath, "*.xlsx");
                        
                        List<string> lst = filePaths.ToList();

                        foreach (var element in lst)
                        {
                            var filename = Path.GetFileName(element);

                            File.Copy(element, destinationpath + filename);
                            
                            using (var fileStream = new FileStream(element, FileMode.Open,FileAccess.Read))
                            {
                                client.BufferSize = 4 * 1024; // bypass Payload error large files
                                client.GetType();
                               
                                client.UploadFile(fileStream, Path.GetFileName(element));
                               
                                Console.WriteLine("File Uploaded successfully!");
                            }
                            File.Delete(element);
                        }
                       
                    }
                    else
                    {
                        Console.WriteLine("I couldn't connect");
                        Logs.WriteLog("I couldn't connect");
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


