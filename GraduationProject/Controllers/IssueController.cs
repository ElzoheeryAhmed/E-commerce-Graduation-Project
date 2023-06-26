using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using GraduationProject.Data;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ILogger<UserController> _logger;

        public IssueController(AppDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get issues in the system
        /// </summary>
        /// <param name="status">issue status</param>
        /// <returns></returns>
        /// <response code="200">Returns all issues in the system</response>
        /// <response code="401">Unauthorized response, user is not authenticated</response>
        /// <response code="403">Forbidden response, user is not authorized to access this endpoint</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [Authorize(Roles = "Admin")]
        [HttpGet(template: "GetIssues")]
        public async Task<IActionResult> GetIssuesAsync(IssueStatus status,[Required]bool all=true)
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var issues = await _context.Issues.Where(i => (i.Status == status) || all)
                    .Select(i => new
                    {
                        Id = i.Id,
                        Description = i.Description,
                        Status = i.Status.ToString(),
                        SubmitDate = i.SubmitDate,
                        Response = i.Response,
                        RespondDate = i.RespondDate,
                        AdminId = i.AdminId
                    })
                    .ToListAsync();



                return Ok(issues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs during retrieving issues in the system with AdminId:{adminId}.");
                return StatusCode(500, "Internal Server Error.");
            }
        }


        /// <summary>
        /// Add issue into the system
        /// </summary>
        /// <param name="description">issue description</param>
        /// <returns></returns>
        /// <response code="200">Returns all issues in the system</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [HttpPost(template:"AddIssue")]
        public async Task<IActionResult> AddIssueAsync([FromBody]string description)
        {
            try
            {
                var issue = new Issue { Description = description };

                //add to database 
                await _context.Issues.AddAsync(issue);
                _context.SaveChanges();


                return Ok(issue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs during adding new  issue in the system");
                return StatusCode(500, "Internal Server Error.");
            }
        }
        /// <summary>
        /// Respond to issue 
        /// </summary>
        /// <param name="dto">Response information</param>
        /// <returns></returns>
        /// <response code="200">Issue is responded successfully</response>
        /// <response code="400">BadRequest, Invalid input data</response>
        /// <response code="404">Issue is not existed</response>
        /// <response code="401">Unauthorized response, user is not authenticated</response>
        /// <response code="403">Forbidden response, user is not authorized to access this endpoint</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [Authorize(Roles = "Admin")]
        [HttpPut(template: "Respond")]
        public async Task<IActionResult> RespondAsync([FromBody] responseIssueDto dto)
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var issue = await _context.Issues.FindAsync(dto.IssueId);

                //validation of issue 
                if (issue is null)
                {
                    return NotFound();
                }

                issue.Response = dto.Response;
                issue.AdminId = adminId;
                issue.RespondDate = DateTime.Now;
                issue.Status = IssueStatus.Responded;

                //update  database 
                _context.SaveChanges();


                return Ok(issue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs responding to issue in the system with AdminId:{adminId}");
                return StatusCode(500, "Internal Server Error.");
            }
        }

        /// <summary>
        /// Resolve an issue 
        /// </summary>
        /// <param name="IssueId">Issue id</param>
        /// <returns></returns>
        /// <response code="200">Issue is resolved</response>
        /// <response code="400">BadRequest, Invalid input data</response>
        /// <response code="404">Issue is not existed</response>
        /// <response code="401">Unauthorized response, user is not authenticated</response>
        /// <response code="403">Forbidden response, user is not authorized to access this endpoint</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [Authorize(Roles = "Admin")]
        [HttpPut(template: "Resolve/{IssueId}")]
        public async Task<IActionResult> ResolveAsync(int IssueId)
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs resolving issue in the system with AdminId:{adminId}");
                return StatusCode(500, "Internal Server Error.");
            }
        }

    }
}
