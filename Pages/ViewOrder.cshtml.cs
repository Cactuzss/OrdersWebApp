using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TestApp.Pages
{
    public class ViewOrderModel : PageModel
    {
        public string OrderID { get; set; }
        public int ProviderID { get; set; }

        public void OnGet()
        {
            var ctx = PageContext.HttpContext.Request.Query;

            OrderID = ctx["id"];
            ProviderID = int.Parse(ctx["provider"]);
        }
    }
}
