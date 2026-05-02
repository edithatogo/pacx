using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Greg.Xrm.Command.Services.Encryption
{
	public static class AesEncryption
	{
		private static readonly string KeyFilePath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			"Greg.Xrm.Command",
			"encryption.key");

		private static byte[] GetOrCreateKey()
		{
			if (OperatingSystem.IsWindows())
			{
				if (File.Exists(KeyFilePath))
				{
					var protectedKey = File.ReadAllText(KeyFilePath);
					var protectedBytes = Convert.FromBase64String(protectedKey);
					return ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser);
				}

				var key = RandomNumberGenerator.GetBytes(32);
				var protectedKeyBytes = ProtectedData.Protect(key, null, DataProtectionScope.CurrentUser);
				Directory.CreateDirectory(Path.GetDirectoryName(KeyFilePath)!);
				File.WriteAllText(KeyFilePath, Convert.ToBase64String(protectedKeyBytes));
				return key;
			}

			return FallbackGetOrCreateKey();
		}

		private static byte[] FallbackGetOrCreateKey()
		{
			if (File.Exists(KeyFilePath))
				return File.ReadAllBytes(KeyFilePath);

			var key = RandomNumberGenerator.GetBytes(32);
			Directory.CreateDirectory(Path.GetDirectoryName(KeyFilePath)!);
			File.WriteAllBytes(KeyFilePath, key);
			return key;
		}

		public static string Encrypt(string plaintext)
		{
			var key = GetOrCreateKey();
			using var aes = Aes.Create();
			aes.Key = key;
			aes.GenerateIV();

			var iv = aes.IV;
			using var encryptor = aes.CreateEncryptor();
			var plainBytes = Encoding.UTF8.GetBytes(plaintext);
			var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

			var combined = new byte[iv.Length + encryptedBytes.Length];
			Buffer.BlockCopy(iv, 0, combined, 0, iv.Length);
			Buffer.BlockCopy(encryptedBytes, 0, combined, iv.Length, encryptedBytes.Length);

			return Convert.ToBase64String(combined);
		}

		public static string Decrypt(string ciphertext)
		{
			var key = GetOrCreateKey();
			var combined = Convert.FromBase64String(ciphertext);

			using var aes = Aes.Create();
			aes.Key = key;

			var iv = new byte[aes.BlockSize / 8];
			Buffer.BlockCopy(combined, 0, iv, 0, iv.Length);
			aes.IV = iv;

			var encryptedBytes = new byte[combined.Length - iv.Length];
			Buffer.BlockCopy(combined, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

			using var decryptor = aes.CreateDecryptor();
			var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

			return Encoding.UTF8.GetString(decryptedBytes);
		}
	}
}
