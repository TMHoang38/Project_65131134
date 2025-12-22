using System;
using System.Web;
using System.Web.Mvc;
using Project_65131134.Helpers;

namespace Project_65131134.Helpers.Attributes
{
    /// <summary>
    /// Attribute để kiểm tra quyền truy cập Nhân viên bán hàng (VaiTro = 1)
    /// </summary>
    public class NhanVienBanHangRoleAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!AuthenticationHelper.IsLoggedIn())
            {
                return false;
            }

            return AuthenticationHelper.IsNhanVienBanHang();
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
                else if (AuthenticationHelper.IsAdmin())
                {
                    filterContext.Result = new RedirectResult("~/Admin/NhanVien_65131134/Index");
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

