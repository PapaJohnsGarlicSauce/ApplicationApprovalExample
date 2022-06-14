using ApplicationApproval.Models;
using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationApprovalTest.Customizations
{
    internal class ApplicationCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            // Make loan payment default to 0 so I can set it appropriately when I want to deny/approve
            fixture.Customize<Application>(a =>
                a.With(x => x.Loan.MonthlyPayment, 0));
        }
    }
}
