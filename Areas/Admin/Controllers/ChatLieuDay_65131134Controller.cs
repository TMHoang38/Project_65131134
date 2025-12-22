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
    public class ChatLieuDay_65131134Controller : Controller
    {
        private CuaHangDongHo_65131134Entities db = new CuaHangDongHo_65131134Entities();
        string LayMaCLD()
        {
            var maMax = db.ChatLieuDay
                .Select(n => n.MaChatLieuDay)
                .OrderByDescending(m => m)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(maMax) || maMax.Length < 3)
            {
                return "CLD001";
            }

            var numericPart = maMax.Substring(3);
            if (!int.TryParse(numericPart, out int currentNumber))
            {
                currentNumber = 0;
            }

            return $"CLD{(currentNumber + 1):D2}";
        }
        // GET: Admin/ChatLieuDay_65131134
        public ActionResult Index()
        {
            return View(db.ChatLieuDay.ToList());
        }

        // GET: Admin/ChatLieuDay_65131134/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChatLieuDay chatLieuDay = db.ChatLieuDay.Find(id);
            if (chatLieuDay == null)
            {
                return HttpNotFound();
            }
            return View(chatLieuDay);
        }

        // GET: Admin/ChatLieuDay_65131134/Create
        public ActionResult Create()
        {
            var model = new ChatLieuDay
            {
                MaChatLieuDay = LayMaCLD()
            };
            return View(model);
        }

        // POST: Admin/ChatLieuDay_65131134/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaChatLieuDay,TenChatLieu,MoTa")] ChatLieuDay chatLieuDay)
        {
            if (ModelState.IsValid)
            {
                chatLieuDay.MaChatLieuDay = LayMaCLD();
                db.ChatLieuDay.Add(chatLieuDay);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (string.IsNullOrEmpty(chatLieuDay.MaChatLieuDay))
            {
                chatLieuDay.MaChatLieuDay = LayMaCLD();
            }
            return View(chatLieuDay);
        }

        // GET: Admin/ChatLieuDay_65131134/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChatLieuDay chatLieuDay = db.ChatLieuDay.Find(id);
            if (chatLieuDay == null)
            {
                return HttpNotFound();
            }
            return View(chatLieuDay);
        }

        // POST: Admin/ChatLieuDay_65131134/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaChatLieuDay,TenChatLieu,MoTa")] ChatLieuDay chatLieuDay)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chatLieuDay).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chatLieuDay);
        }

        // GET: Admin/ChatLieuDay_65131134/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChatLieuDay chatLieuDay = db.ChatLieuDay.Find(id);
            if (chatLieuDay == null)
            {
                return HttpNotFound();
            }
            return View(chatLieuDay);
        }

        // POST: Admin/ChatLieuDay_65131134/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ChatLieuDay chatLieuDay = db.ChatLieuDay.Find(id);
            db.ChatLieuDay.Remove(chatLieuDay);
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
        // Kiểm tra trùng tên bằng Ajax
        public JsonResult CheckTenChatLieu(string tenChatLieu)
        {
            bool exists = db.ChatLieuDay.Any(x => x.TenChatLieu.ToLower().Trim() == tenChatLieu.ToLower().Trim());
            return Json(new { exists = exists }, JsonRequestBehavior.AllowGet);
        }
    }
}
