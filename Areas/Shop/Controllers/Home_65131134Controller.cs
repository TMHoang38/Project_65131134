using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_65131134.Models;
using System.Data.Entity;
using Project_65131134.Models.ViewModels;

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
            if (page > totalPages && totalPages > 0) page = totalPages;

            // 4. Lấy dữ liệu theo trang (Sử dụng Skip và Take)
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

            // Trả về Partial View nếu là AJAX request
            if (Request.IsAjaxRequest())
            {
                return PartialView("_DongHoListPartial", viewModel);
            }

            var thuongHieus = db.ThuongHieu.OrderBy(t => t.TenThuongHieu).ToList();
            ViewBag.BrandList = new SelectList(thuongHieus, "MaThuongHieu", "TenThuongHieu", brandId);
            ViewBag.SearchTerm = searchTerm;
            ViewBag.PriceRange = priceRange;

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult AddToCart(string maDongHo, int soLuong = 1)
        {
            try
            {
                // Kiểm tra đăng nhập
                if (Session["UserId"] == null)
                    return Json(new { success = false, message = "Vui lòng đăng nhập!", requireLogin = true });

                // Kiểm tra sản phẩm tồn tại và còn hàng
                var dongHo = db.DongHo.Find(maDongHo);
                if (dongHo == null)
                    return Json(new { success = false, message = "Sản phẩm không tồn tại!" });

                if (dongHo.SoLuongTon <= 0)
                    return Json(new { success = false, message = "Sản phẩm đã hết hàng!" });

                // Đảm bảo số lượng thêm vào ít nhất là 1
                if (soLuong < 1) soLuong = 1;

                var cart = Session["Cart"] as Dictionary<string, int> ?? new Dictionary<string, int>();

                if (!cart.ContainsKey(maDongHo))
                {
                    // Nếu chưa có trong giỏ, kiểm tra xem số lượng yêu cầu có vượt kho không
                    if (soLuong <= dongHo.SoLuongTon)
                    {
                        cart[maDongHo] = soLuong;
                    }
                    else
                    {
                        return Json(new { success = false, message = "Chỉ còn " + dongHo.SoLuongTon + " sản phẩm trong kho!" });
                    }
                }
                else
                {
                    // Nếu đã có trong giỏ, cộng dồn và kiểm tra tổng số lượng có vượt kho không
                    int newQuantity = cart[maDongHo] + soLuong;
                    if (newQuantity <= dongHo.SoLuongTon)
                    {
                        cart[maDongHo] = newQuantity;
                    }
                    else
                    {
                        return Json(new { success = false, message = "Tổng số lượng trong giỏ hàng vượt quá tồn kho (Hiện có: " + dongHo.SoLuongTon + ")!" });
                    }
                }

                Session["Cart"] = cart;
                int totalQuantity = cart.Values.Sum();
                return Json(new { success = true, message = "Đã thêm " + soLuong + " sản phẩm vào giỏ hàng!", cartCount = totalQuantity });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

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

        [HttpGet]
        public JsonResult GetCartCount()
        {
            var cart = Session["Cart"] as Dictionary<string, int> ?? new Dictionary<string, int>();
            int totalQuantity = cart.Values.Sum();
            return Json(new { cartCount = totalQuantity }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}