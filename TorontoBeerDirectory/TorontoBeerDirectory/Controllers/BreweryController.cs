﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using TorontoBeerDirectory.Models;
using System.Web.Script.Serialization;
using TorontoBeerDirectory.Migrations;

namespace TorontoBeerDirectory.Controllers
{
    public class BreweryController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static BreweryController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44393/api/brewerydata/");
        }

        /// <summary>
        /// Gets a list of breweries from Brewery API and renders them in the view.
        /// </summary>
        /// <returns>
        /// list of breweries
        /// </returns>
        /// <example>
        /// GET: Brewery/List
        /// </example>

        // GET: Brewery/List
        public ActionResult List()
        {
            //goal is to communicate with brewery api to retrieve a list of breweries
            //curl https://localhost:44393/api/brewerydata/listbreweries

            string url = "listbreweries";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<Brewery> breweries = response.Content.ReadAsAsync<IEnumerable<Brewery>>().Result;
            //Debug.WriteLine("Number of breweries recieved: ");
            //Debug.WriteLine(breweries.Count());


            return View(breweries);
        }

        /// <summary>
        /// Gets the details of a specific brewery from the Brewery API by its ID and renders it in the view
        /// </summary>
        /// <param name="id">id used to identify specific brewery in database</param>
        /// <returns>
        /// View of specific brewery
        /// </returns>
        /// <example>
        /// GET: Brewery/Details/1
        /// </example>

        // GET: Brewery/Details/1
        public ActionResult Details(int id)
        {
            //goal is to communicate with brewery api to retrieve one brewery
            //curl https://localhost:44393/api/brewerydata/findbrewery/

            string url = "findbrewery/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            Brewery selectedbrewery = response.Content.ReadAsAsync<Brewery>().Result;
            //Debug.WriteLine("Number of breweries recieved: ");


            return View(selectedbrewery);
        }

        /// <summary>
        /// Displays a view if there is an error
        /// </summary>
        /// <returns>
        /// View of an error page if there is an error
        /// </returns>
        /// <example>
        /// GET: Brewery/Error
        /// </example>

        public ActionResult Error()
        {

            return View();
        }

        /// <summary>
        /// Displays a form for creating a new brewery.
        /// </summary>
        /// <returns>
        /// view for creating a new brewery
        /// </returns>
        /// <example>
        /// GET: Brewery/New
        /// </example>

        // GET: Brewery/New
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// Creates a new brewery in the database using data from the form
        /// </summary>
        /// <param name="brewery">Brewery object which has the details of the new brewery thats to be added</param>
        /// <returns>
        /// Redirects to the brewery list if the creation is successful, else it returns to an error view
        /// </returns>
        /// <example>
        /// POST: Brewery/Create
        /// </example>

        // POST: Brewery/Create
        [HttpPost]
        public ActionResult Create(Brewery brewery)
        {
            Debug.WriteLine("the json payload is: ");
            Debug.WriteLine(brewery.BreweryName);

            //goal: add a new brewery to our system using the API
            //curl -H "Content-Type:application/json" -d @brewery.json https://localhost:44393/api/brewerydata/addbrewery
            string url = "addbrewery";

            
            string jsonpayload = jss.Serialize(brewery);

            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("list");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// displays view with form to edit a specific brewery
        /// </summary>
        /// <param name="id">id for specific brewery thats to be edited</param>
        /// <returns>
        /// View with form to edit specific brewery details
        /// </returns>
        /// <example>
        /// GET: Brewery/Edit/1
        /// </example>

        // GET: Brewery/Edit/1
        public ActionResult Edit(int id)
        {
            string url = "findbrewery/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            Brewery selectedbrewery = response.Content.ReadAsAsync<Brewery>().Result;
            return View(selectedbrewery);
        }

        /// <summary>
        /// Updates details of a specific brewery in the database based on the id
        /// </summary>
        /// <param name="id">id of the brewery thats being updated</param>
        /// <param name="brewery">brewery object with the updated details of the brewery</param>
        /// <returns>
        /// on succeddful update it redirects to brewery list, else redirects to error page
        /// </returns>
        /// <example>
        /// POST: Brewery/Update/1
        /// </example>

        // POST: Brewery/Update/1
        [HttpPost]
        public ActionResult Update(int id, Brewery brewery)
        {
            string url = "updatebrewery/" + id;
            string jsonpayload = jss.Serialize(brewery);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Displays view for confirmation on deleting a specific brewery
        /// </summary>
        /// <param name="id">id for specific brewery</param>
        /// <returns>
        /// view for confirming deletion of specified brewery
        /// </returns>
        /// <example>
        /// GET: Brewery/Delete/5
        /// </example>

        // GET: Brewery/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "findbrewery/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            Brewery selectedbrewery = response.Content.ReadAsAsync<Brewery>().Result;
            return View(selectedbrewery);
        }

        /// <summary>
        /// deletes a specified brewery from the database using its id
        /// </summary>
        /// <param name="id">id for specific brewery</param>
        /// <returns>
        /// redirects to brewery list if the deletion was successful, else redirects to error page
        /// </returns>
        /// <example>
        /// POST: Brewery/Delete/5
        /// </example>

        // POST: Brewery/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "deletebrewery/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
