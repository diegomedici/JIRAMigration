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
        private const string userName = "diego.medici";
        private const string password = "S4r4eCr1st1n4";

        static void Main(string[] args)
        {

            try
            {
               

                JiraClient jiraClient = new JiraClient("https://studiofarma.atlassian.net", userName, password);
                IEnumerable<Issue> issues = jiraClient.GetIssues("ATT");

                List<IssueCGM> issueCgms = new List<IssueCGM>(issues.Count());

                foreach (var locIssue in issues)
                {

                    IssueSTF issueStf = GetIssueSTF(locIssue.self);

                    //Issue issue = jiraClient.LoadIssue(locIssue);

                    //Console.WriteLine("{0} - {1}", issue.key, issue.fields.summary);

                    IssueCGM issueCgm = new IssueCGM(issueStf);
                    issueCgms.Add(issueCgm);


                    //var fields = issue.fields;
                    //IEnumerable<Comment> issueComments = issue.fields. .GetComments(issue);
                    //IEnumerable<Attachment> issueAttachments = jiraClient.GetAttachments(issue);
                    //IEnumerable<IssueLink> issueLinks = jiraClient.GetIssueLinks(issue);
                    //IEnumerable<RemoteLink> issueRemoteLinks = jiraClient.GetRemoteLinks(issue);
                    //IEnumerable<Transition> issueTransitions = jiraClient.GetTransitions(issue);

                    //foreach (var issueAttachment in issue.fields.attachment)
                    //{
                    //    Attachment attach = GetAttach(issueAttachment.self);
                    //    Console.WriteLine(" - Create {0} - Author {1} - Filename {2}", attach.created, attach.author.key, attach.filename);
                    //    //string fileUri = string.Empty;
                    //    //string url = string.Format("{0}?&os_username={1}&os_password={2}", fileUri, userName, password);
                    //    //webClient.DownloadFile(new Uri( fileUri + "?&os_username=usernamehere&os_password=passwordhere"), issueAttachment.filename);
                    //}

                    //foreach (var issueComment in issue.fields.comments)
                    //{
                    //    Comment2 comment2 = GetComment(issue.self, issueComment.id);
                    //    Console.WriteLine(" - Create {0} - Author {1} - Comment {2}", comment2.created, comment2.author.emailAddress, comment2.body);
                    //}

                    
                }
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
                self, id, userName, password);
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
                self, userName, password);
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
                self, userName, password);
            IssueSTF issue;

            using (WebClient client = new WebClient())
            {
                string json = client.DownloadString(url);
                issue = JsonConvert.DeserializeObject<IssueSTF>(json);
            }
            return issue;
        }




        
    }
}
