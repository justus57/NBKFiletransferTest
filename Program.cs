

using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;
using Renci.SshNet;
using System;
using System.Collections.Generic;
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
            string host = "localhost";
            string username = "localhost";
            int port = 22;
            string password = "";
            string remoteFilePath = @"C:\Users\Admin2\Downloads\Config.xml";
            string localFilePath = @"C:\Users\Admin2\Downloads\Config.xml";
     
            try
            {
             var paymentfilelocation = @"C:\Users\Admin2\Downloads\New folder\";
                string[] filePaths = Directory.GetFiles(paymentfilelocation, "*.txt");
                List<string> lst = filePaths.ToList();
                foreach (var element in lst)
                {
                    //Console.WriteLine(element);

                    string fileName1 = element;
                    string fileName = Path.GetFileName(fileName1);
                    var fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read);

                    Logs.WriteLog("Encrypting file..");
                    // encrypt the data using gpg

                    PGPEncryptDecrypt pgp = new PGPEncryptDecrypt();
                    //full path to file to encrypt
                    string origfilePath = fileName;
                    string origFilePath = Path.GetFileName(origfilePath);
                    //folder to store encrypted file
                    string encryptedFilePath = @"C:\Users\Admin2\Downloads\New folder\";
                    //folder to store unencrypted file
                    //   string unencryptedFilePath = GetConfigData("unencryptedFilePath");
                    //path to public key file 
                    string publicKeyFile =@"C:\Users\Admin2\Downloads\REDCROSS.asc";
                    //string publicKeyFile = Path.GetFileName(publicKeyFilepath);
                    //path to private key file (this file should be kept at client, AND in a secure place, far from prying eyes and tinkering hands)
                    // string privateKeyFile = GetConfigData("privateKeyFile");
                    //string privateKeyFile = Path.GetFileName(privateKeyFilepath);
                    pgp.Encrypt(origFilePath, publicKeyFile, encryptedFilePath);
                    // pgp.Decrypt(encryptedFilePath + "credentials.txt.asc", privateKeyFile, passPhrase, unencryptedFilePath);
                    var directory = @"C:\Users\Admin2\Downloads\New folder\";
                    var dir = new DirectoryInfo(directory);
                    var lastModified = dir.GetFiles().OrderByDescending(fi => fi.LastWriteTime).First();
                    var fullfilename = lastModified.FullName;
                    //var encryptedFile = @"C:\Users\Admin2\Downloads\New folder\94424.txt.asc";
                    var encryptedFile = Convert.ToString(fullfilename);

                    SendPaymentFile(host, username, password, remoteFilePath, localFilePath, port);

                }

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

        //private static void encryption()
        //{
        //    // encryption key for encryption/decryption 
        //    byte[] key = { 0x02, 0x03, 0x01, 0x03, 0x03, 0x07, 0x07, 0x08, 0x09, 0x09, 0x11, 0x11, 0x16, 0x17, 0x19, 0x16 };

        //    // ENCRYPT DATA
        //    try
        //    {
        //        // create file stream
        //        FileStream myStream = new FileStream(@"C:\Users\Admin2\Downloads\New folder\samplepaymentfile.txt.asc", FileMode.OpenOrCreate);

        //        // configure encryption key.  
        //        Aes aes = Aes.Create();
        //        aes.Key = key;

        //        // store IV
        //        byte[] iv = aes.IV;
        //        myStream.Write(iv, 0, iv.Length);

        //        // encrypt filestream  
        //         CryptoStream cryptStream = new CryptoStream(
        //            myStream,
        //            aes.CreateEncryptor(),
        //            CryptoStreamMode.Write);

        //        // write to filestream
        //         StreamWriter sWriter = new StreamWriter(cryptStream);
        //        string plainText = "Welcome to the lab of MrNetTek!";
        //        sWriter.WriteLine(plainText);

        //        // done 
        //        Console.WriteLine("---SUCCESSFUL ENCRYPTION---\n");

        //    }
        //    catch
        //    {
        //        // error  
        //        Console.WriteLine("---ENCRYPTION FAILED---");
        //        Console.ReadLine();
        //        throw;
        //    }

        //    // SHOW ENCRYPTED DATA
        //    try
        //    {
        //        string text = System.IO.File.ReadAllText(@"C:\Users\Admin2\Downloads\Config.xml");

        //        // encrypted data
        //        Console.WriteLine("Encrypted Data: {0}\n\n", text);

        //        Console.WriteLine("Press any key to view decrypted data\n");
        //        Console.ReadKey();
        //    }
        //    catch(Exception es)
        //    {
        //        Console.WriteLine(es.Message);
        //        throw;
        //    }
        //}


        //Parameters for sending Payment File
        public static void SendPaymentFile(string host, string username, string password, string remoteFilePath, string localFilePath, int port)
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
    /// <summary>
    /// justus kasyoki 4/03/2022
    /// 
    /// encryption process
    /// </summary>
    public class PGPEncryptDecrypt
    {

        public PGPEncryptDecrypt()
        {

        }

        /**
        * A simple routine that opens a key ring file and loads the first available key suitable for
        * encryption.
        *
        * @param in
        * @return
        * @m_out
        * @
        */
        private static PgpPublicKey ReadPublicKey(Stream inputStream)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);
            PgpPublicKeyRingBundle pgpPub = new PgpPublicKeyRingBundle(inputStream);
            //
            // we just loop through the collection till we find a key suitable for encryption, in the real
            // world you would probably want to be a bit smarter about this.
            // iterate through the key rings.
            //
            foreach (PgpPublicKeyRing kRing in pgpPub.GetKeyRings())
            {
                foreach (PgpPublicKey k in kRing.GetPublicKeys())
                {
                    if (k.IsEncryptionKey)
                    {
                        return k;
                    }
                }
            }
            throw new ArgumentException("Can't find encryption key in key ring.");
        }

        /**
        * Search a secret key ring collection for a secret key corresponding to
        * keyId if it exists.
        *
        * @param pgpSec a secret key ring collection.
        * @param keyId keyId we want.
        * @param pass passphrase to decrypt secret key with.
        * @return
        */
        private static PgpPrivateKey FindSecretKey(PgpSecretKeyRingBundle pgpSec, long keyId, char[] pass)
        {
            PgpSecretKey pgpSecKey = pgpSec.GetSecretKey(keyId);
            if (pgpSecKey == null)
            {
                return null;
            }
            return pgpSecKey.ExtractPrivateKey(pass);
        }

        /**
        * decrypt the passed in message stream
        */
        private static void DecryptFile(Stream inputStream, Stream keyIn, char[] passwd, string defaultFileName, string pathToSaveFile)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);
            try
            {
                PgpObjectFactory pgpF = new PgpObjectFactory(inputStream);
                PgpEncryptedDataList enc;
                PgpObject o = pgpF.NextPgpObject();
                //
                // the first object might be a PGP marker packet.
                //
                if (o is PgpEncryptedDataList)
                {
                    enc = (PgpEncryptedDataList)o;
                }
                else
                {
                    enc = (PgpEncryptedDataList)pgpF.NextPgpObject();
                }
                //
                // find the secret key
                //
                PgpPrivateKey sKey = null;
                PgpPublicKeyEncryptedData pbe = null;
                PgpSecretKeyRingBundle pgpSec = new PgpSecretKeyRingBundle(
                PgpUtilities.GetDecoderStream(keyIn));
                foreach (PgpPublicKeyEncryptedData pked in enc.GetEncryptedDataObjects())
                {
                    sKey = FindSecretKey(pgpSec, pked.KeyId, passwd);
                    if (sKey != null)
                    {
                        pbe = pked;
                        break;
                    }
                }
                if (sKey == null)
                {
                    throw new ArgumentException("secret key for message not found.");
                }
                Stream clear = pbe.GetDataStream(sKey);
                PgpObjectFactory plainFact = new PgpObjectFactory(clear);
                PgpObject message = plainFact.NextPgpObject();
                if (message is PgpCompressedData)
                {
                    PgpCompressedData cData = (PgpCompressedData)message;
                    PgpObjectFactory pgpFact = new PgpObjectFactory(cData.GetDataStream());
                    message = pgpFact.NextPgpObject();
                }

                if (message is PgpLiteralData)
                {

                    PgpLiteralData ld = (PgpLiteralData)message;
                    string outFileName = ld.FileName;
                    if (outFileName.Length == 0)
                    {
                        outFileName = defaultFileName;
                    }

                    Stream fOut = File.Create(pathToSaveFile + outFileName);
                    Stream unc = ld.GetInputStream();
                    Streams.PipeAll(unc, fOut);
                    fOut.Close();
                }
                else if (message is PgpOnePassSignatureList)
                {
                    throw new PgpException("encrypted message contains a signed message - not literal data.");
                }
                else
                {
                    throw new PgpException("message is not a simple encrypted file - type unknown.");
                }
                if (pbe.IsIntegrityProtected())
                {
                    if (!pbe.Verify())
                    {
                        Logs.WriteLog("message failed integrity check");
                    }
                    else
                    {
                        Logs.WriteLog("message integrity check passed");
                    }
                }
                else
                {
                    Logs.WriteLog("no message integrity check");
                }
            }
            catch (PgpException es)
            {
                Logs.WriteLog(es.Message);
                string innerEx = "";
                if (es.InnerException != null)
                    innerEx = es.InnerException.ToString();
            }
        }

        private static void EncryptFile(Stream outputStream, string fileName, PgpPublicKey encKey, bool armor, bool withIntegrityCheck)
        {

            if (armor)
            {
                outputStream = new ArmoredOutputStream(outputStream);
            }
            try
            {
                MemoryStream bOut = new MemoryStream();
                PgpCompressedDataGenerator comData = new PgpCompressedDataGenerator(
                CompressionAlgorithmTag.Zip);
                PgpUtilities.WriteFileToLiteralData(
                comData.Open(bOut),
                PgpLiteralData.Binary,
                new FileInfo(fileName));
                comData.Close();
                PgpEncryptedDataGenerator cPk = new PgpEncryptedDataGenerator(
                SymmetricKeyAlgorithmTag.Cast5, withIntegrityCheck, new SecureRandom());
                cPk.AddMethod(encKey);
                byte[] bytes = bOut.ToArray();
                Stream cOut = cPk.Open(outputStream, bytes.Length);
                cOut.Write(bytes, 0, bytes.Length);
                cOut.Close();
                if (armor)
                {
                    outputStream.Close();
                }
            }
            catch (PgpException es)
            {
               Logs.WriteLog(es.Message);
                string innerEx = "";
                if (es.InnerException != null)
                    innerEx = es.InnerException.ToString();

            }
        }
        public void Encrypt(string filePath, string publicKeyFile, string pathToSaveFile)
        {
            Stream keyIn, fos;
            keyIn = File.OpenRead(publicKeyFile);
            string[] fileSplit = filePath.Split('\\');
            string fileName = fileSplit[fileSplit.Length - 1];
            fos = File.Create(pathToSaveFile + fileName + ".asc");
            EncryptFile(fos, filePath, ReadPublicKey(keyIn), true, true);
            keyIn.Close();
            fos.Close();
        }
        public void Decrypt(string filePath, string privateKeyFile, string passPhrase, string pathToSaveFile)
        {
            Stream fin = File.OpenRead(filePath);
            Stream keyIn = File.OpenRead(privateKeyFile);
            DecryptFile(fin, keyIn, passPhrase.ToCharArray(), new FileInfo(filePath).Name + ".out", pathToSaveFile);
            fin.Close();
            keyIn.Close();
        }
    }

}
