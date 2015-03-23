using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsTraining
{
    public class PasswordCryptography
    {
        private const string HashPrefix = "#¶µ•#‡";

	public bool IsPasswordHash(string text)
	{
	    return !string.IsNullOrEmpty(text) && text.StartsWith(HashPrefix) && text.Length == 50;
	}

        public string GenerateHash(string input)
        {
            var encoding = new UnicodeEncoding();
            var passInBytes = encoding.GetBytes(input);
            var sha = new SHA256Managed();
            var hashInBytes = sha.ComputeHash(passInBytes);
            return string.Format("{0}{1}", HashPrefix, Convert.ToBase64String(hashInBytes));
        }

    }
}
