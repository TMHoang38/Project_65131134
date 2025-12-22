using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Project_65131134.Helpers
{
    public class PasswordResetHelper
    {
        private static string SecretKey = "TimepieceStore_65131134_SecretKey_2024";

        /// <summary>
        /// Tạo token reset mật khẩu
        /// </summary>
        public static string GenerateResetToken(string email)
        {
            string data = $"{email}|{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] keyBytes = Encoding.UTF8.GetBytes(SecretKey);

            using (HMACSHA256 hmac = new HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(dataBytes);
                string hash = Convert.ToBase64String(hashBytes);
                string token = Convert.ToBase64String(Encoding.UTF8.GetBytes(data)) + "." + hash;
                return HttpUtility.UrlEncode(token);
            }
        }

        /// <summary>
        /// Xác thực và giải mã token
        /// </summary>
        public static bool ValidateToken(string token, out string email, out DateTime? timestamp)
        {
            email = null;
            timestamp = null;

            try
            {
                token = HttpUtility.UrlDecode(token);
                string[] parts = token.Split('.');
                if (parts.Length != 2)
                {
                    return false;
                }

                string data = Encoding.UTF8.GetString(Convert.FromBase64String(parts[0]));
                string[] dataParts = data.Split('|');
                if (dataParts.Length != 2)
                {
                    return false;
                }

                email = dataParts[0];
                if (!DateTime.TryParse(dataParts[1], out DateTime tokenTime))
                {
                    return false;
                }

                // Kiểm tra token hết hạn (24 giờ)
                if (DateTime.Now.Subtract(tokenTime).TotalHours > 24)
                {
                    return false;
                }

                // Xác thực hash
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                byte[] keyBytes = Encoding.UTF8.GetBytes(SecretKey);

                using (HMACSHA256 hmac = new HMACSHA256(keyBytes))
                {
                    byte[] hashBytes = hmac.ComputeHash(dataBytes);
                    string expectedHash = Convert.ToBase64String(hashBytes);

                    if (parts[1] != expectedHash)
                    {
                        return false;
                    }
                }

                timestamp = tokenTime;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

