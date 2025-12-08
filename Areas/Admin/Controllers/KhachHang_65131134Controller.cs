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
    public class KhachHang_65131134Controller : Controller
    {
        private CuaHangDongHo_65131134Entities db = new CuaHangDongHo_65131134Entities();

        [HttpGet]
        public ActionResult TimKiem(string hoTen = "")
        {
            ViewBag.hoTen = hoTen;

            // Sử dụng LINQ để tìm kiếm
            var nhanViens = db.KhachHang.AsQueryable();

            if (!string.IsNullOrEmpty(hoTen))
            {
                // Tìm kiếm theo tên có chứa từ khóa (tương đương LIKE %...%)
                nhanViens = nhanViens.Where(x => x.HoTen.Contains(hoTen));
            }

            var result = nhanViens.ToList();

            if (result.Count == 0)
                ViewBag.TB = "Không có thông tin tìm kiếm.";

            return View("Index", result);
        }

        string LayMaKH()
        {
            var maMax = db.KhachHang
                .Select(n => n.MaKhachHang)
                .OrderByDescending(m => m)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(maMax) || maMax.Length < 3)
            {
                return "KH001";
            }

            var numericPart = maMax.Substring(2);
            if (!int.TryParse(numericPart, out int currentNumber))
            {
                currentNumber = 0;
            }

            return $"KH{(currentNumber + 1):D3}";
        }

        // GET: Admin/KhachHang_65131134
        public ActionResult Index()
        {
            return View(db.KhachHang.ToList());
        }

        // GET: Admin/KhachHang_65131134/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = db.KhachHang.Find(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(khachHang);
        }

        // GET: Admin/KhachHang_65131134/Create
        public ActionResult Create()
        {
            var model = new KhachHang
            {
                MaKhachHang = LayMaKH()
            };
            return View(model);
        }

        // POST: Admin/KhachHang_65131134/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaKhachHang,TenDangNhap,MatKhau,HoTen,Email,SoDienThoai,DiaChi,NgayTao,TrangThai")] KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                khachHang.MaKhachHang = LayMaKH();
                db.KhachHang.Add(khachHang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (string.IsNullOrEmpty(khachHang.MaKhachHang))
            {
                khachHang.MaKhachHang = LayMaKH();
            }
            return View(khachHang);
        }

        // GET: Admin/KhachHang_65131134/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = db.KhachHang.Find(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(khachHang);
        }

        // POST: Admin/KhachHang_65131134/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaKhachHang,TenDangNhap,MatKhau,HoTen,Email,SoDienThoai,DiaChi,NgayTao,TrangThai")] KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(khachHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(khachHang);
        }

        // GET: Admin/KhachHang_65131134/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = db.KhachHang.Find(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(khachHang);
        }

        // POST: Admin/KhachHang_65131134/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            KhachHang khachHang = db.KhachHang.Find(id);
            db.KhachHang.Remove(khachHang);
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
