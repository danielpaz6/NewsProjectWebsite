﻿using NewsProject.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace NewsProject.Models
{
    public class GetNews
    {
        public void AddMakoNews(string url, Category cat, User user)
        {
            int counter = 0;
            int k = 0;
            //string url = "http://rcs.mako.co.il/rss/31750a2610f26110VgnVCM1000005201000aRCRD.xml";
            bool flag = false;

            string[,] arrayNews = new string[25, 11];

            XmlTextReader reader = new XmlTextReader(url);
            while (reader.Read())
            {
                if (flag == false)
                {
                    if (reader.Name != "item")
                    {
                        continue;
                    }
                    else
                        flag = true;
                }

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        //Console.Write("<" + reader.Name);
                        //Console.WriteLine(">");
                        break;

                    case XmlNodeType.Text: //Display the text in each element.
                        //Console.WriteLine(reader.Value);
                        arrayNews[k, counter] = reader.Value;

                        break;

                    case XmlNodeType.EndElement: //Display the end of the element.
                        //Console.Write("</" + reader.Name);
                        //Console.WriteLine(">");
                        counter++;
                        break;
                }

                if (counter == 12)
                {
                    k++;
                    counter = 0;
                }
            }

            //Console.WriteLine(arrayNews[3, 2]);
            //Console.ReadKey();

            //return arrayNews;


            for (int i = 0; i < 18; i++)
            {
                Article a = new Article();

                a.Title = arrayNews[i, 0];
                a.Description = arrayNews[i, 1];
                a.Date = arrayNews[i, 3];
                a.NumOfLikes = 0;
                a.ImageLink = arrayNews[i, 9];
                a.ArticleLink = arrayNews[i, 2];
                a.Category = cat;
                a.CategoryId = cat.CategoryId;
                a.User = user;
                a.UserId = user.UserId;

                ArticlesController ac = new ArticlesController();
                ac.Create(a);
            }
        }

        public List<string[]> Add_CNN_News()
        {
            string url = "http://rss.cnn.com/rss/edition.rss";
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feeds = SyndicationFeed.Load(reader); // References -> Right Click -> Add Reference -> System.ServiceModel
            reader.Close();
            List<String[]> lst = new System.Collections.Generic.List<string[]>();
            var ns = (XNamespace)"http://search.yahoo.com/mrss/";
            foreach (SyndicationItem item in feeds.Items)
            {
                String[] temp = new String[4];
                if (item.Summary == null)
                    continue;

                string subject = item.Title.Text;
                string summary = item.Summary.Text;
                string link = item.Links[0].Uri.ToString();

                var urls = from ext in item.ElementExtensions  // all extensions to ext
                           where ext.OuterName == "group" &&    // find the ones called group
                                 ext.OuterNamespace == ns       // in the right namespace
                           from content in ext.GetObject<XElement>().Elements(ns + "content") // get content elements
                           where (string)content.Attribute("medium") == "image"  // if that medium is an image
                           select (string)content.Attribute("url");

                if (urls.Count() < 5)
                    continue;

                string img = urls.ToArray()[3];

                temp[0] = subject;
                temp[1] = summary;
                temp[2] = link;
                temp[3] = img;

                lst.Add(temp);
            }
            return lst;
        }

        public List<string[]> Add_Ynet_News()
        {
            string url = "http://www.ynet.co.il/Integration/StoryRss3082.xml";
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feeds = SyndicationFeed.Load(reader); // References -> Right Click -> Add Reference -> System.ServiceModel
            reader.Close();
            var ns = (XNamespace)"http://search.yahoo.com/mrss/";
            List<String[]> lst = new System.Collections.Generic.List<string[]>();

            foreach (SyndicationItem item in feeds.Items)
            {
                String[] temp = new String[4];

                if (item.Summary == null)
                    continue;

                string subject = item.Title.Text;
                string summary = item.Summary.Text;
                string link = item.Links[0].Uri.ToString();
                string img = null;
                string desc = null;
                string pattern1 = "src='([^']*)'";
             

                RegexOptions options = RegexOptions.Multiline;

                foreach (Match m in Regex.Matches(summary, pattern1, options))
                {
                    img = m.Groups[1].ToString();
                }

                
                desc = summary.Split(new string[] {"</div>"}, StringSplitOptions.None)[1];
                temp[0] = subject;
                temp[1] = desc;
                temp[2] = link;
                temp[3] = img;

                lst.Add(temp);
            }
            return lst;
        }

        public List<string[]> Add_FOX_News()
        {
            string url = "http://feeds.foxnews.com/foxnews/latest";
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feeds = SyndicationFeed.Load(reader); // References -> Right Click -> Add Reference -> System.ServiceModel
            reader.Close();
            List<String[]> lst = new System.Collections.Generic.List<string[]>();
            var ns = (XNamespace)"http://search.yahoo.com/mrss/";
            foreach (SyndicationItem item in feeds.Items)
            {
                String[] temp = new String[4];
                if (item.Summary == null)
                    continue;

                string subject = item.Title.Text;
                string summary = item.Summary.Text;
                string link = item.Links[0].Uri.ToString();

                var urls = from ext in item.ElementExtensions  // all extensions to ext
                           where ext.OuterName == "group" &&    // find the ones called group
                                 ext.OuterNamespace == ns       // in the right namespace
                           from content in ext.GetObject<XElement>().Elements(ns + "content") // get content elements
                           where (string)content.Attribute("medium") == "image"  // if that medium is an image
                           select (string)content.Attribute("url");

                string img = urls.ToArray()[0];

                temp[0] = subject;
                temp[1] = summary;
                temp[2] = link;
                temp[3] = img;

                lst.Add(temp);
            }
            return lst;
        }
    }
}