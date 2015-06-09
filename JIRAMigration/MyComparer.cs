using System.Collections.Generic;
using TechTalk.JiraRestClient;

namespace JIRAMigration
{
    public class MyComparer : IComparer<Issue>
    {
        public int Compare(Issue x, Issue y)
        {
            if (x.id.Equals(y.id)) return 0;
            if (int.Parse(x.id) < int.Parse(y.id)) return -1;
            return 1;
        }
    }
}