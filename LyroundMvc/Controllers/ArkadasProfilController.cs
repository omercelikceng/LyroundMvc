using LyroundMVC.Models;
using LyroundMVC.Models.Managers;
using LyroundMVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LyroundMVC.Controllers
{
    public class ArkadasProfilController : Controller
    {
        [HttpPost]
        public ActionResult Profil(int id, SarkiEkleViewModel model)
        {
            DateTime date = DateTime.Now;
            using (var db = new DatabaseContext())
            {
                var kullaniciAdi = Session["kullaniciAdi"].ToString();
                Uye u = db.Uyeler.Where(x => x.KullaniciAdi == kullaniciAdi).FirstOrDefault();
                Arkadas arkadas = new Arkadas();
                arkadas.UyeId = u.UyeId;
                arkadas.UyeArkadasId = id;
                arkadas.ArkadaslikTarihi = date;
                db.Arkadaslar.Add(arkadas);
                db.SaveChanges();

            }
            return RedirectToAction("Profil");
        }

        public ActionResult Profil(int id)
        {
            SarkiEkleViewModel sarki = new SarkiEkleViewModel();
            using (var db = new DatabaseContext())
            {
                Uye u = db.Uyeler.Where(x => x.UyeId == id).FirstOrDefault();
                sarki.Uyeler = u;
                sarki.ArkadasList = db.Arkadaslar.Where(x => x.UyeId == u.UyeId).ToList();
                sarki.PaylasilanTextList = db.PaylasilanTextler.Where(x => x.UyeId == u.UyeId).ToList();
            }
            return View(sarki);
        }


        public ActionResult ArkadasGoster(SarkiEkleViewModel model)
        {
            var errMsg = TempData["Uyeler"] as SarkiEkleViewModel;
            return View(errMsg);
        }

    }
}