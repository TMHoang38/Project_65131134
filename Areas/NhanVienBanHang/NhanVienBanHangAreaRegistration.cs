using System.Web.Mvc;

namespace Project_65131134.Areas.NhanVienBanHang
{
    public class NhanVienBanHangAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "NhanVienBanHang";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "NhanVienBanHang_default",
                "NhanVienBanHang/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}