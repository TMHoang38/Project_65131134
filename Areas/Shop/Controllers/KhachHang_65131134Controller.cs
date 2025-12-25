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

        // Load giỏ hàng từ database vào Session
        private void LoadCartFromDatabase()
        {
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
                return;

            var maKhachHang = AuthenticationHelper.GetUserId();
            if (string.IsNullOrEmpty(maKhachHang))
                return;

            var gioHangDB = db.GioHangs
                .Where(g => g.MaKhachHang == maKhachHang)
                .ToList();

            var cart = new Dictionary<string, int>();
            foreach (var item in gioHangDB)
            {
                cart[item.MaDongHo] = item.SoLuong;
            }

            Session["Cart"] = cart;
        }

        // Lưu giỏ hàng từ Session vào database
        private void SaveCartToDatabase()
        {
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
                return;

            var maKhachHang = AuthenticationHelper.GetUserId();
            if (string.IsNullOrEmpty(maKhachHang))
                return;

            var cart = Session["Cart"] as Dictionary<string, int> ?? new Dictionary<string, int>();

            // Xóa tất cả giỏ hàng cũ của khách hàng
            var oldCart = db.GioHangs.Where(g => g.MaKhachHang == maKhachHang).ToList();
            db.GioHangs.RemoveRange(oldCart);

            // Thêm giỏ hàng mới
            foreach (var item in cart)
            {
                if (item.Value > 0)
                {
                    var gioHang = new GioHang
                    {
                        MaKhachHang = maKhachHang,
                        MaDongHo = item.Key,
                        SoLuong = item.Value,
                        NgayThem = DateTime.Now
                    };
                    db.GioHangs.Add(gioHang);
                }
            }

            try
            {
                db.SaveChanges();
            }
            catch
            {
                // Ignore errors để không làm gián đoạn flow
            }
        }

        // Đồng bộ giữa Session và Database
        private void SyncCart()
        {
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
                return;

            var cart = Session["Cart"] as Dictionary<string, int>;
            if (cart == null)
            {
                LoadCartFromDatabase();
            }
            else
            {
                SaveCartToDatabase();
            }
        }

        private CartViewModel BuildCartViewModel()
        {
            // Đồng bộ giỏ hàng trước khi build
            SyncCart();

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

        public ActionResult GioHang()
        {
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
            {
                return RedirectToAction("Index", "DangNhap_65131134", new { area = "TaiKhoan" });
            }

            var vm = BuildCartViewModel();
            return View(vm);
        }

        [HttpPost]
        public ActionResult CapNhatSoLuong(string id, int quantity)
        {
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập!" });
            }

            SyncCart();
            var cart = Session["Cart"] as Dictionary<string, int> ?? new Dictionary<string, int>();

            if (cart.ContainsKey(id))
            {
                if (quantity < 1) quantity = 1;
                cart[id] = quantity;
            }

            Session["Cart"] = cart;
            SaveCartToDatabase();

            var vm = BuildCartViewModel();
            var updatedItem = vm.Items.FirstOrDefault(x => x.SanPham.MaDongHo == id);
            var fmt = new System.Globalization.CultureInfo("vi-VN");

            return Json(new
            {
                success = true,
                itemTotal = updatedItem?.ThanhTien.ToString("N0", fmt) + " đ",
                grandTotal = vm.TongTien.ToString("N0", fmt) + " đ"
            });
        }

        [HttpPost]
        public ActionResult XoaKhoiGioHang(string id)
        {
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập!" });
            }

            SyncCart();
            var cart = Session["Cart"] as Dictionary<string, int> ?? new Dictionary<string, int>();

            if (cart.ContainsKey(id))
            {
                cart.Remove(id);
            }

            Session["Cart"] = cart;
            SaveCartToDatabase();

            var vm = BuildCartViewModel();
            var fmt = new System.Globalization.CultureInfo("vi-VN");

            return Json(new
            {
                success = true,
                grandTotal = vm.TongTien.ToString("N0", fmt) + " đ"
            });
        }

        // GET: Shop/KhachHang_65131134/BuyNow - Đặt mua ngay
        public ActionResult BuyNow(string maDongHo, int soLuong = 1)
        {
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
            {
                return RedirectToAction("Index", "DangNhap_65131134", new { area = "TaiKhoan" });
            }

            if (string.IsNullOrEmpty(maDongHo))
            {
                TempData["ErrorMessage"] = "Mã sản phẩm không hợp lệ.";
                return RedirectToAction("Index", "Home_65131134", new { area = "Shop" });
            }

            var dongHo = db.DongHo.Find(maDongHo);
            if (dongHo == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index", "Home_65131134", new { area = "Shop" });
            }

            if (dongHo.SoLuongTon <= 0)
            {
                TempData["ErrorMessage"] = "Sản phẩm đã hết hàng.";
                return RedirectToAction("ChiTiet", "Home_65131134", new { area = "Shop", id = maDongHo });
            }

            if (soLuong < 1) soLuong = 1;
            if (soLuong > dongHo.SoLuongTon)
            {
                TempData["ErrorMessage"] = "Số lượng vượt quá tồn kho (Hiện có: " + dongHo.SoLuongTon + ").";
                return RedirectToAction("ChiTiet", "Home_65131134", new { area = "Shop", id = maDongHo });
            }

            var maKhachHang = AuthenticationHelper.GetUserId();
            
            // Tạo giỏ hàng tạm thời chỉ chứa sản phẩm này với số lượng đã chọn
            var tempCart = new Dictionary<string, int>();
            tempCart[maDongHo] = soLuong;
            Session["Cart"] = tempCart;

            // Lưu vào database để đảm bảo đồng bộ
            var gioHangItem = db.GioHangs.FirstOrDefault(g => g.MaKhachHang == maKhachHang && g.MaDongHo == maDongHo);
            if (gioHangItem != null)
            {
                gioHangItem.SoLuong = soLuong;
                gioHangItem.NgayThem = DateTime.Now;
            }
            else
            {
                db.GioHangs.Add(new GioHang 
                { 
                    MaKhachHang = maKhachHang, 
                    MaDongHo = maDongHo, 
                    SoLuong = soLuong, 
                    NgayThem = DateTime.Now 
                });
            }

            try
            {
                db.SaveChanges();
            }
            catch
            {
                // Ignore errors
            }

            // Chuyển đến trang thanh toán
            return RedirectToAction("ThanhToan");
        }

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
            var cartVm = BuildCartViewModel();
            if (!ModelState.IsValid)
            {
                model.Items = cartVm.Items;
                model.TongTien = cartVm.TongTien;
                return View(model);
            }

            if (!cartVm.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng đang trống.";
                return RedirectToAction("GioHang");
            }

            var maKhachHang = AuthenticationHelper.GetUserId();
            var khachHang = db.KhachHang.Find(maKhachHang);

            if (khachHang == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin khách hàng.";
                return RedirectToAction("Index", "DangNhap_65131134", new { area = "TaiKhoan" });
            }

            string maDonHang;
            int attempt = 0;
            do
            {
                var randomSuffix = new Random().Next(1000, 9999).ToString();
                var timePart = DateTime.Now.ToString("MMddHH");
                maDonHang = "DH" + timePart.Substring(0, Math.Min(6, timePart.Length)) + randomSuffix.Substring(0, 2);

                if (maDonHang.Length > 10) maDonHang = maDonHang.Substring(0, 10);

                attempt++;
                if (attempt > 10)
                {
                    maDonHang = "DH" + DateTime.Now.Ticks.ToString().Substring(DateTime.Now.Ticks.ToString().Length - 8);
                    break;
                }
            } while (db.DatHang.Any(d => d.MaDonHang == maDonHang));

            var diaChiGiaoHang = string.IsNullOrWhiteSpace(model.DiaChi)
                ? (string.IsNullOrWhiteSpace(khachHang.DiaChi) ? "Chưa cập nhật" : khachHang.DiaChi)
                : model.DiaChi;

            if (diaChiGiaoHang.Length > 200) diaChiGiaoHang = diaChiGiaoHang.Substring(0, 200);

            var ghiChu = string.IsNullOrWhiteSpace(model.GhiChu)
                ? null
                : (model.GhiChu.Length > 500 ? model.GhiChu.Substring(0, 500) : model.GhiChu);

            try
            {
                var donHang = new DatHang
                {
                    MaDonHang = maDonHang,
                    MaKhachHang = maKhachHang,
                    NgayDatHang = DateTime.Now,
                    DiaChiGiaoHang = diaChiGiaoHang,
                    TongTien = cartVm.TongTien,
                    TrangThaiDonHang = 0,
                    GhiChu = ghiChu
                };

                db.DatHang.Add(donHang);

                foreach (var item in cartVm.Items)
                {
                    var maChiTiet = Guid.NewGuid().ToString("N").Substring(0, 10);
                    var ct = new ChiTietDatHang
                    {
                        MaChiTiet = maChiTiet,
                        MaDonHang = maDonHang,
                        MaDongHo = item.SanPham.MaDongHo,
                        SoLuong = item.SoLuong,
                        DonGia = item.SanPham.GiaBan,
                        ThanhTien = item.ThanhTien
                    };
                    db.ChiTietDatHang.Add(ct);
                }

                db.SaveChanges();

                // Xóa giỏ hàng sau khi đặt hàng thành công
                // ĐÃ SỬA: Bỏ dòng khai báo maKhachHang dư thừa tại đây (Lỗi CS0136)
                var gioHangItems = db.GioHangs.Where(g => g.MaKhachHang == maKhachHang).ToList();
                db.GioHangs.RemoveRange(gioHangItems);
                db.SaveChanges();

                Session["Cart"] = new Dictionary<string, int>();
                TempData["SuccessMessage"] = "Đặt hàng thành công!";
                return RedirectToAction("DonHangCuaToi");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                model.Items = cartVm.Items;
                model.TongTien = cartVm.TongTien;
                return View(model);
            }
        }

        public ActionResult DonHangCuaToi()
        {
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
            {
                return RedirectToAction("Index", "DangNhap_65131134", new { area = "TaiKhoan" });
            }

            var maKhachHang = AuthenticationHelper.GetUserId();
            var donHangs = db.DatHang
                .Where(d => d.MaKhachHang == maKhachHang)
                .OrderByDescending(d => d.NgayDatHang)
                .ToList();
            return View(donHangs);
        }

        public ActionResult ChiTietDonHang(string id)
        {
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
            {
                return RedirectToAction("Index", "DangNhap_65131134", new { area = "TaiKhoan" });
            }

            var maKhachHang = AuthenticationHelper.GetUserId();
            var donHang = db.DatHang
                .Include(d => d.KhachHang)
                .Include("ChiTietDatHang")
                .Include("ChiTietDatHang.DongHo")
                .FirstOrDefault(d => d.MaDonHang == id && d.MaKhachHang == maKhachHang);

            if (donHang == null) return RedirectToAction("DonHangCuaToi");

            return View(donHang);
        }

        public ActionResult ChinhSuaHoSo()
        {
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
            {
                return RedirectToAction("Index", "DangNhap_65131134", new { area = "TaiKhoan" });
            }

            var maKhachHang = AuthenticationHelper.GetUserId();
            var khachHang = db.KhachHang.Find(maKhachHang);
            return View(khachHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChinhSuaHoSo(KhachHang input)
        {
            var maKhachHang = AuthenticationHelper.GetUserId();
            var khachHang = db.KhachHang.Find(maKhachHang);

            if (ModelState.IsValid)
            {
                khachHang.HoTen = input.HoTen;
                khachHang.Email = input.Email;
                khachHang.SoDienThoai = input.SoDienThoai;
                khachHang.DiaChi = input.DiaChi;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật thành công.";
            }
            return View(input);
        }

        public ActionResult DoiMatKhau()
        {
            if (!AuthenticationHelper.IsLoggedIn() || !AuthenticationHelper.IsKhachHang())
            {
                return RedirectToAction("Index", "DangNhap_65131134", new { area = "TaiKhoan" });
            }
            return View(new DoiMatKhauViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoiMatKhau(DoiMatKhauViewModel model)
        {
            var maKhachHang = AuthenticationHelper.GetUserId();
            var khachHang = db.KhachHang.Find(maKhachHang);

            if (ModelState.IsValid)
            {
                if (khachHang.MatKhau == model.MatKhauCu)
                {
                    khachHang.MatKhau = model.MatKhauMoi;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Đổi mật khẩu thành công.";
                }
                else
                {
                    ModelState.AddModelError("", "Mật khẩu cũ không đúng.");
                }
            }
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}