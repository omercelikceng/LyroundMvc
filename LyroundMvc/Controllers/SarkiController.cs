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
    public class SarkiController : Controller
    {
        public ActionResult SarkiDetayi()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SarkiyaEklemeIslemiYap(int id, SarkiEkleViewModel model)
        {
            String kullaniciAdiCek;
            if (model.PaylasilanTextler != null)
            {
                if (model.PaylasilanTextler.TextIcerik != null)
                {
                    using (var db = new DatabaseContext())
                    {
                        model.PaylasilanTextler.SarkiId = id;
                        kullaniciAdiCek = Session["kullaniciAdi"].ToString();
                        Uye u = db.Uyeler.Where(x => x.KullaniciAdi == kullaniciAdiCek).FirstOrDefault();
                        model.PaylasilanTextler.UyeId = u.UyeId;
                        db.PaylasilanTextler.Add(model.PaylasilanTextler);
                        db.SaveChanges();

                        if (db.PaylasilanTextler.Where(x => x.SarkiId == id).Count() >= 10)
                        {
                            Sarki sarki = db.Sarkilar.Where(x => x.SarkiId == id).FirstOrDefault();
                            sarki.TamamlandiMi = true;
                            db.Entry(sarki).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }

                    }
                }
            }
            if (model.Yorum != null)
            {
                if (model.Yorum.YorumIcerik != null)
                {
                    using (var db = new DatabaseContext())
                    {
                        DateTime tarih = DateTime.Now;
                        model.Yorum.SarkiId = id;
                        kullaniciAdiCek = Session["kullaniciAdi"].ToString();
                        Uye u = db.Uyeler.Where(x => x.KullaniciAdi == kullaniciAdiCek).FirstOrDefault();
                        model.Yorum.UyeId = u.UyeId;
                        model.Yorum.YorumTarihi = tarih;
                        db.Yorumlar.Add(model.Yorum);
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("SarkiyaEklemeIslemiYap/" + id);
        }



        public ActionResult SarkiyaEklemeIslemiYap(int id)
        {
            using (var db = new DatabaseContext())
            {
                SarkiEkleViewModel model = new SarkiEkleViewModel();
                model.Sarkilar = db.Sarkilar.Where(x => x.SarkiId == id).FirstOrDefault();
                model.HashTagList = db.HashTagler.Where(x => x.SarkiId == id).ToList();
                model.Uyeler = db.Uyeler.Where(x => x.UyeId == model.Sarkilar.UyeId).FirstOrDefault();
                model.SarkiResimler = db.SarkiResimler.Where(x => x.SarkiId == id).FirstOrDefault();
                model.YorumList = db.Yorumlar.Where(x => x.SarkiId == model.Sarkilar.SarkiId).ToList();

                model.PaylasilanTextList = db.PaylasilanTextler.Where(x => x.SarkiId == model.Sarkilar.SarkiId).ToList();
                return View(model);
            }
        }


    }
}