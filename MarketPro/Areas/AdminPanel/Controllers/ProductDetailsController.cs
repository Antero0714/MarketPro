using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketPro.WebAPI.Areas.AdminPanel.Controllers
{
    public class ProductDetailsController : AdminBaseController
    {
        // GET: ProductDetailsController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ProductDetailsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductDetailsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductDetailsController/Create
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

        // GET: ProductDetailsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductDetailsController/Edit/5
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

        // GET: ProductDetailsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductDetailsController/Delete/5
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
