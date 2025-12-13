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
    public class NoiSanXuat_65131134Controller : Controller
    {
        private CuaHangDongHo_65131134Entities db = new CuaHangDongHo_65131134Entities();
        string LayMaNSX()
        {
            var maMax = db.NoiSanXuat
                .Select(n => n.MaNoiSanXuat)
                .OrderByDescending(m => m)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(maMax) || maMax.Length < 3)
            {
                return "NSX001";
            }

            var numericPart = maMax.Substring(2);
            if (!int.TryParse(numericPart, out int currentNumber))
            {
                currentNumber = 0;
            }

            return $"NSX{(currentNumber + 1):D3}";
        }
        // GET: Admin/NoiSanXuat_65131134
        public ActionResult Index()
        {
            return View(db.NoiSanXuat.ToList());
        }

        // GET: Admin/NoiSanXuat_65131134/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NoiSanXuat noiSanXuat = db.NoiSanXuat.Find(id);
            if (noiSanXuat == null)
            {
                return HttpNotFound();
            }
            return View(noiSanXuat);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaNoiSanXuat,TenNoiSanXuat,MoTa")] NoiSanXuat noiSanXuat)
        {
            if (ModelState.IsValid)
            {
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

        // GET: Admin/NoiSanXuat_65131134/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NoiSanXuat noiSanXuat = db.NoiSanXuat.Find(id);
            if (noiSanXuat == null)
            {
                return HttpNotFound();
            }
            return View(noiSanXuat);
        }

        // POST: Admin/NoiSanXuat_65131134/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Admin/NoiSanXuat_65131134/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NoiSanXuat noiSanXuat = db.NoiSanXuat.Find(id);
            if (noiSanXuat == null)
            {
                return HttpNotFound();
            }
            return View(noiSanXuat);
        }

        // POST: Admin/NoiSanXuat_65131134/Delete/5
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
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
