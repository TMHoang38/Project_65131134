using System.Web.Mvc;

namespace Project_65131134.Areas.NhanVienGiaoHang
{
    public class NhanVienGiaoHangAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "NhanVienGiaoHang";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "NhanVienGiaoHang_default",
                "NhanVienGiaoHang/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}