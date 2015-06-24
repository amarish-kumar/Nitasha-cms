using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace NITASA.Areas.Admin.Helper
{
    public class CryptoUtility
    {
        /// <summary>
        /// </summary>
        /// <param name="password">Password to generate hash</param>
        /// <param name="salt">salt,non null , minimum 8 character long</param>
        /// <returns>Password hash using pbkdf2(Rfc2898DeriveBytes)</returns>
        public static string GetPasswordHash(string password, int salt)
        {
            byte[] bytes = UTF32Encoding.UTF32.GetBytes(salt.ToString());
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, bytes, 64000);
            byte[] passwordHashBytes = deriveBytes.GetBytes(50);
            string passwordHash = Convert.ToBase64String(passwordHashBytes);
            return passwordHash;
        }

        /// <summary>
        /// To generate salt. salt will be 32 hexadecimal character. generated using guid
        /// </summary>
        /// <returns>return 32 digit hexadecimal strimg</returns>
        public static int GetNewSalt()
        {
            //return Guid.NewGuid().ToString().Replace("-", "");
            Random ran = new Random();
            int number = ran.Next(1000, 9999);
            ran.Next();
            return number;
        }
        /// <summary>
        /// To generate salt. salt will be 32 hexadecimal character. generated using guid
        /// </summary>
        /// <returns>return 32 digit hexadecimal strimg</returns>
        public static string GetNewPassword()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 5);
        }
    }
}