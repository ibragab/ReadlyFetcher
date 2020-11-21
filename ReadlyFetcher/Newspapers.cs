﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ReadlyFetcher
{
    partial class Program
    {

        static void getAllNewspapers(string country)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Content-Type", "application/json");

                string mainjson = client.DownloadString("https://d3og6tlt23zks5.cloudfront.net/newspapers?ppage=1&per_page=1000000" + "&sort=publish_date&safe=false");
                Publications.Root publications = JsonConvert.DeserializeObject<Publications.Root>(mainjson);

                Console.WriteLine("Found: " + publications.Content.Count + " publications.");

                foreach (Publications.Content publication in publications.Content)
                {
                    Console.WriteLine("Fetching: " + publication.Title);
                    getNewspaperIssues(publication.Id);
                }
            }
        }

        static void getNewspaperIssues(string pubid)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Content-Type", "application/json");
                string json = client.DownloadString("https://d3og6tlt23zks5.cloudfront.net/newspapers/" + pubid + "?ppage=1&per_page=1000000");

                Issues.Root issues = JsonConvert.DeserializeObject<Issues.Root>(json);
                //client.Headers.Clear();

                Console.WriteLine("Found: " + issues.Content.Count + " issues.");
                foreach (Issues.Content issue in issues.Content)
                {
                    string issueNum = "";
                    if (issueformat == "issue")
                    {
                        if (issue.Issue == null)
                        {
                            issueNum = issue.PublishDate.ToString(@"yyyy-MM-dd");
                        }
                        else
                        {
                            issueNum = issue.Issue;
                        }
                    }
                    else
                    {
                        issueNum = issue.PublishDate.ToString(@"yyyy-MM-dd");
                    }


                    Console.WriteLine("=============================================================================");
                    Console.WriteLine("Fetching: " + issue.Title + " - " + issueNum);

                    if (outtype == "pdf")
                    {
                        GetPDF(issue.Id, issue.Title, issueNum);
                    }
                    else
                    {
                        getImg(issue.Id, issue.Title, issueNum);
                    }
                }
            }
        }
    }
}
