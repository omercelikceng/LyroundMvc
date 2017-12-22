using LyroundMVC.Models;
using LyroundMVC.Models.Managers;
using LyroundMVC.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LyroundMVC.Controllers
{
    public class ProfilController : Controller
    {
        [HttpPost]
        public ActionResult ProfilDuzenle(SarkiEkleViewModel model)
        {
            string sarkiYolu = "";
            if (model.ImageFile != null)
            {
                string dosyaAdi = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                string extensions = Path.GetExtension(model.ImageFile.FileName);
                dosyaAdi = dosyaAdi + extensions;

                sarkiYolu = "/UploadImages/" + dosyaAdi;
                dosyaAdi = Path.Combine(Server.MapPath("/UploadImages/"), dosyaAdi);
                model.ImageFile.SaveAs(dosyaAdi);

            }

            string kullaniciAdiCek = Session["kullaniciAdi"].ToString();
            using (var db = new DatabaseContext())
            {
                Uye u = db.Uyeler.Where(x => x.KullaniciAdi == kullaniciAdiCek).FirstOrDefault();
                u.Ad = model.Uyeler.Ad;
                u.Soyad = model.Uyeler.Soyad;
                u.Sifre = model.Uyeler.Sifre;

                db.Entry(u).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                UyeResim uyeResim = db.UyeResimleri.Where(x => x.UyeId == u.UyeId).FirstOrDefault();

                if (uyeResim != null && model.ImageFile != null)
                {
                    uyeResim.UyeResimYolu = sarkiYolu;
                    db.Entry(uyeResim).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else if (uyeResim == null && model.ImageFile != null)
                {
                    UyeResim uyeResim2 = new UyeResim();
                    uyeResim2.UyeId = u.UyeId;
                    uyeResim2.UyeResimYolu = sarkiYolu;
                    db.UyeResimleri.Add(uyeResim2);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("ProfilDuzenle");
        }


        public ActionResult ProfilDuzenle()
        {
            SarkiEkleViewModel profil = new SarkiEkleViewModel();
            using (var db = new DatabaseContext())
            {
                string kullaniciAdiCek = Session["kullaniciAdi"].ToString();
                Uye u = db.Uyeler.Where(x => x.KullaniciAdi == kullaniciAdiCek).FirstOrDefault();
                profil.Uyeler = u;
                profil.UyeResim = db.UyeResimleri.Where(x => x.UyeId == u.UyeId).FirstOrDefault();
            }
            return View(profil);
        }

        public ActionResult Profilim()
        {
            SarkiEkleViewModel profil = new SarkiEkleViewModel();
            using (var db = new DatabaseContext())
            {
                string kullaniciAdiCek = Session["kullaniciAdi"].ToString();
                Uye u = db.Uyeler.Where(x => x.KullaniciAdi == kullaniciAdiCek).FirstOrDefault();
                profil.Uyeler = u;
                profil.ArkadasList = db.Arkadaslar.Where(x => x.UyeId == u.UyeId).ToList();
                profil.PaylasilanTextList = db.PaylasilanTextler.Where(x => x.UyeId == u.UyeId).ToList();
            }
            return View(profil);
        }



    }
}