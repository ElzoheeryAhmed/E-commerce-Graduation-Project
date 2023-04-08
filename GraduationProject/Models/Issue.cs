namespace GraduationProject.Models
{

    public enum IssueStatus : byte
    {
        Submitted, InProgress, Rejected, Completed
    }
    public class Issue
    {

        public int Id { get; set; }
        public string Description { get; set; }

        public DateTime SubmitDate { get; set; }
        public DateTime? RespondDate { get; set; }
        public IssueStatus Status { get; set; }

        public string AdminId { get; set; }



        public User RespondAdmin { get; set; }
    }
}
