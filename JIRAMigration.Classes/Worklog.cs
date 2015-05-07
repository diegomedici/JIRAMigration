using System.Collections.Generic;

namespace JIRAMigration.Classes
{
    public class Worklog
    {
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public List<object> worklogs { get; set; } 
    }
}