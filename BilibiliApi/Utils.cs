using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BilibiliApi
{
	public static class Utils
	{
		public static string Md5(string str)
		{
			using MD5 md5 = new MD5CryptoServiceProvider();
			var data = Encoding.UTF8.GetBytes(str);
			var md5Data = md5.ComputeHash(data);
			return md5Data.Aggregate(string.Empty, (current, t) => current + t.ToString(@"x2").PadLeft(2, '0'));
		}

		private static RSA ReadKey(string pemContents)
		{
			const string header = @"-----BEGIN PUBLIC KEY-----";
			const string footer = @"-----END PUBLIC KEY-----";

			if (pemContents.StartsWith(header))
			{
				var endIdx = pemContents.IndexOf(footer, header.Length, StringComparison.Ordinal);
				var base64 = pemContents.Substring(header.Length, endIdx - header.Length);

				var der = Convert.FromBase64String(base64);
				var rsa = RSA.Create();
				rsa.ImportSubjectPublicKeyInfo(der, out _);
				return rsa;
			}

			// "BEGIN PRIVATE KEY" (ImportPkcs8PrivateKey),
			// "BEGIN ENCRYPTED PRIVATE KEY" (ImportEncryptedPkcs8PrivateKey),
			// "BEGIN PUBLIC KEY" (ImportSubjectPublicKeyInfo),
			// "BEGIN RSA PUBLIC KEY" (ImportRSAPublicKey)
			// could any/all be handled here.
			throw new InvalidOperationException();
		}

		public static string RsaEncrypt(string publicKey, string str)
		{
			using var rsa = ReadKey(publicKey);
			var cipherBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(str), RSAEncryptionPadding.Pkcs1);
			return Convert.ToBase64String(cipherBytes);
		}
	}
}
