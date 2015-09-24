using System.Collections.Generic;
using JIRAMigration.Classes.Export;

namespace JIRAMigration.Classes
{
    public class Fields
    {
        public Issuetype issuetype { get; set; }
        public Parent parent { get; set; }
        public int? timespent { get; set; }
        public Project project { get; set; }
        public List<Version> fixVersions { get; set; }
        public string customfield_11200 { get; set; }
        public object aggregatetimespent { get; set; }
        public Resolution resolution { get; set; }
        public object customfield_11401 { get; set; }
        public object customfield_11400 { get; set; }
        public string customfield_11000 { get; set; } //Change Log
        public string customfield_10503 { get; set; } //Acceptance Criteria
        public CustomProject customfield_11100 { get; set; } //Project only 4 SWAT
        public object resolutiondate { get; set; }
        public string workratio { get; set; }
        public string lastViewed { get; set; }
        public Watches watches { get; set; }
        public string created { get; set; }
        public Priority priority { get; set; }
        public object customfield_10102 { get; set; }
        public List<object> labels { get; set; }
        public object timeestimate { get; set; }
        public object aggregatetimeoriginalestimate { get; set; }
        public List<Version> versions { get; set; }
        public List<object> issuelinks { get; set; }
        public Author assignee { get; set; }
        public string updated { get; set; }
        public Status status { get; set; }
        public List<Component> components { get; set; }
        public object timeoriginalestimate { get; set; }
        public string description { get; set; }
        public object customfield_11300 { get; set; }
        public CostUnit customfield_11500 { get; set; }
        public Timetracking timetracking { get; set; }
        public object customfield_10005 { get; set; }
        public object customfield_10203 { get; set; }
        public object customfield_10204 { get; set; }
        public string customfield_10006 { get; set; }
        public object customfield_10600 { get; set; }
        public object customfield_10205 { get; set; }
        public string customfield_10601 { get; set; }
        public List<string> customfield_10007 { get; set; }
        public object customfield_10800 { get; set; }
        public object customfield_10008 { get; set; }
        public List<Attachment> attachment { get; set; }
        public object customfield_10009 { get; set; }
        public object aggregatetimeestimate { get; set; }
        public string summary { get; set; }
        public Creator creator { get; set; }
        public List<object> subtasks { get; set; }
        public Reporter reporter { get; set; }
        public object customfield_10000 { get; set; }
        public Aggregateprogress aggregateprogress { get; set; }
        public object customfield_10001 { get; set; }
        public object customfield_10200 { get; set; }
        public object customfield_10002 { get; set; }
        public object customfield_10003 { get; set; }
        public object customfield_10201 { get; set; }
        public object customfield_10202 { get; set; }
        public object customfield_10004 { get; set; }

        public string customfield_10801 { get; set; } //epicname
        public object environment { get; set; }
        public object duedate { get; set; }
        public Progress progress { get; set; }
        public Comment comment { get; set; }
        public Votes votes { get; set; }
        public Worklog worklog { get; set; }

    }
}