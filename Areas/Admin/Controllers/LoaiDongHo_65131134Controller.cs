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
    public class LoaiDongHo_65131134Controller : Controller
    {
        private CuaHangDongHo_65131134Entities db = new CuaHangDongHo_65131134Entities();

        string LayMaLDH()
        {
            var maMax = db.LoaiDongHo
                .Select(n => n.MaLoai)
                .OrderByDescending(m => m)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(maMax) || maMax.Length < 3)
            {
                return "LDH001";
            }

            var numericPart = maMax.Substring(2);
            if (!int.TryParse(numericPart, out int currentNumber))
            {
                currentNumber = 0;
            }

            return $"LDH{(currentNumber + 1):D3}";
        }
        // GET: Admin/LoaiDongHo_65131134
        public ActionResult Index()
        {
            return View(db.LoaiDongHo.ToList());
        }

        // GET: Admin/LoaiDongHo_65131134/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoaiDongHo loaiDongHo = db.LoaiDongHo.Find(id);
            if (loaiDongHo == null)
            {
                return HttpNotFound();
            }
            return View(loaiDongHo);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaLoai,TenLoai,MoTa")] LoaiDongHo loaiDongHo)
        {
            if (ModelState.IsValid)
            {
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

        // GET: Admin/LoaiDongHo_65131134/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoaiDongHo loaiDongHo = db.LoaiDongHo.Find(id);
            if (loaiDongHo == null)
            {
                return HttpNotFound();
            }
            return View(loaiDongHo);
        }

        // POST: Admin/LoaiDongHo_65131134/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Admin/LoaiDongHo_65131134/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoaiDongHo loaiDongHo = db.LoaiDongHo.Find(id);
            if (loaiDongHo == null)
            {
                return HttpNotFound();
            }
            return View(loaiDongHo);
        }

        // POST: Admin/LoaiDongHo_65131134/Delete/5
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
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
