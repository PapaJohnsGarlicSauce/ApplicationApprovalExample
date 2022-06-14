namespace ApplicationApproval.Models
{
    public class Application
    {
        public static string ApplicationRegex { get; } = @"Application (\S+)";

        public string Id { get; set; }

        public Loan Loan { get; set; }

        public Borrower Borrower { get; set; }

        public Borrower? Coborrower { get; set; }

        public ApplicationStatus? Status { get; set; }

        public Application(string id, Loan loan, Borrower borrower, Borrower? coBorrower)
        {
            Id = id;
            Loan = loan;
            Borrower = borrower;
            Coborrower = coBorrower;
        }
    }
}
