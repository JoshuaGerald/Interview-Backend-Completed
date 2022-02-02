using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moduit.Interview.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Moduit.Interview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionTwoController : ControllerBase
    {
        private IConfiguration config;

        public QuestionTwoController(IConfiguration iConfig)
        {
            config = iConfig;
        }

        private List<QuestionTwoDto> getQuestion()
        {
            List<QuestionTwoDto> result = new List<QuestionTwoDto>();

            try
            {
                using (var client = new HttpClient())
                {
                    string url = config.GetSection("URLAPI").Value;
                    var responseTask = client.GetAsync(url + "/backend/question/two");
                    responseTask.Wait();
                    var response = responseTask.Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = response.Content.ReadAsStringAsync().Result;
                        result = JsonConvert.DeserializeObject<List<QuestionTwoDto>>(readTask);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<QuestionTwoDto> question = getQuestion();

                // Description that contains "Ergonomics" or Title that contains "Ergonomics"
                //List<QuestionTwoDto> resp = question.Where(p => p.description.Contains("Ergonomics") || p.title.Contains("Ergonomics")).ToList();
                // NOTE: There's no description & title that contains "Ergonomics". But there are some that contains "Ergonomic" with no 's'.
                List<QuestionTwoDto> resp = question.Where(p => p.description.Contains("Ergonomic") || p.title.Contains("Ergonomic")).ToList();

                // Tags that contains "Sports"
                resp = resp.Where(p => p.tags != null).ToList();
                resp = resp.Where(p => p.tags.Contains("Sports")).ToList();

                // Order by Id descending
                resp = resp.OrderByDescending(p => p.id).ToList();

                // Get the last 3 (three) items only
                resp = resp.Skip(Math.Max(0, resp.Count() - 3)).ToList();

                return Ok(resp);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
