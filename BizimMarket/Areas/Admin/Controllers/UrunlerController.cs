using BizimMarket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BizimMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UrunlerController : Controller
    {
        private readonly BizimMarketContext _db;

        public UrunlerController(BizimMarketContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.Urunler.Include(x => x.Kategori).ToList());
        }

        public IActionResult Yeni()
        {
            ViewBag.Kategoriler = _db.Kategoriler
                .Select(x => new SelectListItem(x.Ad, x.Id.ToString()))
                .ToList();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Yeni(Urun urun)
        {
            if (ModelState.IsValid)
            {
                _db.Add(urun);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Kategoriler = _db.Kategoriler
                .Select(x => new SelectListItem(x.Ad, x.Id.ToString()))
                .ToList();
            return View();
        }
    }
}
