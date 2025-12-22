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

namespace Project_65131134.Areas.NhanVienGiaoHang.Controllers
{
    [NhanVienGiaoHangRole]
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

        // POST: NhanVienGiaoHang/DonHang_65131134/UpdateStatus
        // Logic cho NV Giao Hàng: chỉ xử lý giao -> hoàn tất
        [HttpPost]
        public ActionResult UpdateStatus(string id)
        {
            return DonHangWorkflowHelper.UpdateStatus(this, db, id, (d) =>
            {
                var current = d.TrangThaiDonHang ?? 0;
                if (current == 3) return 4; // Đang giao -> Giao thành công
                return (int?)null;
            });
        }

        // POST: NhanVienGiaoHang/DonHang_65131134/CancelOrder
        [HttpPost]
        public ActionResult CancelOrder(string id)
        {
            // Cho phép hủy khi đang giao
            return DonHangWorkflowHelper.Cancel(this, db, id, d => (d.TrangThaiDonHang ?? 0) == 3);
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