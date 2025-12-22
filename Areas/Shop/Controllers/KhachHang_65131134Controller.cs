using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Project_65131134.Helpers;
using Project_65131134.Models;
using Project_65131134.Models.ViewModels;

namespace Project_65131134.Areas.Shop.Controllers
{
    public class KhachHang_65131134Controller : Controller
    {
        private CuaHangDongHo_65131134Entities db = new CuaHangDongHo_65131134Entities();

        /// <summary>
        /// Lấy giỏ hàng từ session và map sang view model
        /// </summary>
        private CartViewModel BuildCartViewModel()
        {
            var cart = Session["Cart"] as Dictionary<string, int> ?? new Dictionary<string, int>();
            var vm = new CartViewModel();

            if (!cart.Any()) return vm;

            var productIds = cart.Keys.ToList();
            var products = db.DongHo
                .Include(d => d.ThuongHieu)
                .Where(d => productIds.Contains(d.MaDongHo))
                .ToList();

            foreach (var p in products)
            {
                var qty = cart.ContainsKey(p.MaDongHo) ? cart[p.MaDongHo] : 0;
                if (qty <= 0) continue;

                vm.Items.Add(new CartItemViewModel
                {
                    SanPham = p,
                    SoLuong = qty
                });
            }

            vm.TongTien = vm.Items.Sum(x => x.ThanhTien);
            return vm;
        }

        /// <summary>
        /// Trang giỏ hàng
        /// </summary>
        public ActionResult GioHang()
        {
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
            {
                return RedirectToAction("Index", "DangNhap_65131134", new { area = "TaiKhoan" });
            }

            var vm = BuildCartViewModel();
            return View(vm);
        }

