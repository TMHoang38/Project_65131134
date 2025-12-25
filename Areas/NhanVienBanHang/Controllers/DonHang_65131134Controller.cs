using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_65131134.Models;
using Project_65131134.Helpers.Attributes;
using Project_65131134.Helpers;

namespace Project_65131134.Areas.NhanVienBanHang.Controllers
{
    [NhanVienBanHangRole]
    public class DonHang_65131134Controller : Controller
    {
        private CuaHangDongHo_65131134Entities db = new CuaHangDongHo_65131134Entities();

        // GET: Admin/DonHang_65131134
        public ActionResult Index(int? trangThai)
        {
            var datHang = db.DatHang.Include(d => d.KhachHang).Include(d => d.NhanVien).Include(d => d.NhanVien1);

            // Nếu có filter theo trạng thái
            if (trangThai.HasValue)
            {
                datHang = datHang.Where(d => d.TrangThaiDonHang == trangThai.Value);
            }

            ViewBag.TrangThaiHienTai = trangThai;
            // Sắp xếp đơn hàng mới nhất lên đầu
            return View(datHang.OrderByDescending(x => x.NgayDatHang).ToList());
        }

        // GET: Admin/DonHang_65131134/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Load đầy đủ dữ liệu bao gồm chi tiết đơn hàng và sản phẩm
            DatHang datHang = db.DatHang
                .Include(d => d.KhachHang)
                .Include(d => d.NhanVien)
                .Include(d => d.NhanVien1)
                .Include("ChiTietDatHang")
                .Include("ChiTietDatHang.DongHo")
                .FirstOrDefault(d => d.MaDonHang == id);

            if (datHang == null)
            {
                return HttpNotFound();
            }
            return View(datHang);
        }

        // POST: NhanVienBanHang/DonHang_65131134/UpdateStatus
        // Logic: Nhân viên bán hàng chỉ có thể:
        // - 0 (Chờ xác nhận) -> 1 (Chuẩn bị hàng)
        // - 1 (Chuẩn bị hàng) -> 3 (Đang giao)
        [HttpPost]
        public ActionResult UpdateStatus(string id)
        {
            return DonHangWorkflowHelper.UpdateStatus(this, db, id, (d) =>
            {
                var current = d.TrangThaiDonHang ?? 0;
                if (current == 0) return 1; // Chờ xác nhận -> Chuẩn bị hàng
                if (current == 1) return 3;  // Chuẩn bị hàng -> Đang giao
                return (int?)null; // Không cho phép chuyển trạng thái khác
            });
        }

        // POST: Admin/DonHang_65131134/CancelOrder
        [HttpPost]
        public ActionResult CancelOrder(string id)
        {
            return DonHangWorkflowHelper.Cancel(this, db, id, d => (d.TrangThaiDonHang ?? 0) < 4);
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