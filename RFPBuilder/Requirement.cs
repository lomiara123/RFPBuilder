using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFPBuilder
{
    class Requirement
    {
        public string Id { get; }
        public string Response { get; }
        public string Comments { get; }
        public string Criticality { get; }

        public Requirement()
        {

        }

        public Requirement(string id, string response, string comments, string criticality)
        {
            Id = id;
            Response = response;
            Comments = comments;
            Criticality = criticality;
        }
    }
}
