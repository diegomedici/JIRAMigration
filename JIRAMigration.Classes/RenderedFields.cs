using System.Collections.Generic;

namespace JIRAMigration.Classes
{
    public class RenderedFields
    {
        public object issuetype { get; set; }
        public object timespent { get; set; }
        public object project { get; set; }
        public object fixVersions { get; set; }
        public object customfield_11200 { get; set; }
        public object aggregatetimespent { get; set; }
        public object resolution { get; set; }
        public object customfield_11401 { get; set; }
        public object customfield_11400 { get; set; }
        public string customfield_10503 { get; set; }
        public object resolutiondate { get; set; }
        public object workratio { get; set; }
        public string lastViewed { get; set; }
        public object watches { get; set; }
        public string created { get; set; }
        public object priority { get; set; }
        public object customfield_10102 { get; set; }
        public object labels { get; set; }
        public object timeestimate { get; set; }
        public object aggregatetimeoriginalestimate { get; set; }
        public object versions { get; set; }
        public object issuelinks { get; set; }
        public object assignee { get; set; }
        public string updated { get; set; }
        public object status { get; set; }
        public object components { get; set; }
        public object timeoriginalestimate { get; set; }
        public string description { get; set; }
        public object customfield_11300 { get; set; }
        public object customfield_11500 { get; set; }
        public Timetracking timetracking { get; set; }
        public object customfield_10005 { get; set; }
        public object customfield_10203 { get; set; }
        public object customfield_10204 { get; set; }
        public object customfield_10006 { get; set; }
        public object customfield_10600 { get; set; }
        public object customfield_10205 { get; set; }
        public object customfield_10601 { get; set; }
        public object customfield_10007 { get; set; }
        public object customfield_10800 { get; set; }
        public object customfield_10008 { get; set; }
        public List<Attachment> attachment { get; set; }
        public object customfield_10009 { get; set; }
        public object aggregatetimeestimate { get; set; }
        public object summary { get; set; }
        public object creator { get; set; }
        public object subtasks { get; set; }
        public object reporter { get; set; }
        public object customfield_10000 { get; set; }
        public object aggregateprogress { get; set; }
        public object customfield_10001 { get; set; }
        public object customfield_10200 { get; set; }
        public object customfield_10002 { get; set; }
        public object customfield_10003 { get; set; }
        public object customfield_10201 { get; set; }
        public object customfield_10202 { get; set; }
        public object customfield_10004 { get; set; }
        public string environment { get; set; }
        public object duedate { get; set; }
        public object progress { get; set; }
        public Comment comment { get; set; }
        public object votes { get; set; }
        public Worklog worklog { get; set; }
    }
}