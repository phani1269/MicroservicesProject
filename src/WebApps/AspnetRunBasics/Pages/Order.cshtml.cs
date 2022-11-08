using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspnetRunBasics.Areas.Identity.Data;
using AspnetRunBasics.Models;
using AspnetRunBasics.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspnetRunBasics
{
    public class OrderModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<AspnetRunBasicsUser> _userManager;

        public OrderModel(IOrderService orderService, UserManager<AspnetRunBasicsUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        public IEnumerable<OrderResponseModel> Orders { get; set; } = new List<OrderResponseModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToPage("./Account/Login", new { area = "Identity" });

            var userName = await _userManager.GetUserAsync(HttpContext.User);


            //Orders = await _orderService.GetOrdersByUserName("swn");

            Orders = await _orderService.GetOrdersByUserName(userName.UserName);

            return Page();
        }
    }
}