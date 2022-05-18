namespace RFPBuilder
{
    class Requirement
    {
        public string Id { get; set; }
        public string Response { get; set; }
        public string Comments { get; set; }
        public string Criticality { get; set; }

        public Requirement() {

        }

        public Requirement(string id, string response, string comments, string criticality){
            Id = id;
            Response = response;
            Comments = comments;
            Criticality = criticality;
        }
    }
}
