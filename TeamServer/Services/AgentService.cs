﻿using TeamServer.Models;
using TeamServer.Models.Agents;
using TeamServer.Models.Listeners;

namespace TeamServer.Services
{
    public interface IAgentService
    {

        void AddAgent(Agent agent);
        IEnumerable<Agent> GetAgents();
        Agent GetAgent(string id);
        void RemoveAgent(Agent agent);
    }

    public class AgentService : IAgentService
    {
        private readonly List<Agent> _agents = new();
        public void AddAgent(Agent agent)
        {
            _agents.Add(agent);
        }

        public Agent GetAgent(string id)
        {
            return GetAgents().FirstOrDefault(a => a.Metadata.Id.Equals(id));
        }

        public IEnumerable<Agent> GetAgents()
        {
            return _agents;
        }

        public void RemoveAgent(Agent agent)
        {
            _agents.Remove(agent);
        }
    }
}
