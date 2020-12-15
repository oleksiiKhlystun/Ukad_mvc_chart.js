using System;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using test2MVC.Models;
using Newtonsoft.Json;

namespace test2MVC.Controllers
{
    public class TimingsController : Controller
    {
        private TimingContext db = new TimingContext();
      
         public ActionResult Index()
        {
            /* старый вариант
            var sourse = db.Timings.ToList();

            var items = sourse.GroupBy(t => t.Url).Select(g => new QueryTiming
            {
                Url = g.Key,
                MinValue = g.Min(t => t.Time),
                MaxValue = g.Max(t => t.Time)
            })
            .ToList();
            var model = new GroupQueryTiming
            {
                Items = items
            };
            return View(model);
              */

            var sourse = db.Timings.OrderBy(t => t.Time).ToList();
            var itemsMinMax = sourse.GroupBy(t => t.Url).Select(g => new
            {
                url = g.Key,
                minvalue = g.Min(t => t.Time),
                maxvalue = g.Max(t => t.Time)
            });
            var items = sourse.Join(itemsMinMax,
                s => s.Url,
                it => it.url,
                (s, it) => new QueryTiming
                {
                    UrlValue = s.Url,
                    Value = s.Time,
                    MinValue = it.minvalue,
                    MaxValue = it.maxvalue
                }
                ).ToList();
            var model = new GroupQueryTiming
            {
                Items = items
            };
            return View(model);
        }
     
        public ContentResult GetData()
        {

            var sourse = db.Timings.ToList();

            var items = sourse.GroupBy(t => t.Url).Select(g => new QueryTiming
            {
                UrlValue = g.Key,
                MinValue = g.Min(t => t.Time),
                MaxValue = g.Max(t => t.Time)
            })
            .ToList();
 

            return Content(JsonConvert.SerializeObject(items), "application/json");
        }

        [HttpPost]
        public ActionResult Index(Timing timing)
        {
            int timer = Timer(timing.Url);
           
            if (timer ==0)
            {
                return RedirectToAction("Index");
                // ModelState.AddModelError("","Something bed with URL");
            }
           // int timer = Timer(timing.Url);
            else if (ModelState.IsValid && timer != 0) // Дописал timer != 0
            {
                timing.Time = timer;
                timing.Date = DateTime.Now;
                db.Timings.Add(timing);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(timing);
        }
        //[HttpPost]
        //public string Result(string Url)
        //{
        //    int timer = Timer(Url);
        //    string trs = "Site time response:" + timer.ToString();
        //    //ViewBag.Messege = trs;
        //    return trs;
        //}

        [ChildActionOnly] // метод может быть запущен только как дочернее действие
        public string Count()
        {
            //var TimeReqsSort = db.Timings.Where(t => t.Time > 300).Count();// Считаем количество элементов, где время ответа больше 300
           var TimeReqsSort = db.Timings.Sum(t=>t.Time); // Сумма общего времени ответа

            string trs = "Summary time response: "+TimeReqsSort.ToString();
            
            return trs;
        }


        public ActionResult History()
        {
            var TimeReqsSort = db.Timings.OrderBy(t => t.Date); // Сортировка по дате возростанию
            return View(TimeReqsSort.ToList());
        }

        // GET: Timings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Timing timing = db.Timings.Find(id);
            if (timing == null)
            {
                return HttpNotFound();
            }
            return View(timing);
        }

        // GET: Timings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Timings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        public int Timer(string t)
        {
         
            //try
            //{
                if (t != null)
                {
                  
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(t);

                    System.Diagnostics.Stopwatch timer = new Stopwatch();
                    timer.Start();

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    response.Close();

                    timer.Stop();
                    TimeSpan timeTaken = timer.Elapsed;
                   
                    return timeTaken.Milliseconds;

                }
                else
                {
                    return 0;
                }
                       
            // }
            //catch (WebException ex)
            //{
            //    if (ex.Status == WebExceptionStatus.ProtocolError)
            //    {
            //        var response = ex.Response as HttpWebResponse;

            //        if (response != null)
            //        {
            //           // ModelState.AddModelError("", "Something bed with URL");
            //            ViewBag.Result = "HTTP Status Code: " + (int)response.StatusCode;

                       

            //        }
            //        if (response.StatusCode == HttpStatusCode.NotFound)
            //        {
            //           // ModelState.AddModelError("", "Something bed with URL");
            //            //   ViewBag.Scripts = "<script>alert('HTTP Status Code:')</ script > ";
            //            ViewBag.Result = "Not There! Страница отсутвует!";
                        
            //        }
            //    }
            //    else if (ex.Status == WebExceptionStatus.NameResolutionFailure)
            //    {
            //       // ModelState.AddModelError("", "Something bed with URL");
            //        // ViewBag.Scripts = "<script>alert('HTTP Status Code:')</ script > ";
            //        ViewBag.Result = "Bad domain name or Check Internet connection!";
            //    }
            //    return 0;
            //}
        }

        // GET: Timings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Timing timing = db.Timings.Find(id);
            if (timing == null)
            {
                return HttpNotFound();
            }
            return View(timing);
        }

        // POST: Timings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Timing timing = db.Timings.Find(id);
            db.Timings.Remove(timing);
            db.SaveChanges();
            return RedirectToAction("History");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
