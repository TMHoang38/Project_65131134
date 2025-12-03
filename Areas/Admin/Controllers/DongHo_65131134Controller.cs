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
            // CẬP NHẬT: Include thêm ChatLieuDay và ChatLieuVo
            var dongHo = db.DongHo
                .Include(d => d.LoaiDongHo)
                .Include(d => d.NoiSanXuat)
                .Include(d => d.ThuongHieu)
                .Include(d => d.ChatLieuDay)
                .Include(d => d.ChatLieuVo);
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

            // CẬP NHẬT: Thêm ViewBag cho Dropdown Chất liệu
            ViewBag.MaChatLieuDay = new SelectList(db.ChatLieuDay, "MaChatLieuDay", "TenChatLieu");
            ViewBag.MaChatLieuVo = new SelectList(db.ChatLieuVo, "MaChatLieuVo", "TenChatLieu");

            return View(model);
        }

        // POST: Admin/DongHo_65131134/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // CẬP NHẬT: Thêm MaChatLieuDay, MaChatLieuVo vào Bind
        public ActionResult Create([Bind(Include = "MaDongHo,TenDongHo,MaThuongHieu,MaNoiSanXuat,MaLoai,MoTa,GiaBan,GiaNhap,SoLuongTon,HinhAnh,KichThuoc,MauSac,BaoHanh,NgayTao,MaChatLieuDay,MaChatLieuVo")] DongHo dongHo)
        {
            var AnhDH = Request.Files["Avatar"];

            // Xử lý upload ảnh (nếu có ảnh mới upload)
            if (AnhDH != null && AnhDH.ContentLength > 0)
            {
                string postedFileName = System.IO.Path.GetFileName(AnhDH.FileName);
                var path = Server.MapPath("/Images/" + postedFileName);
                AnhDH.SaveAs(path);
                dongHo.HinhAnh = postedFileName;
            }

            if (ModelState.IsValid)
            {
                dongHo.MaDongHo = LayMaDH(); // Đảm bảo mã luôn mới nhất khi lưu
                db.DongHo.Add(dongHo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu model lỗi, load lại Dropdown
            if (string.IsNullOrEmpty(dongHo.MaDongHo))
            {
                dongHo.MaDongHo = LayMaDH();
            }

            ViewBag.MaLoai = new SelectList(db.LoaiDongHo, "MaLoai", "TenLoai", dongHo.MaLoai);
            ViewBag.MaNoiSanXuat = new SelectList(db.NoiSanXuat, "MaNoiSanXuat", "TenNoiSanXuat", dongHo.MaNoiSanXuat);
            ViewBag.MaThuongHieu = new SelectList(db.ThuongHieu, "MaThuongHieu", "TenThuongHieu", dongHo.MaThuongHieu);

            // CẬP NHẬT: Load lại ViewBag cho Dropdown Chất liệu khi có lỗi
            ViewBag.MaChatLieuDay = new SelectList(db.ChatLieuDay, "MaChatLieuDay", "TenChatLieu", dongHo.MaChatLieuDay);
            ViewBag.MaChatLieuVo = new SelectList(db.ChatLieuVo, "MaChatLieuVo", "TenChatLieu", dongHo.MaChatLieuVo);

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

            // CẬP NHẬT: ViewBag cho Edit
            ViewBag.MaChatLieuDay = new SelectList(db.ChatLieuDay, "MaChatLieuDay", "TenChatLieu", dongHo.MaChatLieuDay);
            ViewBag.MaChatLieuVo = new SelectList(db.ChatLieuVo, "MaChatLieuVo", "TenChatLieu", dongHo.MaChatLieuVo);

            return View(dongHo);
        }

        // POST: Admin/DongHo_65131134/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // CẬP NHẬT: Thêm MaChatLieuDay, MaChatLieuVo vào Bind
        public ActionResult Edit([Bind(Include = "MaDongHo,TenDongHo,MaThuongHieu,MaNoiSanXuat,MaLoai,MoTa,GiaBan,GiaNhap,SoLuongTon,HinhAnh,KichThuoc,MauSac,BaoHanh,NgayTao,MaChatLieuDay,MaChatLieuVo")] DongHo dongHo)
        {
            var AnhDH = Request.Files["Avatar"];
            if (AnhDH != null && AnhDH.ContentLength > 0)
            {
                string postedFileName = System.IO.Path.GetFileName(AnhDH.FileName);
                var path = Server.MapPath("/Images/" + postedFileName);
                AnhDH.SaveAs(path);
                dongHo.HinhAnh = postedFileName;
            }


            if (ModelState.IsValid)
            {
                db.Entry(dongHo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaLoai = new SelectList(db.LoaiDongHo, "MaLoai", "TenLoai", dongHo.MaLoai);
            ViewBag.MaNoiSanXuat = new SelectList(db.NoiSanXuat, "MaNoiSanXuat", "TenNoiSanXuat", dongHo.MaNoiSanXuat);
            ViewBag.MaThuongHieu = new SelectList(db.ThuongHieu, "MaThuongHieu", "TenThuongHieu", dongHo.MaThuongHieu);

            // CẬP NHẬT: Load lại ViewBag Edit khi lỗi
            ViewBag.MaChatLieuDay = new SelectList(db.ChatLieuDay, "MaChatLieuDay", "TenChatLieu", dongHo.MaChatLieuDay);
            ViewBag.MaChatLieuVo = new SelectList(db.ChatLieuVo, "MaChatLieuVo", "TenChatLieu", dongHo.MaChatLieuVo);

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