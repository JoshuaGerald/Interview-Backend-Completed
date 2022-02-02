using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moduit.Interview.Models;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Moduit.Interview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionOneController : ControllerBase
    {
        private IConfiguration config;

        public QuestionOneController(IConfiguration iConfig)
        {
            config = iConfig;
        }

        private QuestionOneDto getQuestion()
        {
            QuestionOneDto result = new QuestionOneDto();

            try
            {
                using (var client = new HttpClient())
                {
                    string url = config.GetSection("URLAPI").Value;
                    var responseTask = client.GetAsync(url + "/backend/question/one");
                    responseTask.Wait();
                    var response = responseTask.Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = response.Content.ReadAsStringAsync().Result;
                        result = JsonConvert.DeserializeObject<QuestionOneDto>(readTask);
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
                QuestionOneDto question = getQuestion();
                QuestionOneResponse resp = new QuestionOneResponse { 
                    id = question.id,
                    title = question.title,
                    description = question.description,
                    footer = question.footer,
                    createdAt = question.createdAt
                };

                return Ok(resp);
            } catch(Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
