using System;
using System.Collections.Generic;
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
        public ActionResult SanPhamBanChay()
        {
            // Logic lấy dữ liệu Sản phẩm bán chạy
            return View();
        }

        // URL: /ThongKe/DoanhThuTheoNgay
        public ActionResult DoanhThuTrongNgay()
        {
            // Logic lấy dữ liệu Doanh thu trong ngày
            return View();
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