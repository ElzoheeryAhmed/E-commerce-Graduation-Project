using GraduationProject.Data;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueController : ControllerBase
    {
        private AppDbContext _context;
        public IssueController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet(template:"GetAll")]

        public async Task<IActionResult> GetAllAsync()
        {
            var issues = await _context.Issues.ToListAsync();
            return Ok(issues);  
        }

        [HttpGet(template: "GetUnrespondedIssues")]

        public async Task<IActionResult> GetUnrespondedIssuesAsync()
        {
            var issues = await _context.Issues.Where(i => i.Status == IssueStatus.Submitted)
                .Select(i=>new UnresponedIssueDto
                {
                    Id=i.Id,
                    Description=i.Description,
                    SubmitDate=i.SubmitDate,
                })
                .ToListAsync();
            


            return Ok(issues);
        }

        [HttpGet(template: "GetrespondedIssues")]

        public async Task<IActionResult> GetrespondedIssuesAsync()
        {
            var issues = await _context.Issues.Where(i => i.Status == IssueStatus.Responded)
                .Select(i => new responedIssueDto
                {
                    Id = i.Id,
                    Description = i.Description,
                    SubmitDate = i.SubmitDate,
                    Response=i.Response,
                    RespondDate=i.RespondDate,
                    AdminId=i.AdminId
                })
                .ToListAsync();



            return Ok(issues);
        }

        [HttpGet(template: "GetresolvedIssues")]

        public async Task<IActionResult> GetresolvedIssuesAsync()
        {
            var issues = await _context.Issues.Where(i => i.Status == IssueStatus.Resolved)
                .Select(i => new responedIssueDto
                {
                    Id = i.Id,
                    Description = i.Description,
                    SubmitDate = i.SubmitDate,
                    Response = i.Response,
                    RespondDate = i.RespondDate,
                    AdminId = i.AdminId
                })
                .ToListAsync();



            return Ok(issues);
        }


        [HttpPost(template:"AddIssue")]

        public async Task<IActionResult> AddIssueAsync([FromBody]string description)
        {
            var issue = new Issue { Description=description };

            //add to database 
            await _context.Issues.AddAsync(issue);
            _context.SaveChanges();


            return Ok(issue);

        }


        [HttpPut(template: "Respond")]

        public async Task<IActionResult> RespondAsync([FromBody] responseIssueDto dto)
        {
            var issue = await _context.Issues.FindAsync(dto.IssueId);

            //validation of issue 
            if(issue is null)
            {
                return NotFound();
            }

            issue.Response = dto.Response;
            issue.AdminId = dto.AdminId;
            issue.RespondDate = DateTime.Now;
            issue.Status = IssueStatus.Responded;

            //update  database 
            _context.SaveChanges();


            return Ok(issue);

        }
        
        [HttpPut(template: "Resolve/{IssuId}")]
        public async Task<IActionResult> ResolveAsync(int IssueId)
        {
            var issue = await _context.Issues.FindAsync(IssueId);

            //validation of issue 
            if (issue is null)
            {
                return NotFound();
            }

            issue.Status = IssueStatus.Resolved;

            //update  database 
            _context.SaveChanges();


            return Ok(issue);

        }

    }
}
