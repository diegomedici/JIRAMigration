using System.Collections.Generic;

namespace JIRAMigration.Classes
{
    public class Comment
    {
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public List<CommentDetail> comments { get; set; } 
    }
}