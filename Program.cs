﻿

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
       
        static string localFilePath = Path.GetFullPath(@"C:\Users\Admin2\Downloads\EncyptionTool\EncyptionTool\OutputFile");
        static string destinationpath = Path.GetFullPath(@"C:\Users\Admin2\Downloads\EncyptionTool\filesendToNBK");
        static string backupdirectory = @"C:\Backup";


        //public static string destinationpath { get; private set; }

        /// <summary>
        /// justus kasyoki-4/03/2022
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string host = "172.16.19.24";
            string username = "Redcross";
            int port = 22;
            string password = "cross2022_test";


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

                        Console.WriteLine("I'm connected to the client");
                        string[] filePaths = Directory.GetFiles(localFilePath, "*.txt");
                        
                        List<string> lst = filePaths.ToList();
                        foreach (var element in lst)
                        {
                            File.Copy(element, destinationpath);
                            using (var fileStream = new FileStream(element, FileMode.Open))
                            {
                                client.BufferSize = 4 * 1024; // bypass Payload error large files
                                client.UploadFile(fileStream, Path.GetFileName(element));

                            }
                        }
                        //try
                        //{
                        //    if (Directory.Exists(localFilePath))
                        //    {
                        //        if (Directory.Exists(destinationpath))
                        //        {
                        //            //Directory.Delete(destinationdirectory);
                        //            //Directory.Move(destinationpath, backupdirectory + DateTime.Now.ToString("_MMMdd_yyyy_HHmmss"));
                        //            Directory.Move(localFilePath, destinationpath);
                        //        }
                        //        else
                        //        {
                        //            Directory.Move(localFilePath, destinationpath);
                        //        }
                        //    }

                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine(ex.Message);
                        //}


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


