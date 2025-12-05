using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_65131134.Models;

namespace Project_65131134.Areas.Admin.Controllers
{
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

        // POST: Admin/DonHang_65131134/UpdateStatus
        // Logic: 0 (Chưa xác nhận) -> 1 (Chuẩn bị) -> 3 (Đang giao) -> 4 (Thành công)
        [HttpPost]
        public ActionResult UpdateStatus(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DatHang datHang = db.DatHang.Find(id);
            if (datHang == null)
            {
                return HttpNotFound();
            }

            int currentStatus = datHang.TrangThaiDonHang ?? 0;
            int nextStatus = currentStatus;
            string message = "";

            if (currentStatus == 0)
            {
                nextStatus = 1; // Duyệt đơn -> Chuẩn bị hàng
            }
            else if (currentStatus == 1)
            {
                nextStatus = 3; // Chuẩn bị xong -> Đang giao hàng
            }
            else if (currentStatus == 3)
            {
                nextStatus = 4; // Đang giao -> Giao thành công
                datHang.NgayGiaoHang = DateTime.Now; // Cập nhật ngày giao hàng thực tế
            }
            else
            {
                return Json(new { success = false, message = "Đơn hàng đã hoàn tất hoặc ở trạng thái không thể cập nhật tiếp." });
            }

            datHang.TrangThaiDonHang = nextStatus;

            // Nếu bạn có trường MaNhanVienDuyet, có thể cập nhật người duyệt tại đây nếu cần
            // datHang.MaNhanVienDuyet = ...

            db.Entry(datHang).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true });
        }

        // POST: Admin/DonHang_65131134/CancelOrder
        [HttpPost]
        public ActionResult CancelOrder(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DatHang datHang = db.DatHang.Find(id);
            if (datHang == null)
            {
                return HttpNotFound();
            }

            // Chuyển sang trạng thái hủy đơn hàng (5)
            datHang.TrangThaiDonHang = 5;
            db.Entry(datHang).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true });
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