using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace JIRAMigration.Classes.Export
{
    public class ListIssueCGM : List<IssueCGM>
    {
        public ListIssueCGM(int count) : base(count)
        {
            
        }

        public ListIssueCGM() 
        {
            
        }

        public int MaxCountLabel()
        {
            return this.Select(issueCgm => issueCgm.Labels.Count).Concat(new[] { 1 }).Max();
        }

        public int MaxCountAffcectVersion()
        {
            return this.Select(issueCgm => issueCgm.AffectVersion.Count).Concat(new[] { 1 }).Max();
        }

        public int MaxCountFixVersion()
        {
            return this.Select(issueCgm => issueCgm.FixVersion.Count).Concat(new[] { 1 }).Max();
        }

        public int MaxCountComponent()
        {
            return this.Select(issueCgm => issueCgm.Components.Count).Concat(new[] { 1 }).Max();
        }

        public int MaxCountComments()
        {
            return this.Select(issueCgm => issueCgm.CommentsBody.Count).Concat(new[] { 1 }).Max();
        }

        public int MaxCountAttachments()
        {
            return this.Select(issueCgm => issueCgm.Attachments.Count).Concat(new[] { 1 }).Max();
        }


        public string FirstLine()
        {
            StringBuilder str = new StringBuilder();
            str.Append("IssueId;");
            str.Append("ParentIssueId;");
            str.Append("Issue Type;");
            str.Append("Reporter;");
            str.Append("Date Created;");
            str.Append("Summary;");
            for (int i = 0; i < MaxCountLabel(); i++)
            {
                str.Append("Labels;");
            }
            str.Append("Assignee;");
            str.Append("Descripton;");
            //str.Append("Acceptance Criteria;");
            str.Append("Public Release Note;");
            str.Append("Cost Unit;");
            str.Append("Priority;");
            str.Append("Epic Name;");
            str.Append("Epic Link;");
            for (int i = 0; i < MaxCountAffcectVersion(); i++)
            {
                str.Append("Affects Version;");
            }
            for (int i = 0; i < MaxCountComponent(); i++)
            {
                str.Append("Components;");
            }
            for (int i = 0; i < MaxCountFixVersion(); i++)
            {
                str.Append("Fix Version;");
            }
            str.Append("Status;");
            str.Append("Resolution;");

            for (int i = 0; i < MaxCountComments(); i++)
            {
                str.Append("Comment;");
            }


            for (int i = 0; i < MaxCountAttachments(); i++)
            {
                str.Append("Attachment;");
            }

            return str.ToString();
        }


    }
}