using System;
using System.Web.Mvc;
using System.Data.Entity;
using Project_65131134.Models;

namespace Project_65131134.Helpers
{
    public static class DonHangWorkflowHelper
    {
        /// <summary>
        /// Cập nhật trạng thái đơn hàng dùng chung cho Admin / NV bán hàng / NV giao hàng.
        /// transition: nhận đơn hàng -> trả về trạng thái kế tiếp (null nếu không cho phép).
        /// afterUpdate: hook bổ sung (ví dụ cập nhật NgàyGiaoHang).
        /// </summary>
        public static JsonResult UpdateStatus(
            Controller controller,
            CuaHangDongHo_65131134Entities db,
            string id,
            Func<DatHang, int?> transition,
            Action<DatHang> afterUpdate = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new JsonResult
                {
                    Data = new { success = false, message = "Mã đơn hàng không hợp lệ." },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            var datHang = db.DatHang.Find(id);
            if (datHang == null)
            {
                return new JsonResult
                {
                    Data = new { success = false, message = "Không tìm thấy đơn hàng." },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            var nextStatus = transition(datHang);
            if (!nextStatus.HasValue)
            {
                return new JsonResult
                {
                    Data = new { success = false, message = "Trạng thái hiện tại không thể cập nhật theo yêu cầu." },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            datHang.TrangThaiDonHang = nextStatus.Value;

            // Nếu chuyển sang giao thành công và chưa có ngày giao, set ngày giao
            if (nextStatus.Value == 4 && !datHang.NgayGiaoHang.HasValue)
            {
                datHang.NgayGiaoHang = DateTime.Now;
            }

            afterUpdate?.Invoke(datHang);

            db.Entry(datHang).State = EntityState.Modified;
            db.SaveChanges();

            return new JsonResult
            {
                Data = new { success = true, trangThai = nextStatus.Value },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public static JsonResult Cancel(
            Controller controller,
            CuaHangDongHo_65131134Entities db,
            string id,
            Func<DatHang, bool> canCancel = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new JsonResult
                {
                    Data = new { success = false, message = "Mã đơn hàng không hợp lệ." },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            var datHang = db.DatHang.Find(id);
            if (datHang == null)
            {
                return new JsonResult
                {
                    Data = new { success = false, message = "Không tìm thấy đơn hàng." },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            if (canCancel != null && !canCancel(datHang))
            {
                return new JsonResult
                {
                    Data = new { success = false, message = "Đơn hàng không thể hủy ở trạng thái hiện tại." },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            datHang.TrangThaiDonHang = 5; // Hủy
            db.Entry(datHang).State = EntityState.Modified;
            db.SaveChanges();

            return new JsonResult
            {
                Data = new { success = true },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}

