using System;
using System.Web;
using System.Web.Mvc;
using Project_65131134.Helpers;

namespace Project_65131134.Helpers.Attributes
{
    /// <summary>
    /// Attribute để kiểm tra quyền truy cập Admin (VaiTro = 0)
    /// </summary>
    public class AdminRoleAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!AuthenticationHelper.IsLoggedIn())
            {
                return false;
            }

            return AuthenticationHelper.IsAdmin();
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!AuthenticationHelper.IsLoggedIn())
            {
                // Chưa đăng nhập, chuyển đến trang đăng nhập
                filterContext.Result = new RedirectResult("~/TaiKhoan/DangNhap_65131134/Index");
            }
            else
            {
                // Đã đăng nhập nhưng không có quyền, chuyển đến trang chủ tương ứng
                string userType = AuthenticationHelper.GetUserType();
                if (userType == AuthenticationHelper.USER_TYPE_KHACHHANG)
                {
                    filterContext.Result = new RedirectResult("~/Shop/Home_65131134/Index");
                }
                else if (AuthenticationHelper.IsNhanVienBanHang())
                {
                    filterContext.Result = new RedirectResult("~/NhanVienBanHang/DonHang_65131134/Index");
                }
                else if (AuthenticationHelper.IsNhanVienGiaoHang())
                {
                    filterContext.Result = new RedirectResult("~/NhanVienGiaoHang/DonHang_65131134/Index");
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/TaiKhoan/DangNhap_65131134/Index");
                }
            }
        }
    }
}

