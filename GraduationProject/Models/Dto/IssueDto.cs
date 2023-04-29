namespace GraduationProject.Models.Dto
{

    
    public class UnresponedIssueDto
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public DateTime SubmitDate { get; set; }
    }

    public class responedIssueDto: UnresponedIssueDto
    {
        public string Response { get; set; }

        public DateTime? RespondDate { get; set; }
        

        public string AdminId { get; set; }
    }

    public class responseIssueDto 
    {
        public int IssueId { get; set; }

        public string Response { get; set; }

        public string AdminId { get; set; }
    }

}
