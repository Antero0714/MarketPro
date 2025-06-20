﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketPro.WebAPI.Areas.AdminPanel.Controllers
{
    public class SocialMediaAnalyticsController : AdminBaseController
    {
        // GET: SocialMediaAnalyticsController
        public ActionResult Index()
        {
            return View();
        }

        // GET: SocialMediaAnalyticsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SocialMediaAnalyticsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SocialMediaAnalyticsController/Create
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

        // GET: SocialMediaAnalyticsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SocialMediaAnalyticsController/Edit/5
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

        // GET: SocialMediaAnalyticsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SocialMediaAnalyticsController/Delete/5
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
