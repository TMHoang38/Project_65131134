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

namespace Project_65131134.Areas.Admin.Controllers
{
    [AdminRole]
    public class ThuongHieu_65131134Controller : Controller
    {
        private CuaHangDongHo_65131134Entities db = new CuaHangDongHo_65131134Entities();

        string LayMaTH()
        {
            var maMax = db.ThuongHieu.Select(n => n.MaThuongHieu).OrderByDescending(m => m).FirstOrDefault();
            if (string.IsNullOrEmpty(maMax) || maMax.Length < 3) return "TH001";
            var numericPart = maMax.Substring(2);
            if (!int.TryParse(numericPart, out int currentNumber)) currentNumber = 0;
            return $"TH{(currentNumber + 1):D3}";
        }

        // --- BỔ SUNG: Hàm kiểm tra trùng tên cho Ajax (Client-side) ---
        public JsonResult CheckTenThuongHieu(string tenThuongHieu)
        {
            // Kiểm tra tồn tại, bỏ qua chữ hoa/thường và khoảng trắng
            bool exists = db.ThuongHieu.Any(x => x.TenThuongHieu.ToLower().Trim() == tenThuongHieu.ToLower().Trim());
            return Json(new { exists = exists }, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/ThuongHieu_65131134
        public ActionResult Index()
        {
            return View(db.ThuongHieu.ToList());
        }

        // GET: Admin/ThuongHieu_65131134/Create
        public ActionResult Create()
        {
            var model = new ThuongHieu
            {
                MaThuongHieu = LayMaTH()
            };
            return View(model);
        }

        // POST: Admin/ThuongHieu_65131134/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaThuongHieu,TenThuongHieu,MoTa")] ThuongHieu thuongHieu)
        {
            if (ModelState.IsValid)
            {
                // --- BỔ SUNG: Kiểm tra trùng tại Server (Server-side) ---
                bool checkTrung = db.ThuongHieu.Any(x => x.TenThuongHieu.ToLower().Trim() == thuongHieu.TenThuongHieu.ToLower().Trim());

                if (checkTrung)
                {
                    ModelState.AddModelError("TenThuongHieu", "Tên thương hiệu này đã tồn tại! Vui lòng chọn tên khác.");

                    // Giữ lại mã cũ để không bị mất khi reload trang
                    if (string.IsNullOrEmpty(thuongHieu.MaThuongHieu))
                    {
                        thuongHieu.MaThuongHieu = LayMaTH();
                    }
                    return View(thuongHieu);
                }
                // ------------------------------------------------------

                thuongHieu.MaThuongHieu = LayMaTH();
                db.ThuongHieu.Add(thuongHieu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(thuongHieu.MaThuongHieu))
            {
                thuongHieu.MaThuongHieu = LayMaTH();
            }
            return View(thuongHieu);
        }

        // ... (Các hàm Details, Edit, Delete giữ nguyên như cũ) ...
        public ActionResult Details(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ThuongHieu thuongHieu = db.ThuongHieu.Find(id);
            if (thuongHieu == null) return HttpNotFound();
            return View(thuongHieu);
        }

        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ThuongHieu thuongHieu = db.ThuongHieu.Find(id);
            if (thuongHieu == null) return HttpNotFound();
            return View(thuongHieu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaThuongHieu,TenThuongHieu,MoTa")] ThuongHieu thuongHieu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thuongHieu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thuongHieu);
        }

        public ActionResult Delete(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ThuongHieu thuongHieu = db.ThuongHieu.Find(id);
            if (thuongHieu == null) return HttpNotFound();
            return View(thuongHieu);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ThuongHieu thuongHieu = db.ThuongHieu.Find(id);
            db.ThuongHieu.Remove(thuongHieu);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}