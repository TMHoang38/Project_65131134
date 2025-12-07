using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_65131134.Models;

namespace Project_65131134.Areas.Admin.Controllers
{
    public class ThongKe_65131134Controller : Controller
    {
        private CuaHangDongHo_65131134Entities db = new CuaHangDongHo_65131134Entities();

        // URL: /ThongKe/SanPhamBanChay
        public ActionResult SanPhamBanChay(int? topN)
        {
            // Mặc định lấy top 10 sản phẩm bán chạy
            int top = topN ?? 10;

            // Lấy tất cả dữ liệu từ đầu (có thể thay đổi theo yêu cầu)
            // Nếu muốn lọc theo khoảng thời gian, có thể thêm tham số tuNgay và denNgay
            DateTime? tuNgay = null; // null = từ đầu
            DateTime? denNgay = DateTime.Now; // đến hiện tại

            // Gọi stored procedure để lấy top sản phẩm bán chạy
            var sanPhamBanChay = db.sp_LayTopSanPhamBanChay(top, tuNgay, denNgay).ToList();

            // Chuẩn bị dữ liệu cho biểu đồ
            var labels = sanPhamBanChay.Select(s => s.MaDongHo).ToArray();
            var data = sanPhamBanChay.Select(s => s.TongSoLuongDaBan ?? 0).ToArray();

            ViewBag.SanPhamBanChay = sanPhamBanChay;
            ViewBag.Labels = labels;
            ViewBag.Data = data;
            ViewBag.TopN = top;

            return View();
        }

        // URL: /ThongKe/DoanhThuTheoNgay
        public ActionResult DoanhThuHomNay(int? page)
        {
            // Lấy ngày hôm nay (chỉ lấy phần ngày, không có giờ)
            DateTime today = DateTime.Now.Date;
            DateTime tomorrow = today.AddDays(1);

            // Lấy tất cả đơn hàng đã giao thành công trong ngày hôm nay
            var donHangHomNay = db.DatHang
                .Where(d => d.NgayDatHang.HasValue &&
                           d.NgayDatHang >= today &&
                           d.NgayDatHang < tomorrow &&
                           d.TrangThaiDonHang == 4) // Chỉ lấy đơn hàng đã giao thành công
                .Include(d => d.KhachHang)
                .OrderByDescending(d => d.NgayDatHang)
                .ToList();

            // Tính tổng doanh thu hôm nay
            decimal tongDoanhThu = donHangHomNay.Sum(d => d.TongTien);

            // Phân trang (10 đơn hàng mỗi trang)
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            int totalItems = donHangHomNay.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var donHangPaged = donHangHomNay
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TongDoanhThu = tongDoanhThu;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(donHangPaged);
        }

        // URL: /ThongKe/DoanhThuTheoNam
        public ActionResult DoanhThuTheoNam(int? year)
        {
            // Mặc định lấy năm hiện tại nếu không có tham số
            int selectedYear = year ?? DateTime.Now.Year;

            // Gọi stored procedure để lấy dữ liệu thống kê
            var thongKeData = db.Sp_ThongKeDoanhThuTheoNam(selectedYear).ToList();

            // Lấy dữ liệu đơn hàng theo tháng trong năm để tính doanh thu theo tháng
            var doanhThuTheoThang = db.DatHang
                .Where(d => d.NgayDatHang.HasValue && 
                           d.NgayDatHang.Value.Year == selectedYear &&
                           d.TrangThaiDonHang == 4) // Chỉ lấy đơn hàng đã giao thành công
                .GroupBy(d => d.NgayDatHang.Value.Month)
                .Select(g => new
                {
                    Thang = g.Key,
                    TongDoanhThu = g.Sum(d => d.TongTien)
                })
                .OrderBy(x => x.Thang)
                .ToList();

            // Tạo mảng doanh thu cho 12 tháng
            decimal[] doanhThuThang = new decimal[12];
            foreach (var item in doanhThuTheoThang)
            {
                doanhThuThang[item.Thang - 1] = item.TongDoanhThu;
            }

            // Tên các tháng
            string[] tenThang = new string[] 
            { 
                "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", 
                "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", 
                "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12" 
            };

            ViewBag.Nam = selectedYear;
            ViewBag.TenThang = tenThang;
            ViewBag.DoanhThuThang = doanhThuThang;
            ViewBag.TongDoanhThu = doanhThuThang.Sum();

            return View();
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