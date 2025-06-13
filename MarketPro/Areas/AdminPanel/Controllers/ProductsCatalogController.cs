using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketPro.WebAPI.Areas.AdminPanel.Controllers
{
    public class ProductsCatalogController : AdminBaseController
    {
        // GET: ProductsCatalogController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ProductsCatalogController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductsCatalogController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductsCatalogController/Create
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

        // GET: ProductsCatalogController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductsCatalogController/Edit/5
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

        // GET: ProductsCatalogController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductsCatalogController/Delete/5
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
