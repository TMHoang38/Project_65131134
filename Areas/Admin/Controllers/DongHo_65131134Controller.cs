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

namespace Project_65131134.Areas.Admin
{
    [AdminRole]
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
        public ActionResult Index(string searchTerm, string brandId, string priceRange)
        {
            // CẬP NHẬT: Include thêm ChatLieuDay và ChatLieuVo
            var dongHoQuery = db.DongHo
                .Include(d => d.LoaiDongHo)
                .Include(d => d.NoiSanXuat)
                .Include(d => d.ThuongHieu)
                .Include(d => d.ChatLieuDay)
                .Include(d => d.ChatLieuVo);

            // Lọc theo tên sản phẩm
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var keyword = searchTerm.Trim();
                dongHoQuery = dongHoQuery.Where(d => d.TenDongHo.Contains(keyword));
            }

            // Lọc theo thương hiệu
            if (!string.IsNullOrWhiteSpace(brandId))
            {
                dongHoQuery = dongHoQuery.Where(d => d.MaThuongHieu == brandId);
            }

            // Lọc theo khoảng giá
            if (!string.IsNullOrWhiteSpace(priceRange))
            {
                switch (priceRange)
                {
                    case "lt1":
                        dongHoQuery = dongHoQuery.Where(d => d.GiaBan < 1000000);
                        break;
                    case "1to3":
                        dongHoQuery = dongHoQuery.Where(d => d.GiaBan >= 1000000 && d.GiaBan <= 3000000);
                        break;
                    case "3to5":
                        dongHoQuery = dongHoQuery.Where(d => d.GiaBan > 3000000 && d.GiaBan <= 5000000);
                        break;
                    case "5to10":
                        dongHoQuery = dongHoQuery.Where(d => d.GiaBan > 5000000 && d.GiaBan <= 10000000);
                        break;
                    case "gt10":
                        dongHoQuery = dongHoQuery.Where(d => d.GiaBan > 10000000);
                        break;
                }
            }

            // Dropdown thương hiệu
            var thuongHieus = db.ThuongHieu
                .OrderBy(t => t.TenThuongHieu)
                .ToList();
            ViewBag.BrandList = new SelectList(thuongHieus, "MaThuongHieu", "TenThuongHieu", brandId);

            // Lưu lại giá trị filter để hiển thị trên View
            ViewBag.SearchTerm = searchTerm;
            ViewBag.PriceRange = priceRange;

            return View(dongHoQuery.ToList());
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
        // POST: Admin/DongHo_65131134/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaDongHo,TenDongHo,MaThuongHieu,MaNoiSanXuat,MaLoai,MoTa,GiaBan,GiaNhap,SoLuongTon,HinhAnh,KichThuoc,MauSac,BaoHanh,NgayTao,MaChatLieuDay,MaChatLieuVo")] DongHo dongHo)
        {
            var AnhDH = Request.Files["Avatar"];

            // Xử lý upload ảnh
            if (AnhDH != null && AnhDH.ContentLength > 0)
            {
                string postedFileName = System.IO.Path.GetFileName(AnhDH.FileName);
                var path = Server.MapPath("/Images/" + postedFileName);
                AnhDH.SaveAs(path);
                dongHo.HinhAnh = postedFileName;
            }

            // --- BẮT ĐẦU ĐOẠN CODE THÊM MỚI ---
            // Kiểm tra tên đồng hồ đã tồn tại chưa (So sánh không phân biệt hoa thường)
            if (db.DongHo.Any(x => x.TenDongHo.ToLower() == dongHo.TenDongHo.ToLower()))
            {
                // 1. Thêm lỗi vào ModelState để hiện đỏ ở ô input
                ModelState.AddModelError("TenDongHo", "Tên đồng hồ này đã tồn tại trong hệ thống!");

                // 2. Thêm thông báo vào ViewBag để hiện Toast Script
                ViewBag.ErrorToast = "Đồng hồ này đã có trong hệ thống! Vui lòng kiểm tra lại tên.";
            }
            // --- KẾT THÚC ĐOẠN CODE THÊM MỚI ---

            if (ModelState.IsValid)
            {
                dongHo.MaDongHo = LayMaDH();
                db.DongHo.Add(dongHo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // ... (Phần load lại ViewBag cũ giữ nguyên) ...
            if (string.IsNullOrEmpty(dongHo.MaDongHo))
            {
                dongHo.MaDongHo = LayMaDH();
            }

            ViewBag.MaLoai = new SelectList(db.LoaiDongHo, "MaLoai", "TenLoai", dongHo.MaLoai);
            ViewBag.MaNoiSanXuat = new SelectList(db.NoiSanXuat, "MaNoiSanXuat", "TenNoiSanXuat", dongHo.MaNoiSanXuat);
            ViewBag.MaThuongHieu = new SelectList(db.ThuongHieu, "MaThuongHieu", "TenThuongHieu", dongHo.MaThuongHieu);
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