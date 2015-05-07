using System.Collections.Generic;
using System.Text;
using TechTalk.JiraRestClient;

namespace JIRAMigration.Classes.Export
{
    public class IssueCGM
    {

        public IssueCGM(IssueSTF issue)
        {
            Labels = new List<string>();
            AffectVersion = new List<string>();
            FixVersion = new List<string>();
            Components = new List<string>();

            IssueType = issue.fields.issuetype.name;
            Reporter = issue.fields.reporter.emailAddress;
            DateCreated = issue.fields.created;
            Summary = issue.fields.summary;
            if (issue.fields.labels != null)
                foreach (string label in issue.fields.labels)
                {
                    Labels.Add(label);
                }
            if (issue.fields.assignee != null)
                Assignee = issue.fields.assignee.emailAddress;

            Descripton = issue.fields.description;
            CostUnit = issue.fields.customfield_11500.value;
            Priority = issue.fields.priority.name;
            EpicName = issue.fields.customfield_10801;

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

            if (issue.fields.comment.comments != null)
            {
                foreach (CommentDetail commentDetail in issue.fields.comment.comments)
                {
                    CommentsBody.Add(new CommentCgm(commentDetail));
                }
            }
        }

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
        public List<string> AffectVersion { get; set; }
        public string Attachments { get; set; }
        public List<CommentCgm> CommentsBody { get; set; }
        public List<string> Components { get; set; }
        public List<string> FixVersion { get; set; }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append(IssueType + ";");
            str.Append(Reporter + ";");
            str.Append(DateCreated + ";");
            str.Append(Summary + ";");
            str.Append(AppendString(Labels));
            str.Append(Assignee +";");
            str.Append(Descripton +";");
            str.Append(CostUnit +";");
            str.Append(Priority +";");
            str.Append(EpicName +";");
            str.Append(AppendString(AffectVersion));
            str.Append(Attachments+";");
            str.Append(AppendComments(CommentsBody));
            str.Append(AppendString(Components));
            str.Append(AppendString(FixVersion));

            return str.ToString();

        }

        private string AppendComments(List<CommentCgm> commentsBody)
        {
            foreach (CommentCgm commentCgm in commentsBody)
            {
                //TODO
            }
            return string.Empty;
        }

        private string AppendString(List<string> list)
        {
            string str= string.Empty;
            foreach (string s in list)
            {
                str = s + ";";
            }
            return str;
        }
    }
}