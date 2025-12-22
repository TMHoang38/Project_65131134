using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Project_65131134.Helpers;
using Project_65131134.Helpers.Attributes;
using Project_65131134.Models;

namespace Project_65131134.Areas.NhanVienGiaoHang.Controllers
{
    [NhanVienGiaoHangRole]
    public class TaiKhoan_65131134Controller : Controller
    {
        private readonly CuaHangDongHo_65131134Entities _db = new CuaHangDongHo_65131134Entities();

        // GET: NhanVienGiaoHang/TaiKhoan_65131134/ThongTinCaNhan
        public ActionResult ThongTinCaNhan()
        {
            var user = LayNhanVienHienTai();
            if (user == null)
            {
                return ChuyenHuongDangNhap();
            }

            return View(user);
        }

        // POST: NhanVienGiaoHang/TaiKhoan_65131134/ThongTinCaNhan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThongTinCaNhan([Bind(Include = "MaNhanVien,HoTen,Email,SoDienThoai,DiaChi")] NhanVien input)
        {
            var user = LayNhanVienHienTai();
            if (user == null)
            {
                return ChuyenHuongDangNhap();
            }

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            user.HoTen = input.HoTen;
            user.Email = input.Email;
            user.SoDienThoai = input.SoDienThoai;
            user.DiaChi = input.DiaChi;

            _db.Entry(user).State = EntityState.Modified;
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Cập nhật thông tin thành công.";
            return RedirectToAction("ThongTinCaNhan");
        }

        // GET: NhanVienGiaoHang/TaiKhoan_65131134/DoiMatKhau
        public ActionResult DoiMatKhau()
        {
            var user = LayNhanVienHienTai();
            if (user == null)
            {
                return ChuyenHuongDangNhap();
            }

            return View();
        }

        // POST: NhanVienGiaoHang/TaiKhoan_65131134/DoiMatKhau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoiMatKhau(string MatKhauHienTai, string MatKhauMoi, string XacNhanMatKhau)
        {
            var user = LayNhanVienHienTai();
            if (user == null)
            {
                return ChuyenHuongDangNhap();
            }

            if (string.IsNullOrWhiteSpace(MatKhauHienTai) ||
                string.IsNullOrWhiteSpace(MatKhauMoi) ||
                string.IsNullOrWhiteSpace(XacNhanMatKhau))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            if (MatKhauMoi.Length < 6)
            {
                ViewBag.ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                return View();
            }

            if (MatKhauMoi != XacNhanMatKhau)
            {
                ViewBag.ErrorMessage = "Mật khẩu xác nhận không khớp.";
                return View();
            }

            if (!string.Equals(user.MatKhau, MatKhauHienTai))
            {
                ViewBag.ErrorMessage = "Mật khẩu hiện tại không đúng.";
                return View();
            }

            user.MatKhau = MatKhauMoi;
            _db.Entry(user).State = EntityState.Modified;
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công.";
            return RedirectToAction("DoiMatKhau");
        }

        // GET: NhanVienGiaoHang/TaiKhoan_65131134/DangXuat
        public ActionResult DangXuat()
        {
            AuthenticationHelper.Logout();
            return RedirectToAction("Index", "DangNhap_65131134", new { area = "TaiKhoan" });
        }

        private NhanVien LayNhanVienHienTai()
        {
            var userId = AuthenticationHelper.GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            return _db.NhanVien.FirstOrDefault(nv => nv.MaNhanVien == userId);
        }

        private ActionResult ChuyenHuongDangNhap()
        {
            AuthenticationHelper.Logout();
            return RedirectToAction("Index", "DangNhap_65131134", new { area = "TaiKhoan" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

