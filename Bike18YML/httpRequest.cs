﻿using System.IO;
using System.Net;
using System.Text;

namespace Bike18
{
    class httpRequest
    {
        public string getRequestEncod(string url)
        {
            HttpWebResponse res = null;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Proxy = null;
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            res = (HttpWebResponse)req.GetResponse();
            StreamReader ressr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding(1251));
            string otv = ressr.ReadToEnd();
            res.Close();
            return otv;
        }

        public string getRequest(string url)
        {
            HttpWebResponse res = null;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            res = (HttpWebResponse)req.GetResponse();
            StreamReader ressr = new StreamReader(res.GetResponseStream());
            string otv = ressr.ReadToEnd();
            res.GetResponseStream().Close();
            req.GetResponse().Close();

            return otv;
        }

        public string getRequest(CookieContainer cookie, string url)
        {
            HttpWebResponse res = null;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Proxy = null;
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.CookieContainer = cookie;
            res = (HttpWebResponse)req.GetResponse();
            StreamReader ressr = new StreamReader(res.GetResponseStream());
            string otv = ressr.ReadToEnd();
            res.GetResponseStream().Close();
            req.GetResponse().Close();
            res.Close();
            return otv;
        }

        public CookieContainer webCookie(string url)
        {
            CookieContainer cooc = new CookieContainer();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cooc;
            Stream stre = req.GetRequestStream();
            stre.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            return cooc;
        }

        public string PostRequest(CookieContainer cookie, string nethouseTovar)
        {
            string otv = null;
            try
            {
                HttpWebResponse res = null;
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(nethouseTovar);
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
                req.Method = "POST";
                //req.Proxy = null;
                req.ContentType = "application/x-www-form-urlencoded";
                req.CookieContainer = cookie;
                res = (HttpWebResponse)req.GetResponse();
                StreamReader ressr = new StreamReader(res.GetResponseStream());
                otv = ressr.ReadToEnd();
                res.Close();
            }
            catch
            {
                otv = "err";
            }
            

            return otv;
        }
        
    }
}
