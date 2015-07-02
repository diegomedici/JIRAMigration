using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Text;
using System.Xml.Schema;
using TechTalk.JiraRestClient;

namespace JIRAMigration.Classes.Export
{
    public class IssueCGM
    {
        public static int Counter = 0;
        public static Dictionary<string, string> Maps = new Dictionary<string, string>();
        public static List<IssueCGM> IssueToDo = new List<IssueCGM>();

        private static Dictionary<string, string> StatusDictionary = new Dictionary<string, string>()
        {
            {"Backlog", "Open"},
            {"Ready", "Selected"},
            {"In Progress", "In Progress"},
            {"Acceptance", "In Progress"},
            {"Accepted", "Close"},
            {"Development done", "In Review"},
            {"Closed", "Closed"},
            {"Graveyard", "Graveyard"}
        };

        private string GetNextCounter()
        {
            Counter++;
            return Counter.ToString();
        }

        public IssueCGM(IssueSTF issue, string project, string destProject)
        {
            string projectWithDash = project + "-";
            string destProjectWithDash = destProject + "-";

            Labels = new List<string>();
            AffectVersion = new List<string>();
            FixVersion = new List<string>();
            Components = new List<string>();
            CommentsBody = new List<CommentCgm>();
            Attachments = new List<string>();
            OriginalIssueKey = issue.key;
            DestinationIssueKey = destProjectWithDash+GetNextCounter();
            Maps.Add(OriginalIssueKey, DestinationIssueKey);

            if (issue.fields.parent != null)
            {
                OriginalParentIssueKey = issue.fields.parent.key;
                if (Maps.ContainsKey(OriginalParentIssueKey))
                {
                    DestinationParentIssueKey = Maps[OriginalParentIssueKey];
                }
                else
                {
                    IssueToDo.Add(this);
                }
            }

            IssueType = issue.fields.issuetype.name;
            if (issue.fields.reporter != null)
            {
                Reporter = issue.fields.reporter.emailAddress.Replace("@studiofarma.it", "@cgm.com");
            }
            DateCreated = issue.fields.created;
            Summary = issue.fields.summary.Replace(projectWithDash, destProjectWithDash);
            if (issue.fields.labels != null)
                foreach (string label in issue.fields.labels)
                {
                    Labels.Add(label);
                }

            if (issue.fields.customfield_11100 != null)
            {
                Labels.Add(issue.fields.customfield_11100.value);
            }

            if (issue.fields.assignee != null)
                Assignee = issue.fields.assignee.emailAddress.Replace("@studiofarma.it", "@cgm.com");

            if (issue.fields.description != null)
            {
                Descripton = issue.fields.description.Replace(projectWithDash, destProjectWithDash);
            }

            if (issue.fields.customfield_11500 != null)
            {
                CostUnit = issue.fields.customfield_11500.value;
            }
            else
            {
                CostUnit = "Order externalcustomer";
            }

            if (issue.fields.priority != null)
            {
                Priority = issue.fields.priority.name;
            }
            EpicName = issue.fields.customfield_10801;
            AcceptanceCriteria = issue.fields.customfield_10503;
            ChangeLog = issue.fields.customfield_11000;

            if (issue.fields.fixVersions != null)
            {
                foreach (Version version in issue.fields.fixVersions)
                {
                    FixVersion.Add(version.name);
                }
            }

            if (issue.fields.components != null)
            {
                foreach (Component component in issue.fields.components)
                {
                    Components.Add(component.name);
                }
            }

            if (!project.Equals(destProject))
            {
                Labels.Add(project);
            }

            CommentCgm oldLink = new CommentCgm();
            oldLink.Author = "diego.medici@cgm.com";
            oldLink.Created = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss+0000");
            oldLink.Body = "Link issue originale su JIRA Studiofarma: https://studiofarma.atlassian.net/browse/" + OriginalIssueKey;
            CommentsBody.Add(oldLink);


            if (!string.IsNullOrEmpty(AcceptanceCriteria))
            {
                CommentCgm acceptCriteria = new CommentCgm();
                acceptCriteria.Author = Reporter;
                acceptCriteria.Created = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss+0000");
                acceptCriteria.Body = "AcceptanceCriteria\\r\\n" + AcceptanceCriteria;
                CommentsBody.Add(acceptCriteria);
            }
          

            if (issue.fields.comment.comments != null)
            {
                foreach (CommentDetail commentDetail in issue.fields.comment.comments)
                {
                    CommentsBody.Add(new CommentCgm(commentDetail, projectWithDash, destProjectWithDash));
                }
            }

            if (issue.fields.attachment != null)
            {
                foreach (var attach in issue.fields.attachment)
                {
                    var fullPathDesintationName = CopyFile(project, destProject, attach);
                    if (!string.IsNullOrEmpty(fullPathDesintationName))
                    {
                        Attachments.Add(fullPathDesintationName.Replace(DestinationDir, ""));                        
                    }
                }
            }

            try
            {
                Status = StatusDictionary[issue.fields.status.name];
            }
            catch (Exception e)
            {
                Console.WriteLine("Status key not found: " + issue.fields.status.name);
            }
            

        }

