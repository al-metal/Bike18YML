using Bike18;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Bike18YML
{
    public partial class Form1 : Form
    {
        nethouse nethouse = new nethouse();
        httpRequest request = new httpRequest();

        XmlTextWriter textWritter = new XmlTextWriter("1.xml", Encoding.UTF8);
        

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            tbLogin.Text = Properties.Settings.Default.loginBike18.ToString();
            tbPassword.Text = Properties.Settings.Default.passwordBike18.ToString();
        }


        
    private void btnStart_Click(object sender, EventArgs e)
        {
            #region pass/login
            Properties.Settings.Default.loginBike18 = tbLogin.Text;
            Properties.Settings.Default.passwordBike18= tbPassword.Text;
            Properties.Settings.Default.Save();
            #endregion

            textWritter.WriteStartElement("head");
            textWritter.WriteEndElement();
            textWritter.Close();

            XmlDocument document = new XmlDocument();
            document.Load("1.xml");

            string otv = "";

            CookieContainer cookie = nethouse.CookieNethouse(tbLogin.Text, tbPassword.Text);

            if(cookie.Count != 4)
            {
                MessageBox.Show("Введены не верные пароли!");
                return;
            }

            otv = request.getRequest("https://bike18.ru/");
            MatchCollection razdel = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*(?=\" class=\"blue\">)").Matches(otv);
            MatchCollection categoryId = new Regex("(?<=id=\"item).*?(?=\">)").Matches(otv);

            if(razdel.Count != categoryId.Count)
            {
                MessageBox.Show("Какой то косяк в разделах на главной странице");
                return;
            }

            for (int r = 0; razdel.Count > r; r++)
            {
                string urlRazdel = razdel[r].ToString();
                otv = request.getRequest("https://bike18.ru/products/category/" + categoryId[r].ToString() + "/page/all");
                MatchCollection razdel2 = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*(?=\" class=\"blue\">)").Matches(otv);
                MatchCollection tovar2 = new Regex("(?<=-text-center\"><a href=\").*?(?=\" >)").Matches(otv);
                if(tovar2.Count != 0)
                {
                    Tovar(cookie, tovar2, document);
                }
                else if(razdel2.Count != 0)
                {

                }
                else
                {

                }
                document.Save("1.xml");

            }

        }

        private void Tovar(CookieContainer cookie, MatchCollection tovar, XmlDocument document)
        {
            for(int i = 0; tovar.Count > i; i++)
            {
                string otv = "";
                string urlTovar = tovar[i].ToString();
                
                List<string> listTovar = nethouse.GetProductList(cookie, urlTovar);
                string id = listTovar[0].ToString();
                string group = listTovar[3].ToString();
                if(group == "")
                    group = listTovar[46].ToString();

                HttpWebResponse res = null;
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://bike18.nethouse.ru/api/v1/catalog/attributes/" + group + "/" + group);
                req.Accept = "application/json, text/plain, */*";
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
                req.Method = "GET";
                req.CookieContainer = cookie;
                res = (HttpWebResponse)req.GetResponse();
                StreamReader ressr = new StreamReader(res.GetResponseStream());
                otv = ressr.ReadToEnd();
                res.Close();


                string available = "";
                if (listTovar[43].ToString() == "0")
                    available = "\"false\"";
                else
                    available = "\"true\"";
                string marketCategory = listTovar[45].ToString();
                string paramUnit = "";
                string paramName = "";
                string vendor = "";
                string url = urlTovar;
                string price = listTovar[9].ToString();
                string currencyId = "RUR";
                string categoryId = listTovar[2].ToString();
                string picture = listTovar[32].ToString();
                string name = listTovar[4].ToString();
                string description = EditDescription(listTovar[7].ToString());

                XmlNode element = document.CreateElement("offer");
                document.DocumentElement.AppendChild(element); // указываем родителя

                XmlAttribute attribute = document.CreateAttribute("available"); // создаём атрибут
                attribute.Value = available; // устанавливаем значение атрибута
                element.Attributes.Append(attribute); // добавляем атрибут

                attribute = document.CreateAttribute("id"); // создаём атрибут
                attribute.Value = id; // устанавливаем значение атрибута
                element.Attributes.Append(attribute); // добавляем атрибут

                XmlNode subElement;

                if(marketCategory != "")
                {
                    subElement = document.CreateElement("market_category"); // даём имя
                    subElement.InnerText = marketCategory; // и значение
                    element.AppendChild(subElement); // и указываем кому принадлежит
                }
                
                subElement = document.CreateElement("url"); // даём имя
                subElement.InnerText = url; // и значение
                element.AppendChild(subElement); // и указываем кому принадлежит

                subElement = document.CreateElement("price");
                subElement.InnerText = price;
                element.AppendChild(subElement);

                subElement = document.CreateElement("currencyId");
                subElement.InnerText = currencyId;
                element.AppendChild(subElement);

                subElement = document.CreateElement("categoryId");
                subElement.InnerText = categoryId;
                element.AppendChild(subElement);

                subElement = document.CreateElement("picture");
                subElement.InnerText = picture;
                element.AppendChild(subElement);

                subElement = document.CreateElement("name");
                subElement.InnerText = name;
                element.AppendChild(subElement);

                subElement = document.CreateElement("description");
                subElement.InnerText = description;
                element.AppendChild(subElement);

            }
        }

        private string EditDescription(string descript)
        {
            MatchCollection tags = new Regex("<.*?>").Matches(descript);
            foreach(Match ss in tags)
            {
                descript = descript.Replace(ss.ToString(), "");
            }
            descript.Trim();
            return descript;
        }
    }
}
