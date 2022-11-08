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
    public class ProductDetailModel : PageModel
    {
        private readonly ICatalogService _catalogService;
        private readonly IBasketService _basketService;
        private readonly UserManager<AspnetRunBasicsUser> _userManager;

        public ProductDetailModel(ICatalogService catalogService, IBasketService basketService
            , UserManager<AspnetRunBasicsUser> userManager)
        {
            _catalogService = catalogService;
            _basketService = basketService;
            _userManager = userManager;
        }

        public CatalogModel Product { get; set; }

        [BindProperty]
        public string Color { get; set; }

        [BindProperty]
        public int Quantity { get; set; }

        public async Task<IActionResult> OnGetAsync(string productId)
        {
            if (productId == null)
            {
                return NotFound();
            }

            Product = await _catalogService.GetCatalog(productId);
            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(string productId)
        {
            var product = await _catalogService.GetCatalog(productId);

            // var userName = "swn";
            if (!User.Identity.IsAuthenticated)
                return RedirectToPage("./Account/Login", new { area = "Identity" });

            var userName =  await _userManager.GetUserAsync(HttpContext.User);

            var basket = await _basketService.GetBasket(userName.UserName);

            basket.Items.Add(new BasketItemModel
            {
                ProductId = productId,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = Quantity,
                Color = Color
            });

            var basketUpdated = await _basketService.UpdateBasket(basket);

            return RedirectToPage("Cart");
        }
    }
}