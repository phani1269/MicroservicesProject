using System;
using System.Linq;
using System.Threading.Tasks;
using AspnetRunBasics.Areas.Identity.Data;
using AspnetRunBasics.Models;
using AspnetRunBasics.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspnetRunBasics
{
    public class CartModel : PageModel
    {
        private readonly IBasketService _basketService;
        private readonly UserManager<AspnetRunBasicsUser> _userManager;

        public CartModel(IBasketService basketService, UserManager<AspnetRunBasicsUser> userManager)
        {
            _basketService = basketService;
            _userManager = userManager;
        }

        public BasketModel Cart { get; set; } = new BasketModel();

        public async Task<IActionResult> OnGetAsync()
        {
            // var userName = "swn";

            if (!User.Identity.IsAuthenticated)
                return RedirectToPage("./Account/Login", new { area = "Identity" });

            var userName =  await _userManager.GetUserAsync(HttpContext.User);

            Cart = await _basketService.GetBasket(userName.UserName);


            return Page();
        }

        public async Task<IActionResult> OnPostRemoveToCartAsync(string productId)
        {
            // var userName = "swn";

            if (!User.Identity.IsAuthenticated)
                return RedirectToPage("./Account/Login", new { area = "Identity" });

            var userName =  await _userManager.GetUserAsync(HttpContext.User);


            var basket = await _basketService.GetBasket(userName.UserName);

            var item = basket.Items.Single(x => x.ProductId == productId);
            basket.Items.Remove(item);

            var basketUpdated = await _basketService.UpdateBasket(basket);

            return RedirectToPage();
        }
    }
}