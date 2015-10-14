using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMVCAuthentication.Security
{
    public class RijndaelCryptoProvider : ICryptoProvider
    {
        protected const string initialVector = "sometestingvectr";//16 bytes
        protected const int KeySize = 128;//Вполне достаточно;

        protected RijndaelManaged rijndael;

        public RijndaelCryptoProvider(string Key)
        {
            rijndael = new RijndaelManaged();
            rijndael.KeySize = KeySize;

            if (Key.Length < KeySize/8)
            {
                //Дополним 
                Key = String.Format("{0}{1}", Key, new string('$', KeySize/8 - Key.Length));
            }

            if (Key.Length > KeySize/8)
            {
                Key = Key.Substring(0, KeySize/8);
            }

            rijndael.Key = Encoding.UTF8.GetBytes(Key);
            rijndael.IV = Encoding.UTF8.GetBytes(initialVector);
        }

        public string Encrypt<T>(T Object)
        {
            string serializedObject;

            //Сериализуем в JSON чтобы не возиться с ISerializable
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, Object);
                serializedObject = Encoding.UTF8.GetString(ms.ToArray());
            }
            byte[] encryptedObject;

            ICryptoTransform encryptor = rijndael.CreateEncryptor();

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(serializedObject);
                    }
                    encryptedObject = msEncrypt.ToArray();
                }
            }

            return Convert.ToBase64String(encryptedObject);
        }

        public T Decrypt<T>(string Data)
        {
            byte[] encryptedObject = Convert.FromBase64String(Data);
            string serializedObject;
            object deserializedObject;

            ICryptoTransform decryptor = rijndael.CreateDecryptor();

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(encryptedObject))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {

                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        serializedObject = srDecrypt.ReadToEnd();
                    }
                }
            }

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(serializedObject)))
            {
                deserializedObject = serializer.ReadObject(ms);                
            }

            if (deserializedObject is T)
            {
                return (T) deserializedObject;
            }
            else
            {
                return default(T);
            }
        }
    }
}
