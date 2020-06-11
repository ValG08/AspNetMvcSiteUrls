using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using UrlsProject.Common.Models;
using UrlsProject.Helpers;

namespace UrlsProject.Services
{
    public class SiteCollector: ISiteCollector
    {
        private int MaxPageTestTime { get; set; } = 50;

        public async Task<IEnumerable<SiteInfoModel>> Collect(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            try
            {             
                string host = Helper.GetHost(url);
                var collectPagesInfoDictionary = new ConcurrentDictionary<string, SiteInfoModel>();

                await CollectPageInfo(collectPagesInfoDictionary, host, CorrectUrl(url));

                return collectPagesInfoDictionary.Select(x => x.Value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }          
        }

        private string CorrectUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            string lastSymb;
            if (url.Length > 1)
            {
                lastSymb = url.Substring(url.Length - 2, 2);

                if (lastSymb == "/#")
                {
                    url = url.Remove(url.Length - 2);
                    return url;
                }
            }

            lastSymb = url.Substring(url.Length - 1, 1);
            if (lastSymb == "/" || lastSymb == "#")
            {
                url = url.Remove(url.Length - 1);
                return url;
            }

            return url;
        }
        
        private async Task CollectPageInfo(ConcurrentDictionary<string, SiteInfoModel> collectPagesInfoDictionary, string host, string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("Url is null");
            }
                
            if (collectPagesInfoDictionary.ContainsKey(url) || collectPagesInfoDictionary.Count == MaxPageTestTime)
            {
                return;
            }
            
            var regexLink = new Regex
                ("<a[^>]*? href=\"(?<url>[^\"]+)\"[^>]*?>(?<text>.*?)</a>",
                 RegexOptions.Singleline | RegexOptions.IgnoreCase);

            var stopwatch = new Stopwatch();
            MatchCollection matches = null;

            var pagesInfo = new List<string>();
          
            try
            {
                stopwatch.Start();

                using (var client = new HttpClient())
                using (var response = await client.GetAsync(url))
                using (HttpContent content = response.Content)
                {
                    stopwatch.Stop();

                    if (!response.IsSuccessStatusCode)
                    {
                        return;
                    }

                    matches = regexLink.Matches(await content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            bool isAdded = collectPagesInfoDictionary
                .TryAdd(url, new SiteInfoModel
                    {
                        Url = url,
                        ResponseTime = (int)stopwatch.ElapsedMilliseconds,
                        Date = DateTime.Now
                    });

            if (!isAdded || matches == null || matches.Count == 0)
            {
                return;
            }

            foreach (Match match in matches)
            {
                string link = match.Groups[1].Value;

                if (string.IsNullOrWhiteSpace(link))
                {
                    continue;
                }

                if (!link.StartsWith("http") && !link.StartsWith("www") && !link.StartsWith(host))
                {
                    if (!link.StartsWith("/"))
                    {
                        link = "/" + link;
                    }
                    link = host + link;
                }

                if (link.StartsWith(host))
                {
                    link = CorrectUrl(link);

                    if (!collectPagesInfoDictionary.ContainsKey(link) && !pagesInfo.Contains(link))
                    {
                        pagesInfo.Add(link);
                    }
                }
            }

            if (!pagesInfo.Any())
            {
                return;
            }

            int length = pagesInfo.Count;
            if (pagesInfo.Count + collectPagesInfoDictionary.Count > MaxPageTestTime)
            {
                length = MaxPageTestTime - collectPagesInfoDictionary.Count;
            }

            for (int i = 0; i < length; i++)
            {
                if (collectPagesInfoDictionary.Count == 10)
                {
                    break;
                }

                Thread.Sleep(100);
                await CollectPageInfo(collectPagesInfoDictionary, host, pagesInfo[i]);
            }
        }
    }
}