using Application.Features.Enums.Queries;
using Application.Features.Jobs.JobApplications.Commands;
using Application.Features.Jobs.JobApplications.Queries;
using Application.Features.Jobs.JobApplications.Search;
using Application.Features.Jobs.JobForms.Commands;
using Application.Features.Jobs.JobForms.Queries;
using Application.Features.Jobs.JobForms.Search;
using Application.Features.Jobs.JobsDetails.Commands;
using Application.Features.Jobs.JobsDetails.Queries;
using DTO.Enums.Job.JobApplication;
using DTO.Job.JobApplication;
using DTO.Job.JobDetails;
using DTO.Pagination;
using DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1
{
    public class JobController : ApiControllerBase
    {

        #region Job Details

        [HttpPost("details")]
        public async Task<IActionResult> CreateJobDetails([FromBody] JobDetailsCreateCommand request)
        {
            await Mediator.Send(request);

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("details/from/{userId}")]
        public async Task<IReadOnlyCollection<JobDetailsBaseResponse>> GetAllJobDetailsFromUser([FromRoute] int userId)
        {
            var response = await Mediator.Send(new JobDetailsGetByCreatorIdQuery(userId));

            return response;
        }

        [AllowAnonymous]
        [HttpGet("details/{hash}")]
        public async Task<JobDetailsBaseResponse> GetJobDetailsByHash([FromRoute] string hash)
        {
            var response = await Mediator.Send(new JobDetailsGetByHashQuery(hash));

            return response;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("details/search/rebuild")]
        public async Task<IActionResult> RebuildJobDetailsSearchIndex()
        {
            await Mediator.Send(new JobDetailsInitiateSearchIndexRebuildCommand());
            return Ok();
        }


        #endregion

        #region Job Form

        [HttpPost("form")]
        public async Task<IActionResult> CreateJobForm([FromBody] JobFormCreateCommand request)
        {
            await Mediator.Send(request);

            return Ok();
        }

        [HttpPut("form")]
        public async Task<IActionResult> UpdateJobForm([FromBody] JobFormUpdateCommand request)
        {
            await Mediator.Send(request);

            return Ok();
        }

        [HttpPost("form/search")]
        public async Task<PaginatedList<JobFormSearchable>> JobFormFullSearch([FromBody] JobFormFullSearchQuery request)
        {
            return await Mediator.Send(request);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("form/search/rebuild")]
        public async Task<IActionResult> RebuildJobFormSearchIndex()
        {
            await Mediator.Send(new JobFormInitiateSearchIndexRebuildCommand());
            return Ok();
        }

        #endregion

        #region Job Application

        [AllowAnonymous]
        [HttpPost("application")]
        public async Task<IActionResult> CreateJobApplication([FromForm] JobApplicationCreateCommand request)
        {
            await Mediator.Send(request);

            return Ok();
        }

        [HttpGet("application/{id}")]
        public async Task<JobApplicationResponse> GetJobApplication([FromRoute] int id)
        {
            var response = await Mediator.Send(new JobApplicationGetQuery(id));

            return response;
        }


        [HttpPut("application/status")]
        public async Task<IActionResult> UpdateJobApplicationStatus([FromForm] JobApplicationStatusChangeCommand request)
        {
            await Mediator.Send(request);
            
            return Ok();
        }

        [HttpPost("application/search")]
        public async Task<PaginatedList<JobApplicationSearchable>> FullSearch([FromBody] JobApplicationFullSearchQuery request)
        {
            return await Mediator.Send(request);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("application/search/rebuild")]
        public async Task<IActionResult> RebuildJobApplicationSearchIndex()
        {
            await Mediator.Send(new JobApplicationInitiateSearchIndexRebuildCommand());
            return Ok();
        }

        [HttpGet("application/dashboard/statistics")]
        public async Task<JobApplicationDashboardStatisticsResponse> GetJobApplicationDashboardStatistics()
        {
            return await Mediator.Send(new JobApplicationDashboardStatisticsQuery());
        }

        [HttpGet("application/hired/statistics")]
        public async Task<JobApplicationHiredStatisticsResponse> GetJobApplicationHiredStatistics()
        {
            return await Mediator.Send(new JobApplicationHiredStatisticsQuery());
        }

        [HttpGet("application/status")]
        public async Task<IReadOnlyCollection<ListItemBaseResponse>> GetJobApplicationStatuses()
        {
            return await Mediator.Send(new GetEnumValuesQuery(typeof(JobApplicationStatus)));
        }

        #endregion

    }
}
