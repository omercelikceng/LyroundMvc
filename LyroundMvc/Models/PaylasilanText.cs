using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LyroundMVC.Models
{
    [Table("PaylasilanText")]
    public class PaylasilanText
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TextId { get; set; }
        [Required]
        public string TextIcerik { get; set; }
        [Required]
        public int UyeId { get; set; }
        [Required]
        public int SarkiId { get; set; }



        public Uye Uye { get; set; }
        public Sarki Sarki { get; set; }


    }
}