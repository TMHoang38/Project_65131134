using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_65131134.Models;
using Project_65131134.Helpers;

namespace Project_65131134.Areas.TaiKhoan.Controllers
{
    public class DangNhap_65131134Controller : Controller
    {
        private CuaHangDongHo_65131134Entities db = new CuaHangDongHo_65131134Entities();

        // GET: TaiKhoan/DangNhap_65131134
        public ActionResult Index()
        {
            // Nếu đã đăng nhập, chuyển hướng đến trang chủ tương ứng
            if (AuthenticationHelper.IsLoggedIn())
            {
                return RedirectToHomePage();
            }
            return View();
        }

        // POST: TaiKhoan/DangNhap_65131134/DangNhap
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(string TaiKhoan, string MatKhau)
        {
            if (string.IsNullOrEmpty(TaiKhoan) || string.IsNullOrEmpty(MatKhau))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập đầy đủ thông tin đăng nhập!";
                return View("Index");
            }

            // Tìm kiếm trong bảng Nhân viên trước
            var nhanVien = db.NhanVien.FirstOrDefault(nv => nv.TenDangNhap == TaiKhoan && nv.MatKhau == MatKhau);
            
            if (nhanVien != null)
            {
                // Đăng nhập thành công cho nhân viên
                AuthenticationHelper.LoginNhanVien(nhanVien);
                
                // Chuyển hướng theo vai trò
                return RedirectToHomePage();
            }

            // Nếu không tìm thấy trong nhân viên, tìm trong khách hàng
            var khachHang = db.KhachHang.FirstOrDefault(kh => kh.TenDangNhap == TaiKhoan && kh.MatKhau == MatKhau);
            
            if (khachHang != null)
            {
                // Kiểm tra trạng thái tài khoản khách hàng
                if (khachHang.TrangThai == false)
                {
                    ViewBag.ErrorMessage = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên!";
                    return View("Index");
                }

                // Đăng nhập thành công cho khách hàng
                AuthenticationHelper.LoginKhachHang(khachHang);
                
                // Load giỏ hàng từ database vào Session
                var gioHangDB = db.GioHangs
                    .Where(g => g.MaKhachHang == khachHang.MaKhachHang)
                    .ToList();
                
                var cart = new Dictionary<string, int>();
                foreach (var item in gioHangDB)
                {
                    cart[item.MaDongHo] = item.SoLuong;
                }
                Session["Cart"] = cart;
                
                // Chuyển đến trang shop cho khách hàng
                return RedirectToAction("Index", "Home_65131134", new { area = "Shop" });
            }

            // Không tìm thấy tài khoản
            ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng!";
            return View("Index");
        }

        // GET: TaiKhoan/DangNhap_65131134/DangXuat
        public ActionResult DangXuat()
        {
            AuthenticationHelper.Logout();
            // Sau khi đăng xuất, chuyển về trang chủ Shop cho khách hàng
            return RedirectToAction("Index", "Home_65131134", new { area = "Shop" });
        }

        /// <summary>
        /// Chuyển hướng đến trang chủ tương ứng với vai trò
        /// </summary>
        private ActionResult RedirectToHomePage()
        {
            if (AuthenticationHelper.IsAdmin())
            {
                return RedirectToAction("Index", "NhanVien_65131134", new { area = "Admin" });
            }
            else if (AuthenticationHelper.IsNhanVienBanHang())
            {
                return RedirectToAction("Index", "DonHang_65131134", new { area = "NhanVienBanHang" });
            }
            else if (AuthenticationHelper.IsNhanVienGiaoHang())
            {
                return RedirectToAction("Index", "DonHang_65131134", new { area = "NhanVienGiaoHang" });
            }
            else if (AuthenticationHelper.IsKhachHang())
            {
                return RedirectToAction("Index", "Home_65131134", new { area = "Shop" });
            }
            
            // Mặc định về trang đăng nhập
            return RedirectToAction("Index");
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