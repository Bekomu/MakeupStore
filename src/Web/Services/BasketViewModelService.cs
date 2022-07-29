using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Extensions;
using Web.Interfaces;
using Web.Models;

namespace Web.Services
{
    public class BasketViewModelService : IBasketViewModelService
    {
        private readonly IBasketService _basketService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderService _orderService;

        public HttpContext HttpContext => _httpContextAccessor.HttpContext;
        public string UserId => HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        public string AnonymousId => HttpContext.Request.Cookies[Constants.BASKET_COOKIENAME];
        public string BuyerId => UserId ?? AnonymousId;

        public BasketViewModelService(IBasketService basketService, IHttpContextAccessor httpContextAccessor, IOrderService orderService)
        {
            _basketService = basketService;
            _httpContextAccessor = httpContextAccessor;
            _orderService = orderService;
        }

        public async Task<int> AddItemToBasketAsync(int productId, int quantity)
        {
            var buyerId = BuyerId ?? CreateAnonymouseId();
            var basket = await _basketService.AddItemToBasketAsync(buyerId, productId, quantity);
            return basket.Items.Sum(x => x.Quantity);
        }

        private string CreateAnonymouseId()
        {
            string newId = Guid.NewGuid().ToString();
            HttpContext.Response.Cookies.Append(Constants.BASKET_COOKIENAME, newId, new CookieOptions()
            {
                Expires = DateTime.Now.AddYears(1),
                IsEssential = true
            });
            return newId;
        }

        public async Task<NavBasketViewModel> GetNavBasketViewModelAsync()
        {
            return new NavBasketViewModel()
            {
                TotalItems = await _basketService.GetBasketItemCountAsync(BuyerId)
            };
        }

        public async Task<BasketViewModel> GetBasketViewModelAsync()
        {
            var basket = await _basketService.GetBasketAsync(BuyerId);
            return basket?.ToBasketViewModel();
        }

        

        public async Task DeleteBasketAsync()
        {
            await _basketService.DeleteBasketAsync(BuyerId);
        }

        public async Task DeleteBasketItemAsync(int basketItemId)
        {
            await _basketService.DeleteBasketItemAsync(BuyerId, basketItemId);
        }

        public async Task<BasketViewModel> SetQuantitiesAsync(Dictionary<int, int> quantities)
        {
            var basket = await _basketService.SetQuantities(BuyerId, quantities);
            return basket?.ToBasketViewModel();
        }

        public async Task<OrderViewModel> CompleteCheckoutAsync(Address address)
        {
            var order = await _orderService.CreateOrderAsync(BuyerId, address);
            await DeleteBasketAsync();

            return new OrderViewModel()
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.Items.Sum(x => x.Quantity * x.UnitPrice)
            };
        }
    }
}
