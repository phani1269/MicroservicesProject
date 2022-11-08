using System;
using System.Threading.Tasks;
using AspnetRunBasics.Areas.Identity.Data;
using AspnetRunBasics.Models;
using AspnetRunBasics.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspnetRunBasics
{
    public class CheckOutModel : PageModel
    {
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;
        private readonly UserManager<AspnetRunBasicsUser> _userManager;

        public CheckOutModel(IBasketService basketService, IOrderService orderService
            , UserManager<AspnetRunBasicsUser> userManager)
        {
            _basketService = basketService;
            _orderService = orderService;
            _userManager = userManager;
        }

        [BindProperty]
        public BasketCheckoutModel Order { get; set; }

        public BasketModel Cart { get; set; } = new BasketModel();

        public async Task<IActionResult> OnGetAsync()
        {
            //var userName = "swn";
            if (!User.Identity.IsAuthenticated)
                return RedirectToPage("./Account/Login", new { area = "Identity" });

            var userName = await _userManager.GetUserAsync(HttpContext.User);

            Cart = await _basketService.GetBasket(userName.UserName);

            return Page();
        }

        public async Task<IActionResult> OnPostCheckOutAsync()
        {
            // var userName = "swn";

            if (!User.Identity.IsAuthenticated)
                return RedirectToPage("./Account/Login", new { area = "Identity" });

            var userName = await _userManager.GetUserAsync(HttpContext.User);

            Cart = await _basketService.GetBasket(userName.UserName);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Order.UserName = userName.UserName; 
            Order.TotalPrice = Cart.TotalPrice;

            await _basketService.CheckoutBasket(Order);

            return RedirectToPage("Confirmation", "OrderSubmitted");
        }
    }
}