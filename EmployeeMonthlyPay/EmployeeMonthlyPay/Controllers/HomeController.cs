using EmployeeMonthlyPay.Models;
using System;
using System.Web.Mvc;



namespace EmployeeMonthlyPay.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
                        
            return View();
        }

        public ActionResult GetPayslipdetails()
        {
            try
            {
                PayslipCalculations objPayslip = new PayslipCalculations();
                objPayslip.createCSVfile();
                ViewBag.Message = "Payslip Generated, Please check file";
                return View("Index");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}