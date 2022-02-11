using BizimMarket.Areas.Admin.Models;
using BizimMarket.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
            KategorileriYukle();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Yeni(YeniUrunViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Urun urun = new Urun()
                {
                    Ad = vm.Ad,
                    Fiyat = vm.Fiyat.Value,
                    KategoriId = vm.KategoriId.Value,
                    ResimYolu = ResimYukle(vm.Resim)
                };
                _db.Add(urun);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            KategorileriYukle();
            return View();
        }

        private string ResimYukle(IFormFile file)
        {
            string dosyaAdi = null;
            if (file != null)
            {
                dosyaAdi = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string kaydetmeYolu = Path.Combine(_env.WebRootPath, "img", "urunler", dosyaAdi);
                using (var fs = new FileStream(kaydetmeYolu, FileMode.Create))
                {
                    file.CopyTo(fs);
                }
            }

            return dosyaAdi;
        }

        public IActionResult Sil(int id)
        {
            var urun = _db.Urunler.Find(id);
            if (urun == null) return NotFound();
            ResimSil(urun.ResimYolu);
            _db.Urunler.Remove(urun);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Duzenle(int id)
        {
            var urun = _db.Urunler.Find(id);
            if (urun == null) return NotFound();

            var vm = new UrunDuzenleViewModel()
            {
                Id = urun.Id,
                Ad = urun.Ad,
                Fiyat = urun.Fiyat,
                KategoriId = urun.KategoriId,
                ResimYolu = urun.ResimYolu
            };

            KategorileriYukle();
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Duzenle(UrunDuzenleViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var urun = _db.Urunler.Find(vm.Id);
                urun.Ad = vm.Ad;
                urun.Fiyat = vm.Fiyat.Value;
                urun.KategoriId = vm.KategoriId.Value;
                if (vm.Resim != null)
                {
                    ResimSil(urun.ResimYolu);
                    urun.ResimYolu = ResimYukle(vm.Resim);
                }
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            KategorileriYukle();
            return View(vm);
        }

        private void KategorileriYukle()
        {
            ViewBag.Kategoriler = _db.Kategoriler
                .Select(x => new SelectListItem(x.Ad, x.Id.ToString()))
                .ToList();
        }

        private void ResimSil(string dosyaAdi)
        {
            if (dosyaAdi == null) return;
            string silmeYolu = Path.Combine(_env.WebRootPath, "img", "urunler", dosyaAdi);
            try
            {
                System.IO.File.Delete(silmeYolu);
            }
            catch (Exception) { }
        }
    }
}
