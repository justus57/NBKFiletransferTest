

using Renci.SshNet;
using System;
using System.IO;
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
            string host = "localhost";
            string username = "localhost";
            int port = 22;
            string password = "";
            string remoteFilePath = @"C:\Users\Admin2\Downloads\Config.xml";
            string localFilePath = @"C:\Users\Admin2\Downloads\Config.xml";
            try
            {

                encryption(localFilePath, remoteFilePath);
                SendPaymentFile(host, username, password, remoteFilePath, localFilePath, port);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        ///<summary>
        /// justus kasyoki- 4/03/2022.
        ///
        /// Encrypts a file using Rijndael algorithm.
        ///</summary>
        ///<param name="inputFile"></param>
        ///<param name="outputFile"></param>

        private static void encryption(string localFilePath, string remoteFilePath)
        {
            try
            {
                string password = AppDomain.CurrentDomain.BaseDirectory + @"myKey123"; // Your Key Here
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = remoteFilePath;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(localFilePath, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);


                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch(Exception es)
            {
                Console.WriteLine("Encryption failed!", "Error");
                Console.WriteLine(es.Message);
                Console.ReadLine();
            }
        }


        //Parameters for sending Payment File
        public static void SendPaymentFile(string host, string username, string password, string remoteFilePath, string localFilePath, int port)
        {
            //try
            //{
            //    using (ScpClient client = new ScpClient(host, username, port))
            //    {
            //        client.Connect();

            //        using (System.IO.Stream localFile = File.OpenRead(localFilePath))
            //        {
            //            client.Upload(localFile, remoteFilePath);
            //        }

            //    }
            //    Console.WriteLine("Successfully connected to the client");
            //    Console.ReadLine();
            //}
            //catch (Exception es)
            //{
            //    WriteLog(es.Message);
            //}
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
                WriteLog(es.Message);
            }
        }
        public static void WriteLog(string text)
        {

            try
            {
                //set up a filestream
                string strPath = @"C:\Logs\NBKFiletransfer";
                string fileName = DateTime.Now.ToString("MMddyyyy") + "_logs.txt";
                string filenamePath = strPath + '\\' + fileName;
                Directory.CreateDirectory(strPath);
                FileStream fs = new FileStream(filenamePath, FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                //set up a streamwriter for adding text
                StreamWriter sw = new StreamWriter(fs);
                //find the end of the underlying filestream
                sw.BaseStream.Seek(0, SeekOrigin.End);
                //add the text
                sw.WriteLine(DateTime.Now.ToString() + " : " + text);
                //add the text to the underlying filestream
                sw.Flush();
                //close the writer
                sw.Close();
            }
            catch (Exception ex)
            {
                //throw;
                ex.Data.Clear();
            }
        }
    }
 
}
