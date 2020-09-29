using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace EmployeeMonthlyPay.Models
{
    public class PayslipCalculations
    {
        string filePathUrl = System.Configuration.ConfigurationManager.AppSettings["FilePath"];

        public List<TaxSlabcalculation> CreateTaxslabdetails()
        {
            try
            {
                var csvTableTaxslab = new DataTable();
                using (var csvReaderTax = new CsvReader(new StreamReader(System.IO.File.OpenRead(filePathUrl + @"\TaxDetails.csv")), true))
                {
                    csvTableTaxslab.Load(csvReaderTax);
                }

                List<TaxSlabcalculation> Taxcalculations = new List<TaxSlabcalculation>();

                for (int j = 0; j < csvTableTaxslab.Rows.Count; j++)
                {
                    Taxcalculations.Add(new TaxSlabcalculation
                    {
                        Taxslabmin = csvTableTaxslab.Rows[j][0].ToString(),
                        Taxslabmax = csvTableTaxslab.Rows[j][1].ToString(),
                        Taxformula = csvTableTaxslab.Rows[j][2].ToString(),
                    });
                }

                return Taxcalculations;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Employee> CreateEmployeeDetails()
            {
            try { 
                var csvTableEmployee = new DataTable();
                using (var csvReaderEmployee = new CsvReader(new StreamReader(System.IO.File.OpenRead(filePathUrl + @"\EmployeDetails.csv")), true))
                {
                    csvTableEmployee.Load(csvReaderEmployee);
                }

                string col1 = csvTableEmployee.Columns[0].ToString();
                string row1 = csvTableEmployee.Rows[0][0].ToString();

                List<Employee> Employeedetails = new List<Employee>();

                for (int i = 0; i < csvTableEmployee.Rows.Count; i++)
                {

                    Employeedetails.Add(new Employee { FirstName = csvTableEmployee.Rows[i][0].ToString(), LastName = csvTableEmployee.Rows[i][1].ToString(), AnnualSalary = csvTableEmployee.Rows[i][2].ToString(), SuperRate = csvTableEmployee.Rows[i][3].ToString(), PaymentStartdate = csvTableEmployee.Rows[i][4].ToString() });
                }
                return Employeedetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PayslipDetails> GeneratePayslipData()
        {
            try
            {
                var taxSlabList = CreateTaxslabdetails();
                var employeeDListetail = CreateEmployeeDetails();
                List<PayslipDetails> payslipdata = new List<PayslipDetails>();

                var annualSalary = 0;
                var grossIncome = 0;
                var Super = 0;
                var incomeTax = 0.0;
                var superRateValue = "";
                var netIncome = 0;

                foreach (var emp in employeeDListetail)
                {
                    annualSalary = Convert.ToInt32(emp.AnnualSalary);
                    grossIncome = annualSalary / 12;
                    superRateValue = emp.SuperRate.Trim(new Char[] { ' ', '%', '.' });
                    Super = grossIncome * Convert.ToInt32(superRateValue) / 100;

                    var formula = taxSlabList.Where(m => Convert.ToInt32(m.Taxslabmin) <= annualSalary && (m.Taxslabmax == "Max" || Convert.ToInt32(m.Taxslabmax) >= annualSalary)).Select(m => m.Taxformula).FirstOrDefault();

                    if (formula == "Nil")
                    {
                        incomeTax = 0;
                    }
                    else if (formula.Contains("x"))
                    {

                        formula = formula.Replace("x", annualSalary.ToString());
                        DataTable dt = new DataTable();
                        var taxcalculation = Convert.ToDouble(dt.Compute(formula.ToString(), null));
                        incomeTax = Math.Round(taxcalculation);
                        netIncome = grossIncome - Convert.ToInt32(incomeTax);
                    }

                    var payMonth = getPayPeriod(emp.PaymentStartdate);

                    payslipdata.Add(new PayslipDetails
                    {
                        FirstName = string.Concat(emp.FirstName, emp.LastName),
                        GossIncome = grossIncome.ToString(),
                        IncomeTax = incomeTax.ToString(),
                        NetIncome = netIncome.ToString(),
                        SuperIncome = Super.ToString(),
                        PayPeriod = payMonth
                    });


                }
                return payslipdata;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string getPayPeriod(string paydate)
        {
            try
            {
                DateTime dateTimeOb = DateTime.ParseExact(paydate, "ddMMyyyy", CultureInfo.InvariantCulture);
                DateTime lastdate = LastDayOfMonth(dateTimeOb);
                string salryMonth = Convert.ToString(dateTimeOb.ToShortDateString()) + "-" + Convert.ToString(lastdate.Date.ToShortDateString());
                return salryMonth;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DateTime LastDayOfMonth(DateTime date)
        {
            try
            {
                DateTime ss = new DateTime(date.Year, date.Month, 1);
                return ss.AddMonths(1).AddDays(-1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void createCSVfile()
        {
            try
            {
                string strFilePath = filePathUrl + @"\OutputPayslip.csv";
                string strSeperator = ",";
                StringBuilder sbOutput = new StringBuilder();

                var Payslipdata = GeneratePayslipData();

                for (int i = 0; i < Payslipdata.Count(); i++)
                {
                    sbOutput.AppendLine(string.Join(strSeperator, Payslipdata[i].FirstName, Payslipdata[i].PayPeriod, Payslipdata[i].GossIncome, Payslipdata[i].IncomeTax, Payslipdata[i].NetIncome, Payslipdata[i].SuperIncome));
                }

                File.WriteAllText(strFilePath, sbOutput.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}