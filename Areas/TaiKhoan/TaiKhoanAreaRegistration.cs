using System.Web.Mvc;

namespace Project_65131134.Areas.TaiKhoan
{
    public class TaiKhoanAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "TaiKhoan";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "TaiKhoan_default",
                "TaiKhoan/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}