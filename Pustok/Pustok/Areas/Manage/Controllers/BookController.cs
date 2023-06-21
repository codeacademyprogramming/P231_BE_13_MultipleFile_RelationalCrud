﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Areas.Manage.ViewModels;
using Pustok.DAL;
using Pustok.Helpers;
using Pustok.Models;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class BookController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(PustokDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page=1)
        {
            var query = _context.Books.Include(x => x.Author).Include(x => x.Genre).Include(x => x.BookImages.Where(bi => bi.PosterStatus == true));
            return View(PaginatedList<Book>.Create(query,page,4));
        }

        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();


            return View();
        }

        [HttpPost]
        public IActionResult Create(Book book)
        {
            if (book.PosterFile == null)
                ModelState.AddModelError("PosterFile", "PosterFile is required");

            if (book.HoverPosterFile == null)
                ModelState.AddModelError("HoverPosterFile", "HoverPosterFile is required");

            if (!ModelState.IsValid)
            {
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Genres = _context.Genres.ToList();
                return View();
            }

            if (!_context.Authors.Any(x => x.Id == book.AuthorId))
                return View("error");

            if (!_context.Genres.Any(x => x.Id == book.GenreId))
                return View("error");


            BookImage poster = new BookImage
            {
                PosterStatus = true,
                ImageName = FileManager.Save(book.PosterFile, _env.WebRootPath, "manage/uploads/books"),
            };
            book.BookImages.Add(poster);

            BookImage hoverPoster = new BookImage
            {
                PosterStatus = false,
                ImageName = FileManager.Save(book.HoverPosterFile, _env.WebRootPath, "manage/uploads/books"),
            };
            book.BookImages.Add(hoverPoster);


            foreach (var file in book.ImageFiles)
            {
                BookImage bookImage = new BookImage
                {
                    PosterStatus = null,
                    ImageName = FileManager.Save(file, _env.WebRootPath, "manage/uploads/books"),
                };
                book.BookImages.Add(bookImage);
            }

            foreach (var tagId in book.TagIds)
            {
                if (!_context.Tags.Any(x => x.Id == tagId))
                    return View("error");

                BookTag tag = new BookTag
                {
                    TagId = tagId
                };

                book.BookTags.Add(tag); 
            }

            _context.Books.Add(book);

            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
         

            Book book = _context.Books.Include(x=>x.BookImages).Include(x=>x.BookTags).FirstOrDefault(x => x.Id == id);

            if(book == null) return View("error");

            book.TagIds = book.BookTags.Select(x => x.TagId).ToList();

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            return View(book);
        }
    }
}
