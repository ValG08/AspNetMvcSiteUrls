using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UrlsProject.Helpers
{
    public class Helper
    {
        public static async Task<bool> ExistUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url) || url.Contains(" "))
            {
                return false;
            }
            
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                return false;
            }

            try
            {
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage()
                        { RequestUri = new Uri(url), Method = HttpMethod.Head })
                {
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        return response.IsSuccessStatusCode;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string RemoveAfter(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            string[] matches = { ".","," };

            for (int i = 0; i < matches.Length; i++)
            {            
                while (true)
                {
                    int lastIndex = url.LastIndexOf(matches[i]);
                    if (lastIndex > 0 && lastIndex == url.Length - 1)
                    {
                        url = url.Remove(lastIndex);
                        i = 0;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return url;
        }

        public static string GetHost(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("Url is null");
            }

            try
            {
                Uri uri = new Uri(url);
                return uri.Scheme + "://" + uri.Host;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
