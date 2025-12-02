using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_65131134.Models;

namespace Project_65131134.Areas.Admin
{
    public class DongHo_65131134Controller : Controller
    {
        private CuaHangDongHo_65131134Entities db = new CuaHangDongHo_65131134Entities();

         string LayMaDH()
        {
            var maMax = db.DongHo
                .Select(n => n.MaDongHo)
                .OrderByDescending(m => m)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(maMax) || maMax.Length < 3)
            {
                return "DH001";
            }

            var numericPart = maMax.Substring(2);
            if (!int.TryParse(numericPart, out int currentNumber))
            {
                currentNumber = 0;
            }

            return $"DH{(currentNumber + 1):D3}";
        }

        // GET: Admin/DongHo_65131134
        public ActionResult Index()
        {
            var dongHo = db.DongHo.Include(d => d.LoaiDongHo).Include(d => d.NoiSanXuat).Include(d => d.ThuongHieu);
            return View(dongHo.ToList());
        }

        // GET: Admin/DongHo_65131134/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DongHo dongHo = db.DongHo.Find(id);
            if (dongHo == null)
            {
                return HttpNotFound();
            }
            return View(dongHo);
        }

        // GET: Admin/DongHo_65131134/Create
        public ActionResult Create()
        {
            var model = new DongHo
            {
                MaDongHo = LayMaDH()
            };

            ViewBag.MaLoai = new SelectList(db.LoaiDongHo, "MaLoai", "TenLoai");
            ViewBag.MaNoiSanXuat = new SelectList(db.NoiSanXuat, "MaNoiSanXuat", "TenNoiSanXuat");
            ViewBag.MaThuongHieu = new SelectList(db.ThuongHieu, "MaThuongHieu", "TenThuongHieu");
            return View(model);
        }

        // POST: Admin/DongHo_65131134/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaDongHo,TenDongHo,MaThuongHieu,MaNoiSanXuat,MaLoai,MoTa,GiaBan,GiaNhap,SoLuongTon,HinhAnh,KichThuoc,MauSac,BaoHanh,NgayTao")] DongHo dongHo)
        {
            var AnhDH = Request.Files["Avatar"];
            //Lấy thông tin từ input type=file có tên Avatar
            string postedFileName = System.IO.Path.GetFileName(AnhDH.FileName);
            //Lưu hình đại diện về Server
            var path = Server.MapPath("/Images/" + postedFileName);
            AnhDH.SaveAs(path);
            if (ModelState.IsValid)
            {
                dongHo.MaDongHo = LayMaDH();
                dongHo.HinhAnh = postedFileName;
                db.DongHo.Add(dongHo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (string.IsNullOrEmpty(dongHo.MaDongHo))
            {
                dongHo.MaDongHo = LayMaDH();
            }
            ViewBag.MaLoai = new SelectList(db.LoaiDongHo, "MaLoai", "TenLoai", dongHo.MaLoai);
            ViewBag.MaNoiSanXuat = new SelectList(db.NoiSanXuat, "MaNoiSanXuat", "TenNoiSanXuat", dongHo.MaNoiSanXuat);
            ViewBag.MaThuongHieu = new SelectList(db.ThuongHieu, "MaThuongHieu", "TenThuongHieu", dongHo.MaThuongHieu);
            return View(dongHo);
        }

        // GET: Admin/DongHo_65131134/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DongHo dongHo = db.DongHo.Find(id);
            if (dongHo == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaLoai = new SelectList(db.LoaiDongHo, "MaLoai", "TenLoai", dongHo.MaLoai);
            ViewBag.MaNoiSanXuat = new SelectList(db.NoiSanXuat, "MaNoiSanXuat", "TenNoiSanXuat", dongHo.MaNoiSanXuat);
            ViewBag.MaThuongHieu = new SelectList(db.ThuongHieu, "MaThuongHieu", "TenThuongHieu", dongHo.MaThuongHieu);
            return View(dongHo);
        }

        // POST: Admin/DongHo_65131134/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaDongHo,TenDongHo,MaThuongHieu,MaNoiSanXuat,MaLoai,MoTa,GiaBan,GiaNhap,SoLuongTon,HinhAnh,KichThuoc,MauSac,BaoHanh,NgayTao")] DongHo dongHo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dongHo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaLoai = new SelectList(db.LoaiDongHo, "MaLoai", "TenLoai", dongHo.MaLoai);
            ViewBag.MaNoiSanXuat = new SelectList(db.NoiSanXuat, "MaNoiSanXuat", "TenNoiSanXuat", dongHo.MaNoiSanXuat);
            ViewBag.MaThuongHieu = new SelectList(db.ThuongHieu, "MaThuongHieu", "TenThuongHieu", dongHo.MaThuongHieu);
            return View(dongHo);
        }

        // GET: Admin/DongHo_65131134/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DongHo dongHo = db.DongHo.Find(id);
            if (dongHo == null)
            {
                return HttpNotFound();
            }
            return View(dongHo);
        }

        // POST: Admin/DongHo_65131134/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DongHo dongHo = db.DongHo.Find(id);
            db.DongHo.Remove(dongHo);
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
