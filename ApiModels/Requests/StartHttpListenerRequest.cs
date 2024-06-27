using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Requests
{
    public class StartHttpListenerRequest
    {

        public String Name { get; set; }

        public int BindPort { get; set; }
    }
}
