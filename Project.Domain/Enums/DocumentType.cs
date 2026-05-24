using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Enums
{
    public enum DocumentType
    {
        BonafideCertificate = 1,
        LeavingCertificate = 2,
        OfferLetter = 3,
        DiscontinueLetter = 4,
        SalaryIncrementLetter = 5,
        FeeReceipt = 6,
        SalarySlip = 7,

        // Student upload documents
        AadharCard = 8,
        RationCard = 9,
        PreviousSchoolLeavingCertificate = 10,

        // Generated reports
        StudentProfileReport = 11
    }
}
