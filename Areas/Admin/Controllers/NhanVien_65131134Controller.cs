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
    public class NhanVien_65131134Controller : Controller
    {
        private CuaHangDongHo_65131134Entities db = new CuaHangDongHo_65131134Entities();
        [HttpPost]
        public JsonResult CheckUsername(string username, string currentId)
        {
            
            bool isExists = db.NhanVien.Any(x => x.TenDangNhap == username && x.MaNhanVien != currentId);

            return Json(new { isTaken = isExists });
        }

        [HttpGet]
        public ActionResult TimKiem(string hoTen = "")
        {
            ViewBag.hoTen = hoTen;
            object[] parameters = {
        new System.Data.SqlClient.SqlParameter("@hoTen", hoTen)
    };
            var nhanViens = db.NhanVien.SqlQuery("NhanVien_TimKiem @hoTen", parameters).ToList();

            if (nhanViens.Count == 0)
                ViewBag.TB = "Không có thông tin tìm kiếm.";

            return View("Index", nhanViens);
        }

        string LayMaNV()
        {
            var maMax = db.NhanVien
                .Select(n => n.MaNhanVien)
                .OrderByDescending(m => m)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(maMax) || maMax.Length < 3)
            {
                return "NV001";
            }

            var numericPart = maMax.Substring(2);
            if (!int.TryParse(numericPart, out int currentNumber))
            {
                currentNumber = 0;
            }

            return $"NV{(currentNumber + 1):D3}";
        }


        // GET: Admin/NhanVien_65131134
        public ActionResult Index()
        {
            return View(db.NhanVien.ToList());
        }

        // GET: Admin/NhanVien_65131134/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanVien = db.NhanVien.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }

        // GET: Admin/NhanVien_65131134/Create
        public ActionResult Create()
        {
            var model = new NhanVien
            {
                MaNhanVien = LayMaNV()
            };
            return View(model);
        }

        // POST: Admin/NhanVien_65131134/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaNhanVien,TenDangNhap,MatKhau,HoTen,Email,SoDienThoai,DiaChi,ChucVu,VaiTro,NgayVaoLam,NgayTao")] NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                nhanVien.MaNhanVien = LayMaNV();
                db.NhanVien.Add(nhanVien);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(nhanVien.MaNhanVien))
            {
                nhanVien.MaNhanVien = LayMaNV();
            }
            return View(nhanVien);
        }

        // GET: Admin/NhanVien_65131134/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanVien = db.NhanVien.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }

        // POST: Admin/NhanVien_65131134/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNhanVien,TenDangNhap,MatKhau,HoTen,Email,SoDienThoai,DiaChi,ChucVu,VaiTro,NgayVaoLam,NgayTao")] NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nhanVien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nhanVien);
        }

        // GET: Admin/NhanVien_65131134/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanVien = db.NhanVien.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }

        // POST: Admin/NhanVien_65131134/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            NhanVien nhanVien = db.NhanVien.Find(id);
                db.NhanVien.Remove(nhanVien);
                db.SaveChanges();
            return RedirectToAction("Index");
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