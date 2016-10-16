using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Facebook
{
    public enum Reaction
    {
        LIKE, LOVE, HAHA, WOW, SAD, ANGRY
    }

    public class ReactionPerPerson
    {
        public string id { get; set; }
        public string name { get; set; }
        public Reaction type { get; set; }
    }
    public class ResponsePaging
    {
        public string next { get; set; }
        public string previous { get; set; }
        public Cursors cursors { get; set; }
    }
    public class Cursors
    {
        public string before { get; set; }
        public string after { get; set; }
    }
    public class Summary
    {
        public int total_count { get; set; }
        public string viewer_reaction { get; set; }
    }
    public class PostReactions
    {
        public List<ReactionPerPerson> data { get; set; }
        public ResponsePaging paging { get; set; }
        public Summary summary { get; set; }

    }
}