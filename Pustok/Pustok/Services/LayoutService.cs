using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.DAL;
using Pustok.Models;
using Pustok.ViewModels;

namespace Pustok.Services
{
    public class LayoutService
    {
        private readonly PustokDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutService(PustokDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public List<Genre> GetGenres()
        {
            return _context.Genres.ToList();
        }

        public Dictionary<string, string> GetSettings()
        {
            return _context.Settings.ToDictionary(x => x.Key, x => x.Value);
        }

        public BasketViewModel GetBasket()
        {
            var basketVM = new BasketViewModel();

            var basketStr = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

            if (basketStr != null)
            {
                List<BasketCookieItemViewModel> cookieItems = JsonConvert.DeserializeObject<List<BasketCookieItemViewModel>>(basketStr);

                foreach (var cookieItem in cookieItems)
                {
                    BasketItemViewModel item = new BasketItemViewModel
                    {
                        Count = cookieItem.Count,
                        Book = _context.Books.Include(x => x.BookImages).FirstOrDefault(x => x.Id == cookieItem.BookId)
                    };
                    basketVM.Items.Add(item);
                    basketVM.TotalAmount += (item.Book.DiscountPercent > 0 ? item.Book.SalePrice * (100 - item.Book.DiscountPercent) / 100 : item.Book.SalePrice) * item.Count;
                }
            }
            return basketVM;
        }
    }
}
