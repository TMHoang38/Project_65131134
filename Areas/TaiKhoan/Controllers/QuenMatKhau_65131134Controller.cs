using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_65131134.Models;
using Project_65131134.Helpers;

namespace Project_65131134.Areas.TaiKhoan.Controllers
{
    public class QuenMatKhau_65131134Controller : Controller
    {
        private CuaHangDongHo_65131134Entities db = new CuaHangDongHo_65131134Entities();

        // GET: TaiKhoan/QuenMatKhau_65131134
        public ActionResult Index()
        {
            return View();
        }

        // POST: TaiKhoan/QuenMatKhau_65131134/QuenMatKhau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuenMatKhau(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập địa chỉ email!";
                return View("Index");
            }

            // Tìm khách hàng hoặc nhân viên theo email
            var khachHang = db.KhachHang.FirstOrDefault(k => k.Email == Email);
            var nhanVien = db.NhanVien.FirstOrDefault(n => n.Email == Email);

            if (khachHang == null && nhanVien == null)
            {
                // Không hiển thị lỗi chi tiết để bảo mật
                ViewBag.SuccessMessage = "Nếu email tồn tại trong hệ thống, chúng tôi đã gửi link khôi phục mật khẩu đến email của bạn.";
                return View("Index");
            }

            try
            {
                string userName = khachHang != null ? khachHang.TenDangNhap : nhanVien.TenDangNhap;
                string resetToken = PasswordResetHelper.GenerateResetToken(Email);

                // Gửi email
                bool emailSent = EmailHelper.SendPasswordResetEmail(Email, resetToken, userName);

                if (emailSent)
                {
                    ViewBag.SuccessMessage = "Chúng tôi đã gửi link khôi phục mật khẩu đến email của bạn. Vui lòng kiểm tra hộp thư (cả thư mục spam).";
                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể gửi email. Vui lòng kiểm tra cấu hình email hoặc thử lại sau.";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi quên mật khẩu: {ex.Message}");
                ViewBag.ErrorMessage = "Có lỗi xảy ra. Vui lòng thử lại sau.";
            }

            return View("Index");
        }

        // GET: TaiKhoan/QuenMatKhau_65131134/ResetMatKhau
        public ActionResult ResetMatKhau(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.ErrorMessage = "Link không hợp lệ hoặc đã hết hạn.";
                return View("Index");
            }

            string email;
            DateTime? timestamp;

            if (!PasswordResetHelper.ValidateToken(token, out email, out timestamp))
            {
                ViewBag.ErrorMessage = "Link không hợp lệ hoặc đã hết hạn. Vui lòng yêu cầu lại.";
                return View("Index");
            }

            ViewBag.Token = token;
            ViewBag.Email = email;
            return View();
        }

        // POST: TaiKhoan/QuenMatKhau_65131134/ResetMatKhau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetMatKhau(string token, string MatKhau, string XacNhanMatKhau)
        {
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.ErrorMessage = "Link không hợp lệ hoặc đã hết hạn.";
                return View("Index");
            }

            string email;
            DateTime? timestamp;

            if (!PasswordResetHelper.ValidateToken(token, out email, out timestamp))
            {
                ViewBag.ErrorMessage = "Link không hợp lệ hoặc đã hết hạn. Vui lòng yêu cầu lại.";
                return View("Index");
            }

            // Validation mật khẩu
            if (string.IsNullOrEmpty(MatKhau) || MatKhau.Length < 6)
            {
                ViewBag.ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự!";
                ViewBag.Token = token;
                ViewBag.Email = email;
                return View();
            }

            if (MatKhau != XacNhanMatKhau)
            {
                ViewBag.ErrorMessage = "Mật khẩu xác nhận không khớp!";
                ViewBag.Token = token;
                ViewBag.Email = email;
                return View();
            }

            try
            {
                // Tìm và cập nhật mật khẩu
                var khachHang = db.KhachHang.FirstOrDefault(k => k.Email == email);
                var nhanVien = db.NhanVien.FirstOrDefault(n => n.Email == email);

                if (khachHang != null)
                {
                    khachHang.MatKhau = MatKhau;
                    db.Entry(khachHang).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    ViewBag.SuccessMessage = "Đặt lại mật khẩu thành công! Bạn có thể đăng nhập ngay bây giờ.";
                    return RedirectToAction("Index", "DangNhap_65131134");
                }
                else if (nhanVien != null)
                {
                    nhanVien.MatKhau = MatKhau;
                    db.Entry(nhanVien).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    ViewBag.SuccessMessage = "Đặt lại mật khẩu thành công! Bạn có thể đăng nhập ngay bây giờ.";
                    return RedirectToAction("Index", "DangNhap_65131134");
                }
                else
                {
                    ViewBag.ErrorMessage = "Không tìm thấy tài khoản.";
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi reset mật khẩu: {ex.Message}");
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi đặt lại mật khẩu. Vui lòng thử lại sau.";
                ViewBag.Token = token;
                ViewBag.Email = email;
                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}