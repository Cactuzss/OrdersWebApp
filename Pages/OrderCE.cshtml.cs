using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TestApp.Pages
{
    public class OrderCEModel : PageModel
    {
        public int OrderID { get; set; }
        public void OnGet()
        {
            var ctx = PageContext.HttpContext.Request.Query;

            OrderID = int.Parse(ctx["id"]);
        }
    }
}
