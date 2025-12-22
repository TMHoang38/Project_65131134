using System;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;

namespace Project_65131134.Helpers
{
    public class EmailHelper
    {
        private static string GetSmtpHost()
        {
            return WebConfigurationManager.AppSettings["SmtpHost"] ?? "smtp.gmail.com";
        }

        private static int GetSmtpPort()
        {
            int port = 587;
            if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["SmtpPort"]))
            {
                int.TryParse(WebConfigurationManager.AppSettings["SmtpPort"], out port);
            }
            return port;
        }

        private static string GetSmtpEmail()
        {
            return WebConfigurationManager.AppSettings["SmtpEmail"] ?? "";
        }

        private static string GetSmtpPassword()
        {
            return WebConfigurationManager.AppSettings["SmtpPassword"] ?? "";
        }

        private static bool GetSmtpEnableSsl()
        {
            bool enableSsl = true;
            if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["SmtpEnableSsl"]))
            {
                bool.TryParse(WebConfigurationManager.AppSettings["SmtpEnableSsl"], out enableSsl);
            }
            return enableSsl;
        }

        /// <summary>
        /// Gửi email
        /// </summary>
        public static bool SendEmail(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                string smtpHost = GetSmtpHost();
                int smtpPort = GetSmtpPort();
                string smtpEmail = GetSmtpEmail();
                string smtpPassword = GetSmtpPassword();
                bool enableSsl = GetSmtpEnableSsl();

                if (string.IsNullOrEmpty(smtpEmail) || string.IsNullOrEmpty(smtpPassword))
                {
                    // Nếu chưa cấu hình email, log lỗi và trả về false
                    System.Diagnostics.Debug.WriteLine("SMTP chưa được cấu hình trong Web.config");
                    return false;
                }

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(smtpEmail, "Timepiece Store");
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = isHtml;

                SmtpClient smtp = new SmtpClient(smtpHost, smtpPort);
                smtp.EnableSsl = enableSsl;
                smtp.Credentials = new NetworkCredential(smtpEmail, smtpPassword);

                smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi gửi email: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gửi email quên mật khẩu
        /// </summary>
        public static bool SendPasswordResetEmail(string toEmail, string resetToken, string userName)
        {
            string resetUrl = HttpContext.Current.Request.Url.Scheme + "://" + 
                             HttpContext.Current.Request.Url.Authority + 
                             "/TaiKhoan/QuenMatKhau_65131134/ResetMatKhau?token=" + resetToken;

            string subject = "Khôi phục mật khẩu - Timepiece Store";
            string body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #c5a059; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; }}
                        .button {{ display: inline-block; padding: 12px 30px; background-color: #c5a059; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                        .footer {{ padding: 20px; text-align: center; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>TIMEPIECE STORE</h1>
                        </div>
                        <div class='content'>
                            <p>Xin chào <strong>{userName}</strong>,</p>
                            <p>Chúng tôi nhận được yêu cầu khôi phục mật khẩu cho tài khoản của bạn.</p>
                            <p>Vui lòng click vào nút bên dưới để đặt lại mật khẩu:</p>
                            <p style='text-align: center;'>
                                <a href='{resetUrl}' class='button'>Đặt lại mật khẩu</a>
                            </p>
                            <p>Hoặc copy và dán link sau vào trình duyệt:</p>
                            <p style='word-break: break-all; color: #c5a059;'>{resetUrl}</p>
                            <p><strong>Lưu ý:</strong> Link này sẽ hết hạn sau 24 giờ.</p>
                            <p>Nếu bạn không yêu cầu khôi phục mật khẩu, vui lòng bỏ qua email này.</p>
                        </div>
                        <div class='footer'>
                            <p>© {DateTime.Now.Year} Timepiece Store. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return SendEmail(toEmail, subject, body, true);
        }
    }
}

