using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Agent.Models
{
    [DataContract]
    public class AgentTask
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "command")]
        public string Command { get; set; }

        [DataMember(Name = "arguments", IsRequired = false)]
        public string[] Arguments { get; set; }

        [DataMember(Name = "file", IsRequired = false)]
        public byte[] File { get; set; }
    }
}
