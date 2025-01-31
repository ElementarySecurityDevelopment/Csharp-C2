﻿using ApiModels.Requests;
using Microsoft.AspNetCore.Mvc;
using TeamServer.Services;
using TeamServer.Models;
using TeamServer.Models.Listeners;

namespace TeamServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListenersController : ControllerBase
    {
        private readonly IListenerServices _listeners;
        private readonly IAgentService _agentServices;
        public ListenersController(IListenerServices listeners, IAgentService agentService) {

            _listeners = listeners;
            _agentServices = agentService;


        }
        [HttpGet]
        public IActionResult GetListeners()
        {
            var listeners = _listeners.GetListeners();
            return Ok(listeners);
        }
        [HttpGet("{name}")]
        public IActionResult GetListener(string name)
        {
            var listener = _listeners.GetListener(name);
            if(listener is null)  return NotFound();
            return Ok(listener);
        }
        [HttpPost]
        public IActionResult StartListener([FromBody] StartHttpListenerRequest request)
        {
            var listener = new HttpListener(request.Name, request.BindPort);
            listener.Init(_agentServices);
            listener.Start();

            _listeners.AddListener(listener);

            var root = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            var path = $"{root}/{listener.Name}";

            return Created(path, listener);
        }

        [HttpDelete]
        public IActionResult StopListener(string name) { 
            var listener = _listeners.GetListener(name);
            if (listener is null) return NotFound();
            listener.Stop();

            _listeners.RemoveListener(listener);
            return NoContent();
        }
    }
}
