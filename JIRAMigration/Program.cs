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
        private static string _password = string.Empty;
        //private const string Projects = "WEBDPC";
        //private const string DestProject = "DPC";

        //private const string Projects = "SCA";
        //private const string DestProject = "SCA";

        //private const string Projects = "WBC";
        //private const string DestProject = "WBC";

        //private const string Projects = "ATT";
        //private const string DestProject = "ATT";

        //private const string Projects = "ESTAT";
        //private const string DestProject = "ESTAT";

        private const string Projects = "ATT,ESTAT,GIANO,LRX,SWAT,ZEN";
        private const string DestProject = "SWAT";

        static void Main(string[] args)
        {

            try
            {
                Console.WriteLine("User: diego.medici");
                Console.Write("Password: ");
                _password = ReadLineMasked('*');

                JiraClient jiraClient = new JiraClient("https://studiofarma.atlassian.net", UserName, _password);

                string[] elencoProject = Projects.Split(',');
                ListIssueCGM issueCgms = new ListIssueCGM();

                foreach (var project in elencoProject)
                {

                    IEnumerable<Issue> issues = jiraClient.GetIssues(project).OrderBy(k => k, new MyComparer()).ToList();

                    foreach (var locIssue in issues)
                    {
                         IssueSTF issueStf = GetIssueSTF(locIssue.self);

                        //Console.Write("Reading {0}...", issueStf.key);
                        IssueCGM issueCgm = new IssueCGM(issueStf, project, DestProject);

                        issueCgms.Add(issueCgm);

                        //Console.WriteLine(" done!");

                    }

                    foreach(IssueCGM issue in IssueCGM.IssueToDo)
                    {
                        if (IssueCGM.Maps.ContainsKey(issue.OriginalParentIssueKey))
                        {
                            issue.DestinationParentIssueKey = IssueCGM.Maps[issue.OriginalParentIssueKey];
                        }
                    }


                    foreach (IssueCGM issue in IssueCGM.EpicsToLink)
                    {
                        if (IssueCGM.MapsEpics.ContainsKey(issue.EpicLinkKey))
                        {
                            issue.EpicLink = IssueCGM.MapsEpics[issue.EpicLinkKey];
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
                    Console.WriteLine("Lines {0} Ok", i);
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
                self, id, UserName, _password);
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
                self, UserName, _password);
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
                self, UserName, _password);
            IssueSTF issue;

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                string json = client.DownloadString(url);
                issue = JsonConvert.DeserializeObject<IssueSTF>(json);
            }
            return issue;
        }

        public static string ReadLineMasked(char mask = '*')
        {
            var sb = new StringBuilder();
            ConsoleKeyInfo keyInfo;
            while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (!char.IsControl(keyInfo.KeyChar))
                {
                    sb.Append(keyInfo.KeyChar);
                    Console.Write(mask);
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);

                    if (Console.CursorLeft == 0)
                    {
                        Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                        Console.Write(' ');
                        Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                    }
                    else Console.Write("\b \b");
                }
            }
            Console.WriteLine();
            return sb.ToString();
        }


        
    }

   
}


