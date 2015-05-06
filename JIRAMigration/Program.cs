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
using RestSharp.Deserializers;
using TechTalk.JiraRestClient;

namespace JIRAMigration
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                const string userName = "diego.medici";
                const string password = "S4r4eCr1st1n4";

                JiraClient jiraClient = new JiraClient("https://studiofarma.atlassian.net", userName, password);
                IEnumerable<Issue> issues = jiraClient.GetIssues("ATT");


                GetjsonStream(userName, password);

                foreach (var locIssue in issues)
                {

                    Issue issue = jiraClient.LoadIssue(locIssue);

                    Console.WriteLine("{0} - {1}", issue.key, issue.fields.summary);

                    //var fields = issue.fields;
                    //IEnumerable<Comment> issueComments = issue.fields. .GetComments(issue);
                    //IEnumerable<Attachment> issueAttachments = jiraClient.GetAttachments(issue);
                    //IEnumerable<IssueLink> issueLinks = jiraClient.GetIssueLinks(issue);
                    //IEnumerable<RemoteLink> issueRemoteLinks = jiraClient.GetRemoteLinks(issue);
                    //IEnumerable<Transition> issueTransitions = jiraClient.GetTransitions(issue);
                   
                    foreach (var issueAttachment in issue.fields.attachment)
                    {
                        Console.WriteLine(" - Attachment {0}", issueAttachment.filename);
                        //string fileUri = string.Empty;
                        //string url = string.Format("{0}?&os_username={1}&os_password={2}", fileUri, userName, password);
                        //webClient.DownloadFile(new Uri( fileUri + "?&os_username=usernamehere&os_password=passwordhere"), issueAttachment.filename);
                    }

                    foreach (var issueComment in issue.fields.comments)
                    {

                        //https://studiofarma.atlassian.net/rest/api/2/issue/ATT-4/comment/19906

                        Console.WriteLine(" - Comment {0}", issueComment.body);
                        Comment comment = new Comment();
                    }

                    
                }
            }
            catch (Exception e)
            {
                
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();

        }

        public static void GetjsonStream(string userName, string password)
        {

            string uri = "https://studiofarma.atlassian.net/rest/api/2/issue/ATT-4/comment/19906";
            string url = string.Format("{0}?&os_username={1}&os_password={2}", uri, userName, password);
            
                using (WebClient client = new WebClient())
                {
                    string stream = client.DownloadString(url);                    
                }



            
            //HttpClient client = new HttpClient();
            ////string url = "https://studiofarma.atlassian.net/rest/api/2/issue/ATT-4/comment/19906";
            //HttpResponseMessage response = await client.GetAsync(url);
            //Debug.WriteLine("Response: " + response);
            //return await response.Content.ReadAsStringAsync();
        }


        public static class JSONHelper
        {
            public static T Deserialise<T>(string json)
            {
                T obj = Activator.CreateInstance<T>();
                MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
                DataContractJsonSerializer serialiser = new DataContractJsonSerializer(obj.GetType());
                ms.Close();
                return obj;
            }
        }
    }
}
