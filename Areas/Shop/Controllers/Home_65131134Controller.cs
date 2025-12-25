using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_65131134.Models;
using System.Data.Entity;
using Project_65131134.Models.ViewModels;
using Project_65131134.Helpers;

namespace Project_65131134.Areas.Shop.Controllers
{
    public class Home_65131134Controller : Controller
    {
        private CuaHangDongHo_65131134Entities db = new CuaHangDongHo_65131134Entities();

        public ActionResult Index(string searchTerm, string brandId, string priceRange, int page = 1)
        {
            // Thiết lập số lượng sản phẩm mỗi trang
            int pageSize = 9;

            // 1. Khởi tạo Query ban đầu
            var dongHoQuery = db.DongHo
                .Include(d => d.LoaiDongHo)
                .Include(d => d.NoiSanXuat)
                .Include(d => d.ThuongHieu)
                .Include(d => d.ChatLieuDay)
                .Include(d => d.ChatLieuVo)
                .Where(d => d.SoLuongTon > 0);

            // 2. Áp dụng các bộ lọc tìm kiếm
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var keyword = searchTerm.Trim();
                dongHoQuery = dongHoQuery.Where(d => d.TenDongHo.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(brandId))
            {
                dongHoQuery = dongHoQuery.Where(d => d.MaThuongHieu == brandId);
            }

            if (!string.IsNullOrWhiteSpace(priceRange))
            {
                switch (priceRange)
                {
                    case "lt5": dongHoQuery = dongHoQuery.Where(d => d.GiaBan < 5000000); break;
                    case "5to10": dongHoQuery = dongHoQuery.Where(d => d.GiaBan >= 5000000 && d.GiaBan <= 10000000); break;
                    case "10to50": dongHoQuery = dongHoQuery.Where(d => d.GiaBan > 10000000 && d.GiaBan <= 50000000); break;
                    case "50to100": dongHoQuery = dongHoQuery.Where(d => d.GiaBan > 50000000 && d.GiaBan <= 100000000); break;
                    case "gt100": dongHoQuery = dongHoQuery.Where(d => d.GiaBan > 100000000); break;
                }
            }

            // 3. Tính toán phân trang
            int totalItems = dongHoQuery.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Đảm bảo trang hiện tại không vượt quá giới hạn
            if (page < 1) page = 1;
            if (totalPages > 0 && page > totalPages) page = totalPages;

            // 4. Lấy dữ liệu theo trang
            var pagedData = dongHoQuery
                .OrderByDescending(d => d.MaDongHo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 5. Tạo ViewModel
            var viewModel = new DongHoPaginationViewModel
            {
                DanhSachDongHo = pagedData,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems
            };

            // Lưu các giá trị lọc vào ViewBag để hiển thị lại trên Form
            ViewBag.SearchTerm = searchTerm;
            ViewBag.PriceRange = priceRange;
            var thuongHieus = db.ThuongHieu.OrderBy(t => t.TenThuongHieu).ToList();
            ViewBag.BrandList = new SelectList(thuongHieus, "MaThuongHieu", "TenThuongHieu", brandId);

            // Trả về Partial View nếu là AJAX request
            if (Request.IsAjaxRequest())
            {
                return PartialView("_DongHoListPartial", viewModel);
            }

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult AddToCart(string maDongHo, int soLuong = 1)
        {
            try
            {
                if (Session["UserId"] == null || !AuthenticationHelper.IsKhachHang())
                    return Json(new { success = false, message = "Vui lòng đăng nhập!", requireLogin = true });

                var dongHo = db.DongHo.Find(maDongHo);
                if (dongHo == null)
                    return Json(new { success = false, message = "Sản phẩm không tồn tại!" });

                if (dongHo.SoLuongTon <= 0)
                    return Json(new { success = false, message = "Sản phẩm đã hết hàng!" });

                if (soLuong < 1) soLuong = 1;

                var maKhachHang = AuthenticationHelper.GetUserId();
                var cart = Session["Cart"] as Dictionary<string, int> ?? new Dictionary<string, int>();

                if (!cart.Any())
                {
                    var gioHangDB = db.GioHangs.Where(g => g.MaKhachHang == maKhachHang).ToList();
                    foreach (var item in gioHangDB) cart[item.MaDongHo] = item.SoLuong;
                }

                int currentQuantity = cart.ContainsKey(maDongHo) ? cart[maDongHo] : 0;
                int newQuantity = currentQuantity + soLuong;

                if (newQuantity > dongHo.SoLuongTon)
                {
                    return Json(new { success = false, message = "Tổng số lượng vượt quá tồn kho (Hiện có: " + dongHo.SoLuongTon + ")!" });
                }

                cart[maDongHo] = newQuantity;
                Session["Cart"] = cart;

                var gioHangItem = db.GioHangs.FirstOrDefault(g => g.MaKhachHang == maKhachHang && g.MaDongHo == maDongHo);
                if (gioHangItem != null)
                {
                    gioHangItem.SoLuong = newQuantity;
                    gioHangItem.NgayThem = DateTime.Now;
                }
                else
                {
                    db.GioHangs.Add(new GioHang { MaKhachHang = maKhachHang, MaDongHo = maDongHo, SoLuong = newQuantity, NgayThem = DateTime.Now });
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Đã thêm vào giỏ hàng!", cartCount = cart.Values.Sum() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetCartCount()
        {
            var cart = Session["Cart"] as Dictionary<string, int> ?? new Dictionary<string, int>();
            return Json(new { cartCount = cart.Values.Sum() }, JsonRequestBehavior.AllowGet);
        }

        // GET: Shop/Home_65131134/ChiTiet/id
        public ActionResult ChiTiet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var dongHo = db.DongHo
                .Include(d => d.LoaiDongHo)
                .Include(d => d.NoiSanXuat)
                .Include(d => d.ThuongHieu)
                .Include(d => d.ChatLieuDay)
                .Include(d => d.ChatLieuVo)
                .FirstOrDefault(d => d.MaDongHo == id);

            if (dongHo == null)
            {
                return HttpNotFound();
            }

            return View(dongHo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}