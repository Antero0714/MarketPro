using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketPro.WebAPI.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class Dashboard1Controller : Controller
    {
        // GET: Dashboard1Controller
        public ActionResult Index()
        {
            return View();
        }

        // GET: Dashboard1Controller/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Dashboard1Controller/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dashboard1Controller/Create
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

        // GET: Dashboard1Controller/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Dashboard1Controller/Edit/5
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

        // GET: Dashboard1Controller/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Dashboard1Controller/Delete/5
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
