using LyroundMVC.Models;
using LyroundMVC.Models.Managers;
using LyroundMVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LyroundMVC.Controllers
{
    public class HomeController : Controller
    {
        public static string durum="";
    
        public ActionResult CikisYap()
        {
            Session.Clear();
            return RedirectToAction("GirisYap");
        }

        public ActionResult Welcome()
        {
            DatabaseContext dbContext = new DatabaseContext();
            if (Session["kullaniciAdi"] != null)
            {
                return RedirectToAction("GirisYap");
            }
            else {
                //Database yaratılıyor. Eğer oluşturulmuşsa database hiç bir iş yapmaz.
                return View();
            }
        }


        public ActionResult GirisYap()
        {
            if (Session["kullaniciAdi"] != null)
            {
                return RedirectToAction("HomePage");
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        public ActionResult GirisYap(GirisYapViewModel model)
        {
            if (ModelState.IsValid) { 
            using (var context = new DatabaseContext())
            {
                var query = context.Uyeler.Where(x => x.KullaniciAdi == model.KullaniciAdi && x.Sifre == model.Sifre).ToList();
                foreach (var item in query)
                {
                    Session["kullaniciAdi"] = model.KullaniciAdi;
                    return RedirectToAction("../Main/HomePage");
                }
                ViewData["Error"] = "Kullanıcı adı ve şifre kontrol ediniz.";
            }
            }
            return View();
        }


        [HttpPost]
        public ActionResult KayitOl(UyeOlViewModel model)
        {
            if (ModelState.IsValid) { 
            DatabaseContext dbContext = new DatabaseContext();
            DateTime suAnkiTarih = DateTime.Now;
            Uye uye = new Uye(model.KullaniciAdi, model.Sifre, model.EMail, model.Ad, model.Soyad,
                suAnkiTarih, 1);
            dbContext.Uyeler.Add(uye);
            dbContext.SaveChanges();
            return RedirectToAction("../Main/HomePage");
            }
            return View();
        }


        public ActionResult KayitOl()
        {

            return View(new UyeOlViewModel());
        }
     
        public int HashTagIdDondur(HashTag hashTag)
        {
            int id = 0;
            using (var context = new DatabaseContext())
            {
                var query = (from p in context.HashTagler
                             where p.HashTagIcerik == hashTag.HashTagIcerik
                             select p).ToList();
                foreach (var professor in query)
                {
                    id = professor.HashTagId;
                }
            }
            return id;
        }
    }
}