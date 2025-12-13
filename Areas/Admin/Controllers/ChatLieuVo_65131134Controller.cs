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
    public class ChatLieuVo_65131134Controller : Controller
    {
        private CuaHangDongHo_65131134Entities db = new CuaHangDongHo_65131134Entities();
        string LayMaCLV()
        {
            var maMax = db.ChatLieuVo
                .Select(n => n.MaChatLieuVo)
                .OrderByDescending(m => m)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(maMax) || maMax.Length < 3)
            {
                return "CLV001";
            }

            var numericPart = maMax.Substring(2);
            if (!int.TryParse(numericPart, out int currentNumber))
            {
                currentNumber = 0;
            }

            return $"CLV{(currentNumber + 1):D3}";
        }

        // GET: Admin/ChatLieuVo_65131134
        public ActionResult Index()
        {
            return View(db.ChatLieuVo.ToList());
        }

        // GET: Admin/ChatLieuVo_65131134/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChatLieuVo chatLieuVo = db.ChatLieuVo.Find(id);
            if (chatLieuVo == null)
            {
                return HttpNotFound();
            }
            return View(chatLieuVo);
        }

        // GET: Admin/ChatLieuVo_65131134/Create
        public ActionResult Create()
        {
            var model = new ChatLieuVo
            {
                MaChatLieuVo = LayMaCLV()
            };
            return View(model);
        }

        // POST: Admin/ChatLieuVo_65131134/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaChatLieuVo,TenChatLieu,MoTa")] ChatLieuVo chatLieuVo)
        {
            if (ModelState.IsValid)
            {
                chatLieuVo.MaChatLieuVo = LayMaCLV();
                db.ChatLieuVo.Add(chatLieuVo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (string.IsNullOrEmpty(chatLieuVo.MaChatLieuVo))
            {
                chatLieuVo.MaChatLieuVo = LayMaCLV();
            }
            return View(chatLieuVo);
        }

        // GET: Admin/ChatLieuVo_65131134/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChatLieuVo chatLieuVo = db.ChatLieuVo.Find(id);
            if (chatLieuVo == null)
            {
                return HttpNotFound();
            }
            return View(chatLieuVo);
        }

        // POST: Admin/ChatLieuVo_65131134/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaChatLieuVo,TenChatLieu,MoTa")] ChatLieuVo chatLieuVo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chatLieuVo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chatLieuVo);
        }

        // GET: Admin/ChatLieuVo_65131134/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChatLieuVo chatLieuVo = db.ChatLieuVo.Find(id);
            if (chatLieuVo == null)
            {
                return HttpNotFound();
            }
            return View(chatLieuVo);
        }

        // POST: Admin/ChatLieuVo_65131134/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ChatLieuVo chatLieuVo = db.ChatLieuVo.Find(id);
            db.ChatLieuVo.Remove(chatLieuVo);
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
