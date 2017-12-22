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
    public class MainController : Controller
    {
        public ActionResult HomePage()
        {
            if (Session["kullaniciAdi"] != null)
            {
                using (var db = new DatabaseContext())
                {
                    SarkiEkleViewModel sarki = new SarkiEkleViewModel();
                    if (Session["durum"] == null)
                    {
                        sarki.SarkilarList = db.Sarkilar.ToList();
                    }
                    else
                    {


                        if (Session["durum"].ToString() == "Tamamlanmis")
                        {
                            sarki.SarkilarList = db.Sarkilar.Where(x => x.TamamlandiMi == true).ToList();
                        }
                        else if (Session["durum"].ToString() == "Tamamlanmamis")
                        {
                            sarki.SarkilarList = db.Sarkilar.Where(x => x.TamamlandiMi == false).ToList();
                        }
                        else
                        {
                            sarki.SarkilarList = db.Sarkilar.ToList();
                        }

                    }
                    return View(sarki);
                }
            }
            else
            {
                return RedirectToAction("../Home/GirisYap");
            }
        }


        [HttpPost]
        public ActionResult HomePage(SarkiEkleViewModel model)
        {


            string dosyaAdi = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
            string extensions = Path.GetExtension(model.ImageFile.FileName);
            dosyaAdi = dosyaAdi + extensions;

            string sarkiYolu = "/UploadImages/" + dosyaAdi;
            dosyaAdi = Path.Combine(Server.MapPath("/UploadImages/"), dosyaAdi);
            model.ImageFile.SaveAs(dosyaAdi);

            SarkiResim sarkiResmi = new SarkiResim();
            sarkiResmi.SarkiResimYolu = sarkiYolu;

            //sarkiModeli.ImageFile = model.ImageFile;
            //sarkiModeli.HashTagler = model.HashTagler;
            //sarkiModeli.PaylasilanTextler = model.PaylasilanTextler;

            //sarkiModeli.Sarkilar.Baslik = model.Sarkilar.Baslik;

            //sarkiModeli.SarkiResimler.SarkiId = sarkiModeli.Sarkilar.SarkiId;


            // if(ModelState.IsValid)
            // {
            DateTime suAnkiTarih = DateTime.Now;
            model.Sarkilar.TamamlandiMi = false;
            model.Sarkilar.OlusturulmaTarihi = suAnkiTarih;

            using (var context = new DatabaseContext())
            {
                var kullaniciAdi = Session["kullaniciAdi"].ToString();
                var query = context.Uyeler.Where(x => x.KullaniciAdi == kullaniciAdi).ToList();
                foreach (var item in query)
                {
                    model.Sarkilar.UyeId = item.UyeId;
                }
                try
                {
                    context.Sarkilar.Add(model.Sarkilar);
                    context.SaveChanges();
                    sarkiResmi.SarkiId = model.Sarkilar.SarkiId;
                    context.SarkiResimler.Add(sarkiResmi);
                    context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Response.Write(string.Format("Entity türü \"{0}\" şu hatalara sahip \"{1}\" Geçerlilik hataları:", eve.Entry.Entity.GetType().Name, eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Response.Write(string.Format("- Özellik: \"{0}\", Hata: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                        }
                        Response.End();
                    }
                }
            }

            model.HashTagler.SarkiId = model.Sarkilar.SarkiId;

            String[] hashTagDizisi = model.HashTagler.HashTagIcerik.ToString().Split('#');
            using (var context = new DatabaseContext())
            {
                for (int i = 0; i < hashTagDizisi.Length - 1; i++)
                {
                    model.HashTagler.HashTagIcerik = hashTagDizisi[i + 1];
                    context.HashTagler.Add(model.HashTagler);
                    context.SaveChanges();
                }
                model.PaylasilanTextler.UyeId = model.Sarkilar.UyeId;
                model.PaylasilanTextler.SarkiId = model.Sarkilar.SarkiId;
                context.PaylasilanTextler.Add(model.PaylasilanTextler);
                context.SaveChanges();

                //Buraya paylasılan text e alanları giricem gelince halledicem
            }
            // }

            return RedirectToAction("../Main/HomePage");
        }



        [HttpPost]
        public ActionResult ArkadasAra(SarkiEkleViewModel model)
        {
            SarkiEkleViewModel m = new SarkiEkleViewModel();
            using (var db = new DatabaseContext())
            {
                m.UyelerList = db.Uyeler.Where(x => ((x.Ad.StartsWith(model.Uyeler.Ad)) || (x.Soyad.StartsWith(model.Uyeler.Ad))
                 || x.EMail.StartsWith(model.Uyeler.Ad))).ToList();
            }
            TempData["Uyeler"] = m;
            return RedirectToAction("ArkadasGoster", "ArkadasProfil", m);
        }


        public ActionResult Hepsi()
        {
            Session["durum"] = "Hepsi";
            return RedirectToAction("HomePage");
        }


        public ActionResult Tamamlanmis()
        {
            Session["durum"] = "Tamamlanmis";
            return RedirectToAction("HomePage");
        }

        public ActionResult Tamamlanmamis()
        {
            Session["durum"] = "Tamamlanmamis";
            return RedirectToAction("HomePage");
        }

    }
}