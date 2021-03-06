﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using testDMS.DAL;
using testDMS.Models;
using System.Linq;
using Microsoft.Ajax.Utilities;
using PagedList;

namespace testDMS.Controllers
{
    public class ChartController : Controller
    {
        private DonorManagementDatabaseEntities ddlData = new DonorManagementDatabaseEntities();
        IDonorRepository drRepo;
        IDonationRepository dnRepo;
        public Dictionary<int, int[]> amountList = new Dictionary<int, int[]>()
        {
            { 1, new []{1, 100} },
            { 2, new []{101, 500} },
            { 3, new []{501,  1000} },
            { 4, new []{1000, 2000 }},
            { 5, new []{2001, 4000} },
            { 6, new []{4001, 7000} },
            { 7, new []{7001, 10000} },
            { 8, new []{10001, 1000000} }
        };


        public ChartController(IDonorRepository drRepo, IDonationRepository dnRepo)
        {
            this.drRepo = drRepo;
            this.dnRepo = dnRepo;
        }

        public ActionResult Index(int? page)
        {
            LoadData(page);
            LoadSelectList();
            return View();
        }

        public ActionResult LoadSelectList()
        {
            ViewBag.Person = new SelectList(ddlData.DONOR, "DonorId", "FNAME");

            ViewBag.Department = new SelectList(ddlData.DEPARTMENTS, "Department", "Department");

            ViewBag.Gl = new SelectList(ddlData.GLS, "GL", "GL");

            var amountList = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem {Text = "Amount", Value="0", Selected=true },
                    new SelectListItem {Text = "0-100", Value="1" },
                    new SelectListItem {Text = "101-500", Value="2" },
                    new SelectListItem {Text = "501-1000", Value="3" },
                    new SelectListItem {Text = "1001-2000", Value="4" },
                    new SelectListItem {Text = "2001-4000", Value="5" },
                    new SelectListItem {Text = "4001-7000", Value="6" },
                    new SelectListItem {Text = "7001-10,000", Value="7" },
                    new SelectListItem {Text = "10,000+", Value="8" },
                }, "Value", "Text", 0);

            ViewBag.Amount = amountList;

            return View("~/Views/Chart/Index.cshtml");
        }

        public ActionResult LoadData(int? page)
        {

            int count = 0;
            int pageSize = 10;
            int pageNum = (page ?? 1);

            var donations = from DONATION d in dnRepo.GetDonations()
                            select d;

            var donors = from DONOR d in drRepo.GetDonors
                         select d;

            ChartDispalyViewModel model = new ChartDispalyViewModel();

            count = donations.Count();

            model.Donors = donors.Take(count).ToPagedList(pageNum, pageSize);
            model.Donations = donations.Take(count).ToPagedList(pageNum, pageSize);

            return View("~/Views/Chart/Index.cshtml", model);
        }

        public ActionResult Search(string searchString, int amount, DateTime? date1, DateTime? date2, string department, string gl, int? page)
        {
            int count = 0;
            int pageSize = 10;
            int pageNum = (page ?? 1);

            int amount1 = 0;
            int amount2 = 0;


            var tempArray = new int[2];

            tempArray = amountList[amount];

            amount1 = tempArray.ElementAt(0);

            amount2 = tempArray.ElementAt(1);


            IEnumerable<DONATION> Donations = new List<DONATION>();

            if (searchString != null)
            {
                page = 1;
                Donations = dnRepo.FindBy(searchString, amount1, amount2, date1, date2, department, gl);
            }

            count = Donations.Count();

            ChartDispalyViewModel model = new ChartDispalyViewModel();

            model.Donations = Donations.Take(count).ToPagedList(pageNum, pageSize);

            model.searchString = searchString;
            model.amount = amount;
            model.date1 = date1;
            model.date2 = date2;
            model.department = department;
            model.gl = gl;

            LoadSelectList();


            return View("~/Views/Chart/Index.cshtml", model);
        }


        public ActionResult ExportToExcel(string searchString, int? amount, DateTime? date1, DateTime? date2, string department, string gl)
        {
            int amount1 = 0;
            int amount2 = 0;

            /* switch (amount)
             {
                 case 0:
                     break;
                 case 1:
                     amount1 = 1;
                     amount2 = 100;
                     break;
                 case 2:
                     amount1 = 101;
                     amount2 = 500;
                     break;
                 case 3:
                     amount1 = 501;
                     amount2 = 1000;
                     break;
                 case 4:
                     amount1 = 1001;
                     amount2 = 2000;
                     break;
                 case 5:
                     amount1 = 2001;
                     amount2 = 4000;
                     break;
                 case 6:
                     amount1 = 4001;
                     amount2 = 7000;
                     break;
                 case 7:
                     amount1 = 7001;
                     amount2 = 10000;
                     break;
                 case 8:
                     amount1 = 10001;
                     amount2 = 1000000;
                     break;
                 default:
                     break;
             };*/




            var tempArray = new int[2];

            tempArray = amountList[amount.GetValueOrDefault()];

            amount1 = tempArray.ElementAt(0);

            amount2 = tempArray.ElementAt(1);

            var Donations = (IEnumerable<DONATION>)dnRepo.FindBy(searchString,
                amount1, amount2, date1, date2, department, gl);

            BindingList<CHARTDATA> bList = new BindingList<CHARTDATA>();

            foreach (var item in Donations)
            {
                var cData = new CHARTDATA
                {
                    DonorName = item.DONOR.FName,
                    Amount = item.Amount,
                    DateReceived = item.DateRecieved,
                    Department = item.Department,
                    GL = item.GL
                };
                bList.Add(cData);
            }

            DateTime dt = DateTime.Now;
            string date = dt.ToShortDateString();

            // Step 1 - get the data from database
            //var myData = ddlData.DONATION.ToList();

            // instantiate the GridView control from System.Web.UI.WebControls namespace
            // set the data source
            GridView gridview = new GridView();
            gridview.DataSource = bList;
            gridview.DataBind();

            // Clear all the content from the current response
            Response.ClearContent();
            Response.Buffer = true;

            // set the header
            Response.AddHeader("content-disposition", "attachment; filename = Report-" + date + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";

            // create HtmlTextWriter object with StringWriter
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    // render the GridView to the HtmlTextWriter
                    gridview.RenderControl(htw);
                    // Output the GridView content saved into StringWriter
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
            return View();
        }
    }
}