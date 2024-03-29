﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CryptoLib
{
    public static class CryptoUtils
    {
        public const string version = "Crypto_Utils - Ver 1.1 del 22/01/2018";
        // https://msdn.microsoft.com/it-it/library/system.security.cryptography.symmetricalgorithm(v=vs.110).aspx
        // https://msdn.microsoft.com/it-it/library/system.security.cryptography.aes(v=vs.110).aspx
        // https://rushfrisby.com/c-cryptography-library-md5-sha1-sha2-aes-3des/

        #region MD5

        public static string HashMD5(string phrase)
        {
            if (phrase == null)
                return null;
            var encoder = new UTF8Encoding();
            var md5Hasher = new MD5CryptoServiceProvider();
            var hashedDataBytes = md5Hasher.ComputeHash(encoder.GetBytes(phrase));
            return ByteArrayToHexString(hashedDataBytes);
        }

        #endregion

        #region SHA

        public static string HashSHA1(string phrase)
        {
            if (phrase == null)
                return null;
            var encoder = new UTF8Encoding();
            var sha1Hasher = new SHA1CryptoServiceProvider();
            var hashedDataBytes = sha1Hasher.ComputeHash(encoder.GetBytes(phrase));
            return ByteArrayToHexString(hashedDataBytes);
        }

        public static string HashSHA256(string phrase)
        {
            if (phrase == null)
                return null;
            var encoder = new UTF8Encoding();
            var sha256Hasher = new SHA256CryptoServiceProvider();
            var hashedDataBytes = sha256Hasher.ComputeHash(encoder.GetBytes(phrase));
            return ByteArrayToHexString(hashedDataBytes);
        }

        public static string HashSHA384(string phrase)
        {
            if (phrase == null)
                return null;
            var encoder = new UTF8Encoding();
            var sha384Hasher = new SHA384CryptoServiceProvider();
            var hashedDataBytes = sha384Hasher.ComputeHash(encoder.GetBytes(phrase));
            return ByteArrayToHexString(hashedDataBytes);
        }

        public static string HashSHA512(string phrase)
        {
            if (phrase == null)
                return null;
            var encoder = new UTF8Encoding();
            var sha512Hasher = new SHA512CryptoServiceProvider();
            var hashedDataBytes = sha512Hasher.ComputeHash(encoder.GetBytes(phrase));
            return ByteArrayToHexString(hashedDataBytes);
        }

        #endregion

        #region AES

        public static string EncryptAES(string phrase, string key, bool hashKey = true)
        {
            if (phrase == null || key == null)
                return null;

            var keyArray = HexStringToByteArray(hashKey ? HashMD5(key) : key);
            var toEncryptArray = Encoding.UTF8.GetBytes(phrase);
            byte[] result;

            using (var aes = new AesCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            })
            {
                var cTransform = aes.CreateEncryptor();
                result = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                aes.Clear();
            }
            return ByteArrayToHexString(result);
        }

        public static string DecryptAES(string hash, string key, bool hashKey = true)
        {
            if (hash == null || key == null)
                return null;

            var keyArray = HexStringToByteArray(hashKey ? HashMD5(key) : key);
            var toEncryptArray = HexStringToByteArray(hash);

            var aes = new AesCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = aes.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            aes.Clear();
            return Encoding.UTF8.GetString(resultArray);
        }

        #endregion

        #region 3DES

        public static string EncryptTripleDES(string phrase, string key, bool hashKey = true)
        {
            var keyArray = HexStringToByteArray(hashKey ? HashMD5(key) : key);
            var toEncryptArray = Encoding.UTF8.GetBytes(phrase);

            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = tdes.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            return ByteArrayToHexString(resultArray);
        }

        public static string DecryptTripleDES(string hash, string key, bool hashKey = true)
        {
            var keyArray = HexStringToByteArray(hashKey ? HashMD5(key) : key);
            var toEncryptArray = HexStringToByteArray(hash);

            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = tdes.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            return Encoding.UTF8.GetString(resultArray);
        }

        #endregion

        #region Helpers

        internal static string ByteArrayToHexString(byte[] inputArray)
        {
            if (inputArray == null)
                return null;
            var o = new StringBuilder("");
            for (var i = 0; i < inputArray.Length; i++)
                o.Append(inputArray[i].ToString("X2"));
            return o.ToString();
        }

        internal static byte[] HexStringToByteArray(string inputString)
        {
            if (inputString == null)
                return null;

            if (inputString.Length == 0)
                return new byte[0];

            if (inputString.Length % 2 != 0)
                throw new Exception("Hex strings have an even number of characters and you have got an odd number of characters!");

            var num = inputString.Length / 2;
            var bytes = new byte[num];
            for (var i = 0; i < num; i++)
            {
                var x = inputString.Substring(i * 2, 2);
                try
                {
                    bytes[i] = Convert.ToByte(x, 16);
                }
                catch (Exception ex)
                {
                    throw new Exception("Part of your \"hex\" string contains a non-hex value.", ex);
                }
            }
            return bytes;
        }

        public static string Base64_Encode(string s)
        {
            var sBytes = Encoding.UTF8.GetBytes(s);
            return Convert.ToBase64String(sBytes);
        }

        public static string Base64_Decode(string s)
        {
            var sBytes = Convert.FromBase64String(s);
            return Encoding.UTF8.GetString(sBytes);
        }

        #endregion

        #region RSA

        public static string EncryptRSA(string plaintext, string pubXML_key)
        {
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    if (string.IsNullOrEmpty(plaintext))
                        throw new Exception("Null text");
                    if (string.IsNullOrEmpty(pubXML_key))
                        throw new Exception("Null key");

                    rsa.FromXmlString(pubXML_key);
                    return Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(plaintext), true));
                }
                catch(Exception e)
                {
                    return "Exception on RSA Encryption: " + e.Message;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        public static string DecryptRSA(string ciphertext, string privXML_key)
        {
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    if (string.IsNullOrEmpty(ciphertext))
                        throw new Exception("Null text");
                    if (string.IsNullOrEmpty(privXML_key))
                        throw new Exception("Null key");

                    rsa.FromXmlString(privXML_key);
                    return Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(ciphertext), true));
                }
                catch(Exception e)
                {
                    return "Exception on RSA Decryption: " + e.Message; ;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
        #endregion
    }
}
