using BizimMarket.Areas.Admin.Models;
using BizimMarket.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BizimMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UrunlerController : Controller
    {
        private readonly BizimMarketContext _db;
        private readonly IWebHostEnvironment _env;

        public UrunlerController(BizimMarketContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
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
        public IActionResult Yeni(YeniUrunViewModel vm)
        {
            if (ModelState.IsValid)
            {
                #region Resim Kaydetme
                string dosyaAdi = null;
                if (vm.Resim != null)
                {
                    dosyaAdi = Guid.NewGuid() + Path.GetExtension(vm.Resim.FileName);
                    string kaydetmeYolu = Path.Combine(_env.WebRootPath, "img", "urunler", dosyaAdi);
                    using (var fs = new FileStream(kaydetmeYolu, FileMode.Create))
                    {
                        vm.Resim.CopyTo(fs);
                    } 
                }
                #endregion

                Urun urun = new Urun()
                {
                    Ad = vm.Ad,
                    Fiyat = vm.Fiyat.Value,
                    KategoriId = vm.KategoriId.Value,
                    ResimYolu = dosyaAdi
                };
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
