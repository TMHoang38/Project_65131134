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
    public class NoiSanXuat_65131134Controller : Controller
    {
        private CuaHangDongHo_65131134Entities db = new CuaHangDongHo_65131134Entities();

        string LayMaNSX()
        {
            var maMax = db.NoiSanXuat.Select(n => n.MaNoiSanXuat).OrderByDescending(m => m).FirstOrDefault();
            if (string.IsNullOrEmpty(maMax) || maMax.Length < 3) return "NSX001";
            var numericPart = maMax.Substring(3);
            if (!int.TryParse(numericPart, out int currentNumber)) currentNumber = 0;
            return $"NSX{(currentNumber + 1):D3}";
        }

        // --- BỔ SUNG: Hàm kiểm tra trùng tên cho Ajax (Client-side) ---
        public JsonResult CheckTenNoiSanXuat(string tenNoiSanXuat)
        {
            // Kiểm tra tồn tại, bỏ qua chữ hoa/thường và khoảng trắng
            bool exists = db.NoiSanXuat.Any(x => x.TenNoiSanXuat.ToLower().Trim() == tenNoiSanXuat.ToLower().Trim());
            return Json(new { exists = exists }, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/NoiSanXuat_65131134
        public ActionResult Index()
        {
            return View(db.NoiSanXuat.ToList());
        }

        // GET: Admin/NoiSanXuat_65131134/Create
        public ActionResult Create()
        {
            var model = new NoiSanXuat
            {
                MaNoiSanXuat = LayMaNSX()
            };
            return View(model);
        }

        // POST: Admin/NoiSanXuat_65131134/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaNoiSanXuat,TenNoiSanXuat,MoTa")] NoiSanXuat noiSanXuat)
        {
            if (ModelState.IsValid)
            {
                // --- BỔ SUNG: Kiểm tra trùng tại Server (Server-side) ---
                bool checkTrung = db.NoiSanXuat.Any(x => x.TenNoiSanXuat.ToLower().Trim() == noiSanXuat.TenNoiSanXuat.ToLower().Trim());

                if (checkTrung)
                {
                    ModelState.AddModelError("TenNoiSanXuat", "Tên nơi sản xuất này đã tồn tại! Vui lòng chọn tên khác.");

                    // Giữ lại mã cũ để không bị mất hiển thị
                    if (string.IsNullOrEmpty(noiSanXuat.MaNoiSanXuat))
                    {
                        noiSanXuat.MaNoiSanXuat = LayMaNSX();
                    }
                    return View(noiSanXuat);
                }
                // ------------------------------------------------------

                noiSanXuat.MaNoiSanXuat = LayMaNSX();
                db.NoiSanXuat.Add(noiSanXuat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(noiSanXuat.MaNoiSanXuat))
            {
                noiSanXuat.MaNoiSanXuat = LayMaNSX();
            }
            return View(noiSanXuat);
        }

        // ... (Các hàm Edit, Details, Delete giữ nguyên như cũ) ...
        public ActionResult Details(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            NoiSanXuat noiSanXuat = db.NoiSanXuat.Find(id);
            if (noiSanXuat == null) return HttpNotFound();
            return View(noiSanXuat);
        }

        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            NoiSanXuat noiSanXuat = db.NoiSanXuat.Find(id);
            if (noiSanXuat == null) return HttpNotFound();
            return View(noiSanXuat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNoiSanXuat,TenNoiSanXuat,MoTa")] NoiSanXuat noiSanXuat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(noiSanXuat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(noiSanXuat);
        }

        public ActionResult Delete(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            NoiSanXuat noiSanXuat = db.NoiSanXuat.Find(id);
            if (noiSanXuat == null) return HttpNotFound();
            return View(noiSanXuat);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            NoiSanXuat noiSanXuat = db.NoiSanXuat.Find(id);
            db.NoiSanXuat.Remove(noiSanXuat);
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