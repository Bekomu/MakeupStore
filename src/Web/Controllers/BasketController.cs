using ApplicationCore.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.Models;

namespace Web.Controllers
{
    public class BasketController : Controller
    {
        private readonly IBasketViewModelService _basketViewModelService;

        public BasketController(IBasketViewModelService basketViewModelService)
        {
            _basketViewModelService = basketViewModelService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _basketViewModelService.GetBasketViewModelAsync());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Dictionary<int, int> quantities)
        {
            if (quantities.Values.Any(x => x < 1))
            {
                TempData["error"] = "We have encountered a problem updating the quantities.";
                return RedirectToAction(nameof(Index));
            }
            TempData["message"] = "Items updated successfully.";
            var basket = await _basketViewModelService.SetQuantitiesAsync(quantities);
            return View(basket);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(int productId, int quantity = 1)
        {
            if (quantity < 1)
            {
                return BadRequest();
            }
            int totalItems = await _basketViewModelService.AddItemToBasketAsync(productId, quantity);
            return Json(new { totalItems });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EmptyBasket()
        {
            await _basketViewModelService.DeleteBasketAsync();
            TempData["message"] = "All items removed from cart successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            await _basketViewModelService.DeleteBasketItemAsync(itemId);
            TempData["message"] = "Item removed from cart successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var vm = new CheckoutViewModel();
            vm.Basket = await _basketViewModelService.GetBasketViewModelAsync();
            return View(vm);
        }

        [Authorize]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var address = new Address()
                {
                    City = vm.City,
                    Country = vm.Country,
                    State = vm.State,
                    Street = vm.Street,
                    ZipCode = vm.ZipCode
                };
                var order = await _basketViewModelService.CompleteCheckoutAsync(address);
                return RedirectToAction(nameof(OrderComplete), new { orderId = order.Id });
            }

            vm.Basket = await _basketViewModelService.GetBasketViewModelAsync();
            return View(vm);
        }

        [Authorize]
        public async Task<IActionResult> OrderComplete(int orderId)
        {
            return View(orderId);
        }
    }
}
