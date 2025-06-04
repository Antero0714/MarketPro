using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketPro.WebAPI.Controllers
{
    public class ProductDetails : Controller
    {
        // GET: ProductDetails
        public ActionResult Index()
        {
            return View();
        }

        // GET: ProductDetails/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductDetails/Create
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

        // GET: ProductDetails/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductDetails/Edit/5
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

        // GET: ProductDetails/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductDetails/Delete/5
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
