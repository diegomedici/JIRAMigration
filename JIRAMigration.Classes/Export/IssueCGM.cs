using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RestSharp.Contrib;

namespace JIRAMigration.Classes.Export
{
    public class IssueCGM
    {
        public static int Counter = 0;
        public static Dictionary<string, string> Maps = new Dictionary<string, string>();
        public static Dictionary<string, string> MapsEpics = new Dictionary<string, string>();
        public static List<IssueCGM> IssueToDo = new List<IssueCGM>();
        public static List<IssueCGM> EpicsToLink = new List<IssueCGM>();

        private static Dictionary<string, string> StatusDictionary = new Dictionary<string, string>()
        {
            {"Backlog", "Open"},
            {"Ready", "Selected"},
            {"In Progress", "In Progress"},
            {"Acceptance", "In Progress"},
            {"Accepted", "Closed"},
            {"Development done", "In Review"},
            {"Closed", "Closed"},
            {"Graveyard", "Closed"}
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

            if (issue.fields.customfield_10800 != null)
            {
                EpicLinkKey = issue.fields.customfield_10800.ToString();
                if (MapsEpics.ContainsKey(EpicLinkKey))
                {
                    EpicLink = MapsEpics[EpicLinkKey];
                }
                else
                {
                    EpicsToLink.Add(this);
                }
            }

            switch (issue.fields.issuetype.name)
            {
                case "Non-Dev Task":
                    IssueType = "Task";
                    Labels.Add("Non-Dev Task");
                    break;
                case "Improvement":
                    IssueType = "Task";
                    Labels.Add("Improvement");
                    break;
                default:
                    IssueType = issue.fields.issuetype.name;
                    break;
            }

            if (issue.fields.reporter != null)
            {
                Reporter = EmailChanger(issue.fields.reporter.emailAddress, "reporter");
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
                Assignee = EmailChanger(issue.fields.assignee.emailAddress, "assignee");

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
                CostUnit = "Order external Customer";
            }

            if (issue.fields.priority != null)
            {
                Priority = issue.fields.priority.name;
            }

            EpicName = issue.fields.customfield_10801;
            if (!string.IsNullOrEmpty(EpicName))
            {
                MapsEpics.Add(OriginalIssueKey, EpicName);
            }

            AcceptanceCriteria = issue.fields.customfield_10503;
            ChangeLog = issue.fields.customfield_11000;
            Resolution = issue.fields.resolution == null ? "Unresolved" : issue.fields.resolution.name;
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
                if (issue.fields.status.name.Equals("Graveyard"))
                {
                    Resolution = "Rejected";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Status key not found: " + issue.fields.status.name);
            }
            Console.WriteLine("{0} -> {1} Res: {2} Status: {3} -> {4} ", issue.key, DestinationIssueKey, Resolution, issue.fields.status.name, Status);
        }

        private string EmailChanger(string emailAddress, string type)
        {
            string[] oldEmails = { "federico.rota@studiofarma.it", "nicola.febbrari@studiofarma.it", "gabriele.bonzi@studiofarma.it", "tiziana.pizzocaro@sca.com" };
            string newEmail = string.Empty;
            if (emailAddress != null)
            {
                if (oldEmails.Any(oldEmail => oldEmail.Equals(emailAddress)))
                {
                    newEmail = "diego.medici@cgm.com";
                    if (!string.IsNullOrEmpty(type))
                    {
                        CommentCgm oldLink = new CommentCgm();
                        oldLink.Author = "diego.medici@cgm.com";
                        oldLink.Created = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss+0000");
                        oldLink.Body = $"Original {type} {emailAddress}";
                        CommentsBody.Add(oldLink);
                    }
                }
                else
                {
                    newEmail = emailAddress.Replace("@studiofarma.it", "@cgm.com");
                }
            }
            return newEmail;
        }

        public const string DestinationDir = @"C:\CGM\attachments\";

        public string RemoveSpecialChars(string input)
        {
            return Regex.Replace(input, @"[^0-9a-zA-Z.]", "_");
        }

        private string CopyFile(string project, string destProject, Attachment attach)
        {
            string fullPathName = $@"C:\CGM\JIRA-backup\data\attachments\{project}\10000\{OriginalIssueKey}\";

            var listNameSplitted = new List<string>(attach.content.Split('/'));
            string fileName = listNameSplitted[listNameSplitted.Count - 1];
            fileName = HttpUtility.UrlDecode(fileName);
            fileName = fileName.Replace(' ', '_');
            fileName = RemoveSpecialChars(fileName);
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
        public string EpicLink { get; set; }
        public string EpicLinkKey { get; set; }
        public string AcceptanceCriteria { get; set; }
        public string ChangeLog { get; set; }
        public string Resolution { get; set; }
        public List<string> AffectVersion { get; set; }
        public List<string> Attachments { get; set; }
        public List<CommentCgm> CommentsBody { get; set; }
        public List<string> Components { get; set; }
        public List<string> FixVersion { get; set; }
        public string Status { get; set; }

        public string ToCSV(int maxLabels, int maxAffVer, int maxComponent, int maxFixVer, int maxComment, int maxAttachment)
        {
            StringBuilder str = new StringBuilder();
            str.Append($"{DestinationIssueKey};");
            str.Append($"{DestinationParentIssueKey};");
            str.Append($"\"{IssueType}\";");
            str.Append($"\"{Reporter}\";");
            str.Append($"\"{FormatDate(DateCreated)}\";");
            str.Append($"\"{(string.IsNullOrEmpty(Summary) ? string.Empty : Summary.Replace('\"', '\''))}\";");
            str.Append(AppendString(Labels, maxLabels));
            str.Append($"\"{Assignee}\";");
            //str.Append(string.Format("\"{0}\";", Descripton.Replace("\r\n", "$")));
            str.Append($"\"{(string.IsNullOrEmpty(Descripton) ? string.Empty : Descripton.Replace('\"', '\''))}\";");
            //str.Append(string.Format("\"{0}\";", AcceptanceCriteria));
            str.Append($"\"{(string.IsNullOrEmpty(ChangeLog) ? string.Empty : ChangeLog.Replace('\"', '\''))}\";");
            str.Append($"\"{CostUnit}\";");
            str.Append($"\"{Priority}\";");
            str.Append($"\"{EpicName}\";");
            str.Append($"\"{EpicLink}\";");
            str.Append(AppendString(AffectVersion, maxAffVer));
            //str.Append(string.Format("\"{0}\";", Attachments+";");
            str.Append(AppendString(Components, maxComponent));
            str.Append(AppendString(FixVersion, maxFixVer));
            str.Append($"\"{Status}\";");
            str.Append($"\"{Resolution}\";");
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
                    str = str +
                          $"\"{FormatDate(commentsBody[i].Created)};{EmailChanger(commentsBody[i].Author, "")};{commentsBody[i].Body.Replace('\"', '\'')}\";";
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
                    str = str + $"\"{list[i]}\";";
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
                    str = str + $"\"file://{list[i].Replace("\\", "/")}\";";
            }

            return str;

        }


    }
}