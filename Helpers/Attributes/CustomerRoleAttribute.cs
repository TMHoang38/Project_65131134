using System;
using System.Web;
using System.Web.Mvc;
using Project_65131134.Helpers;

namespace Project_65131134.Helpers.Attributes
{
    /// <summary>
    /// Attribute để kiểm tra quyền truy cập Khách hàng
    /// </summary>
    public class CustomerRoleAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!AuthenticationHelper.IsLoggedIn())
            {
                return false;
            }

            return AuthenticationHelper.IsKhachHang();
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
                // Đã đăng nhập nhưng không phải khách hàng, chuyển đến trang chủ tương ứng
                if (AuthenticationHelper.IsAdmin())
                {
                    filterContext.Result = new RedirectResult("~/Admin/NhanVien_65131134/Index");
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