        /// <summary>
        /// Trang thanh toán
        /// </summary>
        public ActionResult ThanhToan()
        {
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
            {
                return RedirectToAction("Index", "DangNhap_65131134", new { area = "TaiKhoan" });
            }

            var cartVm = BuildCartViewModel();
            if (!cartVm.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng đang trống.";
                return RedirectToAction("GioHang");
            }

            var maKhachHang = AuthenticationHelper.GetUserId();
            var khachHang = db.KhachHang.Find(maKhachHang);

            var vm = new CheckoutViewModel
            {
                Items = cartVm.Items,
                TongTien = cartVm.TongTien,
                HoTen = khachHang?.HoTen,
                Email = khachHang?.Email,
                SoDienThoai = khachHang?.SoDienThoai,
                DiaChi = khachHang?.DiaChi
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThanhToan(CheckoutViewModel model)
        {
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
            {
                return RedirectToAction("Index", "DangNhap_65131134", new { area = "TaiKhoan" });
            }

            var cartVm = BuildCartViewModel();
            if (!cartVm.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng đang trống.";
                return RedirectToAction("GioHang");
            }

            if (!ModelState.IsValid)
            {
                model.Items = cartVm.Items;
                model.TongTien = cartVm.TongTien;
                return View(model);
            }

            var maKhachHang = AuthenticationHelper.GetUserId();
            var khachHang = db.KhachHang.Find(maKhachHang);

            // Tạo mã đơn hàng đơn giản: DH + timestamp
            var maDonHang = "DH" + DateTime.Now.ToString("yyyyMMddHHmmss");

            var donHang = new DatHang
            {
                MaDonHang = maDonHang,
                MaKhachHang = maKhachHang,
                NgayDatHang = DateTime.Now,
                DiaChiGiaoHang = string.IsNullOrWhiteSpace(model.DiaChi) ? khachHang?.DiaChi : model.DiaChi,
                TongTien = cartVm.TongTien,
                TrangThaiDonHang = 0, // Chờ xác nhận
                GhiChu = model.GhiChu
            };

            db.DatHang.Add(donHang);

            foreach (var item in cartVm.Items)
            {
                var ct = new ChiTietDatHang
                {
                    MaChiTiet = Guid.NewGuid().ToString("N").Substring(0, 10),
                    MaDonHang = maDonHang,
                    MaDongHo = item.SanPham.MaDongHo,
                    SoLuong = item.SoLuong,
                    DonGia = item.SanPham.GiaBan,
                    ThanhTien = item.ThanhTien
                };
                db.ChiTietDatHang.Add(ct);

                // Trừ kho
                var sp = db.DongHo.Find(item.SanPham.MaDongHo);
                if (sp != null && sp.SoLuongTon.HasValue)
                {
                    sp.SoLuongTon -= item.SoLuong;
                }
            }

            db.SaveChanges();

            // Xóa giỏ hàng
            Session["Cart"] = new Dictionary<string, int>();

            TempData["SuccessMessage"] = "Đặt hàng thành công!";
            return RedirectToAction("DonHangCuaToi");
        }

        /// <summary>
        /// Thông tin cá nhân khách hàng đang đăng nhập
        /// </summary>
        public ActionResult ChinhSuaHoSo()
        {
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
            {
                return RedirectToAction("Index", "DangNhap_65131134", new { area = "TaiKhoan" });
            }

            var maKhachHang = AuthenticationHelper.GetUserId();
            var khachHang = db.KhachHang.Find(maKhachHang);
            if (khachHang == null)
            {
                return RedirectToAction("Index", "Home_65131134");
            }

            return View(khachHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChinhSuaHoSo([Bind(Include = "MaKhachHang,TenDangNhap,HoTen,Email,SoDienThoai,DiaChi")] KhachHang model)
        {
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
            {
                return RedirectToAction("Index", "DangNhap_65131134", new { area = "TaiKhoan" });
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var khachHang = db.KhachHang.Find(model.MaKhachHang);
            if (khachHang == null)
            {
                return RedirectToAction("Index", "Home_65131134");
            }

            khachHang.HoTen = model.HoTen;
            khachHang.Email = model.Email;
            khachHang.SoDienThoai = model.SoDienThoai;
            khachHang.DiaChi = model.DiaChi;

            db.Entry(khachHang).State = EntityState.Modified;
            db.SaveChanges();

            TempData["SuccessMessage"] = "Cập nhật thông tin cá nhân thành công!";
            return RedirectToAction("ChinhSuaHoSo");
        }

        /// <summary>
        /// Danh sách đơn hàng của khách hàng hiện tại
        /// </summary>
        public ActionResult DonHangCuaToi()
        {
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
            {
                return RedirectToAction("Index", "DangNhap_65131134", new { area = "TaiKhoan" });
            }

            var maKhachHang = AuthenticationHelper.GetUserId();

            var donHangs = db.DatHang
                .Include(d => d.ChiTietDatHang.Select(ct => ct.DongHo))
                .Where(d => d.MaKhachHang == maKhachHang)
                .OrderByDescending(d => d.NgayDatHang)
                .ToList();

            return View(donHangs);
        }

        /// <summary>
        /// Chi tiết đơn hàng của khách hàng hiện tại
        /// </summary>
        public ActionResult ChiTietDonHang(string id)
        {
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
            {
                return RedirectToAction("Index", "DangNhap_65131134", new { area = "TaiKhoan" });
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("DonHangCuaToi");
            }

            var maKhachHang = AuthenticationHelper.GetUserId();

            var donHang = db.DatHang
                .Include(d => d.ChiTietDatHang.Select(ct => ct.DongHo))
                .Include(d => d.KhachHang)
                .Include(d => d.NhanVien)
                .Include(d => d.NhanVien1)
                .FirstOrDefault(d => d.MaDonHang == id && d.MaKhachHang == maKhachHang);

            if (donHang == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("DonHangCuaToi");
            }

            return View(donHang);
        }

        public ActionResult DoiMatKhau()
        {
            // 1. Kiểm tra đăng nhập
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
            {
                return RedirectToAction("Index", "DangNhap_65131134", new { area = "TaiKhoan" });
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoiMatKhau(DoiMatKhauViewModel model)
        {
            // 1. Kiểm tra đăng nhập
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
            {
                return RedirectToAction("Index", "DangNhap_65131134", new { area = "TaiKhoan" });
            }

            // 2. Kiểm tra tính hợp lệ của dữ liệu nhập vào (Required, Compare, Length...)
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 3. Lấy thông tin khách hàng từ Database
            var maKhachHang = AuthenticationHelper.GetUserId();
            var khachHang = db.KhachHang.Find(maKhachHang);

            if (khachHang == null)
            {
                return RedirectToAction("Index", "Home_65131134");
            }

            // 4. Kiểm tra mật khẩu cũ có đúng không
            // Lưu ý: Nếu bạn có dùng mã hóa (MD5/SHA) thì phải mã hóa model.MatKhauCu trước khi so sánh
            if (khachHang.MatKhau != model.MatKhauCu)
            {
                ModelState.AddModelError("MatKhauCu", "Mật khẩu hiện tại không đúng.");
                return View(model);
            }

            // 5. Kiểm tra mật khẩu mới có trùng mật khẩu cũ không (Tùy chọn)
            if (khachHang.MatKhau == model.MatKhauMoi)
            {
                ModelState.AddModelError("MatKhauMoi", "Mật khẩu mới không được trùng với mật khẩu cũ.");
                return View(model);
            }

            // 6. Cập nhật mật khẩu mới và lưu vào Database
            // Lưu ý: Nếu hệ thống dùng mã hóa, hãy mã hóa model.MatKhauMoi trước khi gán
            khachHang.MatKhau = model.MatKhauMoi;

            // Đánh dấu trường mật khẩu đã thay đổi
            db.Entry(khachHang).Property(x => x.MatKhau).IsModified = true;
            db.SaveChanges();

            // 7. Thông báo thành công
            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("ChinhSuaHoSo"); // Hoặc quay lại trang chủ
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


