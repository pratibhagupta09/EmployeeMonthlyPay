using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeMonthlyPay.Models
{
    public class PayslipDetails
    {
        public string FirstName { get; set; }
        public string PayPeriod { get; set; }
        public string GossIncome { get; set; }
        public string IncomeTax { get; set; }
        public string NetIncome { get; set; }
        public string SuperIncome { get; set; }
    }
}