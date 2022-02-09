using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BizimMarket.Models
{
    public class Urun
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Ad { get; set; }

        public decimal Fiyat { get; set; }

        public string ResimYolu { get; set; }


        [Required(ErrorMessage = "Kategori alanı zorunludur.")]
        public int? KategoriId { get; set; }
        public Kategori Kategori { get; set; }
    }
}
