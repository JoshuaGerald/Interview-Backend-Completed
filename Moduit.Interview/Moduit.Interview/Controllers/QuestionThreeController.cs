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
    public class QuestionThreeController : ControllerBase
    {
        private IConfiguration config;

        public QuestionThreeController(IConfiguration iConfig)
        {
            config = iConfig;
        }

        private List<QuestionThreeDto> getQuestion()
        {
            List<QuestionThreeDto> result = new List<QuestionThreeDto>();

            try
            {
                using (var client = new HttpClient())
                {
                    string url = config.GetSection("URLAPI").Value;
                    var responseTask = client.GetAsync(url + "/backend/question/three");
                    responseTask.Wait();
                    var response = responseTask.Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = response.Content.ReadAsStringAsync().Result;
                        result = JsonConvert.DeserializeObject<List<QuestionThreeDto>>(readTask);
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
                List<QuestionThreeDto> question = getQuestion();
                List<QuestionThreeResponse> resp = new List<QuestionThreeResponse>();

                foreach(var quest in question)
                {
                    if(quest.items != null)
                    {
                        foreach(var childQuest in quest.items)
                        {
                            resp.Add(new QuestionThreeResponse
                            {
                                id = quest.id,
                                category = quest.category,
                                title = childQuest.title,
                                description = childQuest.description,
                                footer = childQuest.footer,
                                createdAt = quest.createdAt
                            });
                        }
                    } else
                    {
                        resp.Add(new QuestionThreeResponse
                        {
                            id = quest.id,
                            category = quest.category,
                            createdAt = quest.createdAt
                        });
                    }
                }

                return Ok(resp);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