        public const string DestinationDir = @"C:\CGM\attachments\";

        private string CopyFile(string project, string destProject, Attachment attach)
        {
            string fullPathName = @"C:\Users\diego.medici\Downloads\JIRA-backup-20150608\data\attachments\" + project + @"\" +
                                  OriginalIssueKey + @"\";
            var listNameSplitted = new List<string>(attach.content.Split('/'));
            string fileName = listNameSplitted[listNameSplitted.Count - 1];
            string fileCode = listNameSplitted[listNameSplitted.Count - 2];
            fullPathName += fileCode;
            FileInfo file = new FileInfo(fullPathName);
            if (!file.Exists)
            {
                return null;
            }
            string fullPathDesintationName = DestinationDir + destProject + @"\" + DestinationIssueKey;
            DirectoryInfo directoryDest = new DirectoryInfo(fullPathDesintationName);
            if (!directoryDest.Exists)
            {
                directoryDest.Create();
            }
            fullPathDesintationName += @"\" + fileName;
            if (!File.Exists(fullPathDesintationName))
            {
                file.CopyTo(fullPathDesintationName, true);
            }
            return fullPathDesintationName;
        }

        public string OriginalIssueKey { get; set; }
        public string DestinationIssueKey { get; set; }
        public string OriginalParentIssueKey { get; set; }
        public string DestinationParentIssueKey { get; set; }
        public string IssueType { get; set; }
        public string Reporter { get; set; }
        public string DateCreated { get; set; }
        public string Summary { get; set; }
        public List<string> Labels { get; set; }
        public string Assignee { get; set; }
        public string Descripton { get; set; }
        public string CostUnit { get; set; }
        public string Priority { get; set; }
        public string EpicName { get; set; }
        public string AcceptanceCriteria { get; set; }
        public string ChangeLog { get; set; }
        public List<string> AffectVersion { get; set; }
        public List<string> Attachments { get; set; }
        public List<CommentCgm> CommentsBody { get; set; }
        public List<string> Components { get; set; }
        public List<string> FixVersion { get; set; }
        public string Status { get; set; }

        public string ToCSV(int maxLabels, int maxAffVer, int maxComponent, int maxFixVer, int maxComment, int maxAttachment)
        {
            StringBuilder str = new StringBuilder();
            str.Append(string.Format("{0};", DestinationIssueKey));
            str.Append(string.Format("{0};", DestinationParentIssueKey));
            str.Append(string.Format("\"{0}\";", IssueType));
            str.Append(string.Format("\"{0}\";", Reporter));
            str.Append(string.Format("\"{0}\";", FormatDate(DateCreated)));
            str.Append(string.Format("\"{0}\";", Summary));
            str.Append(AppendString(Labels, maxLabels));
            str.Append(string.Format("\"{0}\";", Assignee));
            //str.Append(string.Format("\"{0}\";", Descripton.Replace("\r\n", "$")));
            str.Append(string.Format("\"{0}\";", Descripton));
            //str.Append(string.Format("\"{0}\";", AcceptanceCriteria));
            str.Append(string.Format("\"{0}\";", ChangeLog));
            str.Append(string.Format("\"{0}\";", CostUnit));
            str.Append(string.Format("\"{0}\";", Priority));
            str.Append(string.Format("\"{0}\";", EpicName));
            str.Append(AppendString(AffectVersion, maxAffVer));
            //str.Append(string.Format("\"{0}\";", Attachments+";");
            str.Append(AppendString(Components, maxComponent));
            str.Append(AppendString(FixVersion, maxFixVer));
            str.Append(string.Format("\"{0}\";", Status));
            str.Append(AppendComments(CommentsBody, maxComment));
            str.Append(AppendAttachment(Attachments, maxAttachment));
            return str.ToString();

        }

        private string FormatDate(string date)
        {
            string newDate = date.Replace('T', ' ');
            newDate = newDate.Remove(19);
            return newDate;
        }

        private string AppendComments(List<CommentCgm> commentsBody, int max)
        {
            string str = string.Empty;
            for (int i = 0; i < max; i++)
            {
                if (i >= commentsBody.Count)
                    str = str + ";";
                else
                    str = str + string.Format("\"{0};{1};{2}\";", FormatDate(commentsBody[i].Created), commentsBody[i].Author, commentsBody[i].Body);
            }

            return str;
        }

        private string AppendString(List<string> list, int max)
        {
            string str= string.Empty;
            for (int i = 0; i < max; i++)
            {
                if (i >= list.Count)
                    str = str + ";";
                else
                    str = str + string.Format("\"{0}\";", list[i]);
            }

            return str;

        }

        private string AppendAttachment(List<string> list, int max)
        {
            string str = string.Empty;
            for (int i = 0; i < max; i++)
            {
                if (i >= list.Count)
                    str = str + ";";
                else
                    str = str + string.Format("\"file://{0}\";", list[i].Replace("\\", "/"));
            }

            return str;

        }


    }
}