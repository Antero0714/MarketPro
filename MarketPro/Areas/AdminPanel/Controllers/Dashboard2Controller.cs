using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketPro.WebAPI.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class Dashboard2Controller : Controller
    {
        // GET: Dashboard2Controller
        public ActionResult Index()
        {
            return View();
        }

        // GET: Dashboard2Controller/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Dashboard2Controller/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dashboard2Controller/Create
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

        // GET: Dashboard2Controller/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Dashboard2Controller/Edit/5
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

        // GET: Dashboard2Controller/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Dashboard2Controller/Delete/5
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
