using System.Web.Mvc;

namespace Project_65131134.Areas.Shop
{
    public class ShopAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Shop";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Shop_default",
                "Shop/{controller}/{action}/{id}",
                new { controller = "Home_65131134", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}