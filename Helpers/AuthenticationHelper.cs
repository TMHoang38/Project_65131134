using System;
using System.Web;
using Project_65131134.Models;

namespace Project_65131134.Helpers
{
    public class AuthenticationHelper
    {
        // Session keys
        private const string SESSION_USER_ID = "UserId";
        private const string SESSION_USER_NAME = "UserName";
        private const string SESSION_USER_ROLE = "UserRole";
        private const string SESSION_USER_TYPE = "UserType"; // "NhanVien" or "KhachHang"
        private const string SESSION_FULL_NAME = "FullName";

        // User types
        public const string USER_TYPE_NHANVIEN = "NhanVien";
        public const string USER_TYPE_KHACHHANG = "KhachHang";

        // Role values for NhanVien
        public const int ROLE_ADMIN = 0;           // Quản lý
        public const int ROLE_NHANVIEN_BANHANG = 1; // Nhân viên bán hàng
        public const int ROLE_NHANVIEN_GIAOHANG = 2; // Nhân viên giao hàng

        /// <summary>
        /// Đăng nhập cho nhân viên
        /// </summary>
        public static void LoginNhanVien(NhanVien nhanVien)
        {
            HttpContext.Current.Session[SESSION_USER_ID] = nhanVien.MaNhanVien;
            HttpContext.Current.Session[SESSION_USER_NAME] = nhanVien.TenDangNhap;
            HttpContext.Current.Session[SESSION_USER_ROLE] = nhanVien.VaiTro;
            HttpContext.Current.Session[SESSION_USER_TYPE] = USER_TYPE_NHANVIEN;
            HttpContext.Current.Session[SESSION_FULL_NAME] = nhanVien.HoTen;
        }

        /// <summary>
        /// Đăng nhập cho khách hàng
        /// </summary>
        public static void LoginKhachHang(KhachHang khachHang)
        {
            HttpContext.Current.Session[SESSION_USER_ID] = khachHang.MaKhachHang;
            HttpContext.Current.Session[SESSION_USER_NAME] = khachHang.TenDangNhap;
            HttpContext.Current.Session[SESSION_USER_ROLE] = -1; // Khách hàng không có vai trò
            HttpContext.Current.Session[SESSION_USER_TYPE] = USER_TYPE_KHACHHANG;
            HttpContext.Current.Session[SESSION_FULL_NAME] = khachHang.HoTen;
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        public static void Logout()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }

        /// <summary>
        /// Kiểm tra đã đăng nhập chưa
        /// </summary>
        public static bool IsLoggedIn()
        {
            return HttpContext.Current.Session[SESSION_USER_ID] != null;
        }

        /// <summary>
        /// Lấy ID người dùng hiện tại
        /// </summary>
        public static string GetUserId()
        {
            return HttpContext.Current.Session[SESSION_USER_ID]?.ToString();
        }

        /// <summary>
        /// Lấy tên đăng nhập hiện tại
        /// </summary>
        public static string GetUserName()
        {
            return HttpContext.Current.Session[SESSION_USER_NAME]?.ToString();
        }

        /// <summary>
        /// Lấy vai trò hiện tại (chỉ cho nhân viên)
        /// </summary>
        public static int? GetUserRole()
        {
            if (HttpContext.Current.Session[SESSION_USER_ROLE] != null)
            {
                return (int)HttpContext.Current.Session[SESSION_USER_ROLE];
            }
            return null;
        }

        /// <summary>
        /// Lấy loại người dùng (NhanVien hoặc KhachHang)
        /// </summary>
        public static string GetUserType()
        {
            return HttpContext.Current.Session[SESSION_USER_TYPE]?.ToString();
        }

        /// <summary>
        /// Lấy họ tên người dùng hiện tại
        /// </summary>
        public static string GetFullName()
        {
            return HttpContext.Current.Session[SESSION_FULL_NAME]?.ToString();
        }

        /// <summary>
        /// Kiểm tra có phải Admin không
        /// </summary>
        public static bool IsAdmin()
        {
            return GetUserType() == USER_TYPE_NHANVIEN && GetUserRole() == ROLE_ADMIN;
        }

        /// <summary>
        /// Kiểm tra có phải Nhân viên bán hàng không
        /// </summary>
        public static bool IsNhanVienBanHang()
        {
            return GetUserType() == USER_TYPE_NHANVIEN && GetUserRole() == ROLE_NHANVIEN_BANHANG;
        }

        /// <summary>
        /// Kiểm tra có phải Nhân viên giao hàng không
        /// </summary>
        public static bool IsNhanVienGiaoHang()
        {
            return GetUserType() == USER_TYPE_NHANVIEN && GetUserRole() == ROLE_NHANVIEN_GIAOHANG;
        }

        /// <summary>
        /// Kiểm tra có phải Khách hàng không
        /// </summary>
        public static bool IsKhachHang()
        {
            return GetUserType() == USER_TYPE_KHACHHANG;
        }
    }
}

