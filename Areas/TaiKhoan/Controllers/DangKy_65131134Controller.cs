using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_65131134.Models;
using Project_65131134.Models.ViewModels;

namespace Project_65131134.Areas.TaiKhoan.Controllers
{
    public class DangKy_65131134Controller : Controller
    {
        private CuaHangDongHo_65131134Entities db = new CuaHangDongHo_65131134Entities();

        // GET: TaiKhoan/DangKy_65131134
        public ActionResult Index()
        {
            return View();
        }

        // POST: TaiKhoan/DangKy_65131134/DangKy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(string TenDangNhap, string MatKhau, string XacNhanMatKhau, 
            string HoTen, string Email, string SoDienThoai, string DiaChi)
        {
            // Kiểm tra validation
            if (string.IsNullOrEmpty(TenDangNhap))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập tên đăng nhập!";
                return View("Index");
            }

            if (string.IsNullOrEmpty(MatKhau) || MatKhau.Length < 6)
            {
                ViewBag.ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự!";
                return View("Index");
            }

            if (MatKhau != XacNhanMatKhau)
            {
                ViewBag.ErrorMessage = "Mật khẩu xác nhận không khớp!";
                return View("Index");
            }

            if (string.IsNullOrEmpty(HoTen))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập họ tên!";
                return View("Index");
            }

            if (string.IsNullOrEmpty(Email) || !System.Text.RegularExpressions.Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ViewBag.ErrorMessage = "Email không hợp lệ!";
                return View("Index");
            }

            if (string.IsNullOrEmpty(SoDienThoai))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập số điện thoại!";
                return View("Index");
            }

            if (string.IsNullOrEmpty(DiaChi))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập địa chỉ!";
                return View("Index");
            }

            // Kiểm tra tên đăng nhập đã tồn tại chưa (trong cả KhachHang và NhanVien)
            bool tenDangNhapExists = db.KhachHang.Any(k => k.TenDangNhap == TenDangNhap) ||
                                     db.NhanVien.Any(n => n.TenDangNhap == TenDangNhap);

            if (tenDangNhapExists)
            {
                ViewBag.ErrorMessage = "Tên đăng nhập đã tồn tại! Vui lòng chọn tên khác.";
                return View("Index");
            }

            // Kiểm tra email đã tồn tại chưa
            bool emailExists = db.KhachHang.Any(k => k.Email == Email) ||
                              db.NhanVien.Any(n => n.Email == Email);

            if (emailExists)
            {
                ViewBag.ErrorMessage = "Email đã được sử dụng! Vui lòng sử dụng email khác.";
                return View("Index");
            }

            try
            {
                // Tạo mã khách hàng mới
                string maKhachHang = LayMaKH();

                // Tạo khách hàng mới
                KhachHang khachHang = new KhachHang
                {
                    MaKhachHang = maKhachHang,
                    TenDangNhap = TenDangNhap,
                    MatKhau = MatKhau,
                    HoTen = HoTen,
                    Email = Email,
                    SoDienThoai = SoDienThoai,
                    DiaChi = DiaChi,
                    NgayTao = DateTime.Now,
                    TrangThai = true // Mặc định là kích hoạt
                };

                db.KhachHang.Add(khachHang);
                db.SaveChanges();

                ViewBag.SuccessMessage = "Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.";
                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi đăng ký. Vui lòng thử lại sau!";
                System.Diagnostics.Debug.WriteLine($"Lỗi đăng ký: {ex.Message}");
                return View("Index");
            }
        }

        /// <summary>
        /// Tạo mã khách hàng tự động
        /// </summary>
        private string LayMaKH()
        {
            var maMax = db.KhachHang
                .Select(n => n.MaKhachHang)
                .OrderByDescending(m => m)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(maMax) || maMax.Length < 3)
            {
                return "KH001";
            }

            var numericPart = maMax.Substring(2);
            if (!int.TryParse(numericPart, out int currentNumber))
            {
                currentNumber = 0;
            }

            return $"KH{(currentNumber + 1):D3}";
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