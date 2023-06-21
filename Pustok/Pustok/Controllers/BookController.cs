using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using Pustok.DAL;
using Pustok.Models;
using Pustok.ViewModels;

namespace Pustok.Controllers
{
    public class BookController : Controller
    {
        private readonly PustokDbContext _context;

        public BookController(PustokDbContext context)
        {
            _context = context;
        }
        public IActionResult GetDetail(int id)
        {
            Book book = _context.Books.Include(x=>x.BookImages).FirstOrDefault(x => x.Id == id);
            return Json(book);
            //return PartialView("_BookModalPartial", book);
        }

        public IActionResult AddToBasket(int id)
        {
            var basketStr = Request.Cookies["basket"];

            List<BasketCookieItemViewModel> cookieItems = null;

            if (basketStr == null)
                cookieItems = new List<BasketCookieItemViewModel>();
            else
                cookieItems = JsonConvert.DeserializeObject<List<BasketCookieItemViewModel>>(basketStr);


            BasketCookieItemViewModel cookieItem = cookieItems.FirstOrDefault(x => x.BookId == id);
            if (cookieItem == null)
            {
                cookieItem = new BasketCookieItemViewModel
                {
                    BookId = id,
                    Count = 1
                };
                cookieItems.Add(cookieItem);
            }
            else
                cookieItem.Count++;
           
            HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(cookieItems));

            BasketViewModel basketVM = new BasketViewModel();
            foreach (var ci in cookieItems)
            {
                BasketItemViewModel item = new BasketItemViewModel
                {
                    Count = ci.Count,
                    Book = _context.Books.Include(x => x.BookImages.Where(x => x.PosterStatus == true)).FirstOrDefault(x => x.Id == ci.BookId)
                };
                basketVM.Items.Add(item);
                basketVM.TotalAmount += (item.Book.DiscountPercent > 0 ? item.Book.SalePrice * (100 - item.Book.DiscountPercent) / 100 : item.Book.SalePrice) * item.Count;
            }


            return PartialView("_BasketPartial", basketVM);
        }

        public IActionResult ShowBasket()
        {
            var dataStr = HttpContext.Request.Cookies["basket"];
            var data = JsonConvert.DeserializeObject<List<BasketCookieItemViewModel>>(dataStr);
            return Json(data);
        }
    }
}
