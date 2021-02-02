using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sample.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Http;

namespace Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly CommonContext dbContext;


        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, CommonContext dbcontext)
        {
            _logger = logger;
            this.dbContext = dbcontext;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetContactDetails")]
        public List<Contacts> GetContactDetails()
        {

            return dbContext.Contacts.AsNoTracking().Select(x => x).ToList();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetContactgroups")]
        public List<ContactGroups> GetContactgroups()
        {

            return dbContext.ContactGroups.AsNoTracking().Select(x => x).ToList();

        }

        /// <summary>
        ///  records will be created in both tables using primary n foreign key relation
        /// </summary>
        /// <param name="callDetail"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostContactGroupDetail")]
        public async Task<ActionResult> PostContactGroupDetail(Contacts callDetail)
        {

            string text = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    var isexist = dbContext.Contacts.Where(x => x.FirstName.ToLower() == callDetail.FirstName.ToLower() && x.LastName.ToLower() == callDetail.LastName.ToLower()).FirstOrDefault();
                    if (isexist == null)
                    {
                        var group = Displaygroup(callDetail.MobileNumber);
                        callDetail.ContactGroups.Add(new ContactGroups()
                        {

                            ContactMapId = callDetail.ContactId,
                            GroupName = group.Trim()
                        });

                        dbContext.Contacts.Add(callDetail);
                        await dbContext.SaveChangesAsync();
                        return Ok("Inserted successfully!");
                    }
                    else
                    {
                        return Ok("Record already Exist with same User");
                    }
                }
                else
                {
                    return BadRequest("Failed to insert!");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateContacts")]
        public IActionResult UpdateContacts(Contacts model)
        {
            if (ModelState.IsValid)
            {
                var entity= dbContext.Contacts.AsNoTracking().Where(x => x.ContactId==model.ContactId).FirstOrDefault();
                if (entity != null)
                {
                    entity.FirstName = model.FirstName;
                    entity.LastName = model.LastName;
                    entity.MobileNumber = model.MobileNumber;
                    dbContext.SaveChanges();
                }
                

                return Ok("Contacts Updated ");
            }
            else
            {
                return Ok("Invalid Data");
            }
            
        }

        /// <summary>
        /// Implemented Cascading Delete- primary and foreign key deletion
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteContacts(id={id})")]
        public IActionResult DeleteContacts(int id)
        {
            if (id <= 0)
                return BadRequest("Not a valid id");
            try
            {
                var post = dbContext.Contacts.AsNoTracking().FirstOrDefault(x => x.ContactId == id);

                if (post != null)
                {
                    dbContext.Contacts.Remove(post);
                    dbContext.SaveChanges();
                }

            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }


            return Ok("Records Deleted Successfully!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SearchContact")]
        public async Task<IEnumerable<Contacts>> SearchContact(PagingParameterModel Pagingparametermodel)
        {
            var result = new List<Contacts>();
            try
            {
                var source = from s in dbContext.Contacts.AsNoTracking()
                               select s;

                    if (!string.IsNullOrEmpty(Pagingparametermodel.QuerySearch))
                    {
                        source = source.Where(a => a.FirstName.ToLower().Trim().Contains(Pagingparametermodel.QuerySearch.ToLower().Trim())
                                        || a.LastName.Contains(Pagingparametermodel.QuerySearch));
                    }
                    int count = source.Count();
                    int CurrentPage = Pagingparametermodel.pageNumber;
                    int PageSize = Pagingparametermodel.pageSize;

                    int TotalCount = count;

                    // Calculating Totalpage by Dividing (No of Records / Pagesize)  
                    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

                    // Returns List of Customer after applying Paging   
                    result = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                    return await Task.FromResult(result);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //return await Task.FromResult(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SearchContactGroup(searchString={searchString})")]
        public async Task<List<ContactGroups>> SearchContactGroup(string searchString)
        {
            var result = new List<ContactGroups>();

            try
            {
                var students = from s in dbContext.ContactGroups.AsNoTracking()
                               select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    result = students.Where(s => s.GroupName.ToLower().Trim().Contains(searchString.ToLower().Trim()))
                             .Select(x => new ContactGroups
                             {
                                GroupName = x.GroupName
   
                              }).ToList();
                    return await Task.FromResult(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return await Task.FromResult(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phome"></param>
        /// <returns></returns>
        public string Displaygroup(string phome)
            {

            if (phome.Contains("8050"))
            {

                return "Jio";
            }
            else if (phome.Contains("9216"))
            {
                return "Airtel";
            }
            else if (phome.Contains("8020"))
            {
                return "Vodafone";
            }

            else
            {
                return "Not in use";
            }
                

         }

        
       

    }
}
