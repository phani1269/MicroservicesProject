using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspnetRunBasics.Areas.Identity.Data;
using AspnetRunBasics.Models;
using AspnetRunBasics.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspnetRunBasics.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICatalogService _catalogService;
        private readonly IBasketService _basketService;
        private readonly UserManager<AspnetRunBasicsUser> _userManager;

        public IndexModel(ICatalogService catalogService, IBasketService basketService
            , UserManager<AspnetRunBasicsUser> userManager)
        {
            _catalogService = catalogService;
            _basketService = basketService;
            _userManager = userManager;
        }

        public IEnumerable<CatalogModel> ProductList { get; set; } = new List<CatalogModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            ProductList = await _catalogService.GetCatalog();
            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(string productId)
        {
            var product = await _catalogService.GetCatalog(productId);

            //  var userName = "swn";

            if (!User.Identity.IsAuthenticated)
                return RedirectToPage("./Account/Login", new { area = "Identity" });

            var userName = await _userManager.GetUserAsync(HttpContext.User);


            var basket = await _basketService.GetBasket(userName.UserName);

            basket.Items.Add(new BasketItemModel
            {
                Color = "black",
                Price = product.Price,
                ProductId = productId,
                ProductName = product.Name,
                Quantity = 1
            });

            var basketUpdated = await _basketService.UpdateBasket(basket); 

            return RedirectToPage("Cart");
        }
    }
}
