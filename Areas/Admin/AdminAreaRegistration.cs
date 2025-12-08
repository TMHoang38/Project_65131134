using System.Web.Mvc;

namespace Project_65131134.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "ThongKe_65131134", action = "DoanhThuHomNay", id = UrlParameter.Optional }
            );
        }
    }
}