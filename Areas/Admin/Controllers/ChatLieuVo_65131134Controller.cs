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
    public class ChatLieuVo_65131134Controller : Controller
    {
        private CuaHangDongHo_65131134Entities db = new CuaHangDongHo_65131134Entities();

        // Hàm sinh mã tự động (Giữ nguyên của bạn)
        string LayMaCLV()
        {
            var maMax = db.ChatLieuVo.Select(n => n.MaChatLieuVo).OrderByDescending(m => m).FirstOrDefault();
            if (string.IsNullOrEmpty(maMax) || maMax.Length < 3) return "CLV01";
            var numericPart = maMax.Substring(3);
            if (!int.TryParse(numericPart, out int currentNumber)) currentNumber = 0;
            return $"CLV{(currentNumber + 1):D2}";
        }

        // --- BỔ SUNG: Hàm kiểm tra trùng tên cho Ajax ---
        public JsonResult CheckTenChatLieu(string tenChatLieu)
        {
            // Kiểm tra tồn tại, bỏ qua chữ hoa/thường và khoảng trắng thừa
            bool exists = db.ChatLieuVo.Any(x => x.TenChatLieu.ToLower().Trim() == tenChatLieu.ToLower().Trim());
            return Json(new { exists = exists }, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/ChatLieuVo_65131134
        public ActionResult Index()
        {
            return View(db.ChatLieuVo.ToList());
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaChatLieuVo,TenChatLieu,MoTa")] ChatLieuVo chatLieuVo)
        {
            if (ModelState.IsValid)
            {
                // --- BỔ SUNG: Kiểm tra trùng tại Server (Quan trọng) ---
                bool checkTrung = db.ChatLieuVo.Any(x => x.TenChatLieu.ToLower().Trim() == chatLieuVo.TenChatLieu.ToLower().Trim());
                if (checkTrung)
                {
                    ModelState.AddModelError("TenChatLieu", "Tên chất liệu vỏ này đã tồn tại! Vui lòng chọn tên khác.");
                    if (string.IsNullOrEmpty(chatLieuVo.MaChatLieuVo))
                    {
                        chatLieuVo.MaChatLieuVo = LayMaCLV();
                    }
                    return View(chatLieuVo);
                }
                // ------------------------------------------------------

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

        // ... Các hàm Edit, Details, Delete giữ nguyên như cũ ...
        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ChatLieuVo chatLieuVo = db.ChatLieuVo.Find(id);
            if (chatLieuVo == null) return HttpNotFound();
            return View(chatLieuVo);
        }

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

        public ActionResult Delete(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ChatLieuVo chatLieuVo = db.ChatLieuVo.Find(id);
            if (chatLieuVo == null) return HttpNotFound();
            return View(chatLieuVo);
        }

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
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}