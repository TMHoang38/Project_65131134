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
    public class LoaiDongHo_65131134Controller : Controller
    {
        private CuaHangDongHo_65131134Entities db = new CuaHangDongHo_65131134Entities();

        string LayMaLDH()
        {
            var maMax = db.LoaiDongHo.Select(n => n.MaLoai).OrderByDescending(m => m).FirstOrDefault();
            if (string.IsNullOrEmpty(maMax) || maMax.Length < 3) return "LDH001";
            var numericPart = maMax.Substring(3);
            if (!int.TryParse(numericPart, out int currentNumber)) currentNumber = 0;
            return $"LDH{(currentNumber + 1):D3}";
        }

        // --- BỔ SUNG: Hàm kiểm tra trùng tên cho Ajax (Client-side) ---
        public JsonResult CheckTenLoai(string tenLoai)
        {
            // Kiểm tra tồn tại, bỏ qua chữ hoa/thường và khoảng trắng
            bool exists = db.LoaiDongHo.Any(x => x.TenLoai.ToLower().Trim() == tenLoai.ToLower().Trim());
            return Json(new { exists = exists }, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/LoaiDongHo_65131134
        public ActionResult Index()
        {
            return View(db.LoaiDongHo.ToList());
        }

        // GET: Admin/LoaiDongHo_65131134/Create
        public ActionResult Create()
        {
            var model = new LoaiDongHo
            {
                MaLoai = LayMaLDH()
            };
            return View(model);
        }

        // POST: Admin/LoaiDongHo_65131134/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaLoai,TenLoai,MoTa")] LoaiDongHo loaiDongHo)
        {
            if (ModelState.IsValid)
            {
                // --- BỔ SUNG: Kiểm tra trùng tại Server (Server-side) ---
                bool checkTrung = db.LoaiDongHo.Any(x => x.TenLoai.ToLower().Trim() == loaiDongHo.TenLoai.ToLower().Trim());

                if (checkTrung)
                {
                    ModelState.AddModelError("TenLoai", "Tên loại đồng hồ này đã tồn tại! Vui lòng chọn tên khác.");

                    // Giữ lại mã cũ để không bị mất hiển thị
                    if (string.IsNullOrEmpty(loaiDongHo.MaLoai))
                    {
                        loaiDongHo.MaLoai = LayMaLDH();
                    }
                    return View(loaiDongHo);
                }
                // ------------------------------------------------------

                loaiDongHo.MaLoai = LayMaLDH();
                db.LoaiDongHo.Add(loaiDongHo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(loaiDongHo.MaLoai))
            {
                loaiDongHo.MaLoai = LayMaLDH();
            }
            return View(loaiDongHo);
        }

        // ... (Các hàm Edit, Details, Delete giữ nguyên như cũ) ...
        public ActionResult Details(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            LoaiDongHo loaiDongHo = db.LoaiDongHo.Find(id);
            if (loaiDongHo == null) return HttpNotFound();
            return View(loaiDongHo);
        }

        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            LoaiDongHo loaiDongHo = db.LoaiDongHo.Find(id);
            if (loaiDongHo == null) return HttpNotFound();
            return View(loaiDongHo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaLoai,TenLoai,MoTa")] LoaiDongHo loaiDongHo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loaiDongHo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(loaiDongHo);
        }

        public ActionResult Delete(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            LoaiDongHo loaiDongHo = db.LoaiDongHo.Find(id);
            if (loaiDongHo == null) return HttpNotFound();
            return View(loaiDongHo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            LoaiDongHo loaiDongHo = db.LoaiDongHo.Find(id);
            db.LoaiDongHo.Remove(loaiDongHo);
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