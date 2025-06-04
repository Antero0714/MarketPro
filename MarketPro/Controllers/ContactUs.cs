using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketPro.WebAPI.Controllers
{
    public class ContactUs : Controller
    {
        // GET: ContactUs
        public ActionResult Index()
        {
            return View();
        }

        // GET: ContactUs/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ContactUs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ContactUs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ContactUs/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ContactUs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ContactUs/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ContactUs/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
