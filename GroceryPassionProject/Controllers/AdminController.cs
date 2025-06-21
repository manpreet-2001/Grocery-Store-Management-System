using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GroceryPassionProject.Controllers
{
    [Authorize(Roles = "Admin")] // ✅ This line restricts access
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View(); // This should render Views/Admin/Index.cshtml
        }
        public IActionResult ManageProducts()
        {
            return View();
        }
        public IActionResult ManageSuppliers()
        {
            return View();
        }

        public IActionResult ManageProductSuppliers()
        {
            return View();
        }
    }
}
