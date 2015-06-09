using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using JIRAMigration.Classes;
using JIRAMigration.Classes.Export;
using Newtonsoft.Json;
using RestSharp.Deserializers;
using TechTalk.JiraRestClient;
using Attachment = JIRAMigration.Classes.Attachment;

namespace JIRAMigration
{
    class Program
    {
        private const string UserName = "diego.medici";
        private const string Password = "S4r4eCr1st1n4";
        private const string Projects = "ATT,ESTAT,GIANO,LRX,SWAT,ZEN";
        private const string DestProject = "SWAT";

        static void Main(string[] args)
        {

            try
            {
                JiraClient jiraClient = new JiraClient("https://studiofarma.atlassian.net", UserName, Password);

                string[] elencoProject = Projects.Split(',');
                ListIssueCGM issueCgms = new ListIssueCGM();

                foreach (var project in elencoProject)
                {

                    IEnumerable<Issue> issues = jiraClient.GetIssues(project).OrderBy(k => k, new MyComparer()).ToList();

                    foreach (var locIssue in issues)
                    {
                        IssueSTF issueStf = GetIssueSTF(locIssue.self);

                        IssueCGM issueCgm = new IssueCGM(issueStf, project, DestProject);

                        issueCgms.Add(issueCgm);

                        Console.WriteLine(issueCgm.OriginalIssueKey);

                    }

                    foreach(IssueCGM issue in IssueCGM.IssueToDo)
                    {
                        if (IssueCGM.Maps.ContainsKey(issue.OriginalParentIssueKey))
                        {
                            issue.DestinationParentIssueKey = IssueCGM.Maps[issue.OriginalParentIssueKey];
                        }
                    }
                }

                //if (args[0] != null && !string.IsNullOrEmpty(args[0]))
                //{
                //    Issue issue = issues.SingleOrDefault(j => j.key == args[0]);
                //    IssueSTF issueStf = GetIssueSTF(issue.self);
                //}
               

                string[] lines = new string[issueCgms.Count + 1];
                lines[0] = issueCgms.FirstLine();
                int i = 1;
                foreach (IssueCGM issueCgm in issueCgms)
                {
                    lines[i++] = issueCgm.ToCSV(issueCgms.MaxCountLabel(), issueCgms.MaxCountAffcectVersion(),
                        issueCgms.MaxCountComponent(), issueCgms.MaxCountFixVersion(), issueCgms.MaxCountComments(), issueCgms.MaxCountAttachments());
                }
                const string nameFileCsv = @"C:\CGM\" + DestProject + ".csv";
                Console.WriteLine("Writing file {0}", nameFileCsv);
                File.WriteAllLines(nameFileCsv, lines);
                Console.WriteLine("Done!");
            }
            catch (Exception e)
            {
                
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();

        }

        public static CommentDetail GetComment(string self, string id)
        {
            string url = string.Format("{0}/comment/{1}?&os_username={2}&os_password={3}&expand=names,renderedFields",
                self, id, UserName, Password);
            CommentDetail commentDetail;

                using (WebClient client = new WebClient())
                {
                    string json = client.DownloadString(url);
                    commentDetail = JsonConvert.DeserializeObject<CommentDetail>(json);
                }
            return commentDetail;
        }


        public static Attachment GetAttach(string self)
        {
            string url = string.Format("{0}?os_username={1}&os_password={2}",
                self, UserName, Password);
            Attachment attach;

            using (WebClient client = new WebClient())
            {
                string json = client.DownloadString(url);
                attach = JsonConvert.DeserializeObject<Attachment>(json);
            }
            return attach;
        }


        public static IssueSTF GetIssueSTF(string self)
        {
            //string url = string.Format("{0}?os_username={1}&os_password={2}&expand=renderedFields,names",
            string url = string.Format("{0}?os_username={1}&os_password={2}",
                self, UserName, Password);
            IssueSTF issue;

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                string json = client.DownloadString(url);
                issue = JsonConvert.DeserializeObject<IssueSTF>(json);
            }
            return issue;
        }




        
    }

   
}


