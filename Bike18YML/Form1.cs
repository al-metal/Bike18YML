using Bike18;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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
        int count = 0;
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
            Properties.Settings.Default.passwordBike18 = tbPassword.Text;
            Properties.Settings.Default.Save();
            #endregion

            DateTime thisDate = DateTime.Now;
            string date = thisDate.ToString(thisDate.ToString("yyyy-mm-dd H:mm"));

            textWritter.WriteStartElement("yml_catalog");
            textWritter.WriteAttributeString("date", date);
            textWritter.WriteStartElement("shop");
            textWritter.WriteEndElement();
            textWritter.Close();           

            XmlDocument document = new XmlDocument();
            document.Load("1.xml");

            XmlNode element = document.CreateElement("name");
            element.InnerText = "BIKE18.RU";

            element = document.CreateElement("offer");
            document.DocumentElement.AppendChild(element); // указываем родителя

            XmlAttribute attribute = document.CreateAttribute("available"); // создаём атрибут
            attribute.Value = "true"; // устанавливаем значение атрибута
            element.Attributes.Append(attribute); // добавляем атрибут

            string otv = "";

            CookieContainer cookie = nethouse.CookieNethouse(tbLogin.Text, tbPassword.Text);

            if (cookie.Count != 4)
            {
                MessageBox.Show("Введены не верные пароли!");
                return;
            }

            otv = request.getRequest("https://bike18.ru/");
            MatchCollection razdel = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*(?=\" class=\"blue\">)").Matches(otv);
            MatchCollection categoryId = new Regex("(?<=id=\"item).*?(?=\">)").Matches(otv);

            if (razdel.Count != categoryId.Count)
            {
                MessageBox.Show("Какой то косяк в разделах на главной странице");
                return;
            }

            #region Весь сайт
            //for (int r = 0; razdel.Count > r; r++)
            //{
            //    MatchCollection razdel2 = new Regex("(?<=id=\"item).*?(?=\">)").Matches(otv);
            //    MatchCollection tovar2 = new Regex("(?<=-text-center\"><a href=\").*?(?=\" >)").Matches(otv);
            //    if (tovar2.Count != 0)
            //    {
            //        Tovar(cookie, tovar2, document);
            //    }
            //    else if (razdel2.Count != 0)
            //    {
            //        for(int i = 0; razdel2.Count > i; i++)
            //        {
            //            otv = request.getRequest("https://bike18.ru/products/category/" + razdel2[i].ToString() + "/page/all");
            //            MatchCollection categoryId3 = new Regex("(?<=id=\"item).*?(?=\">)").Matches(otv);
            //            MatchCollection razdel3 = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*(?=\" class=\"blue\">)").Matches(otv);
            //            MatchCollection tovar3 = new Regex("(?<=-text-center\"><a href=\").*?(?=\" >)").Matches(otv);
            //            if (razdel3.Count != 0 && categoryId3.Count == razdel3.Count)
            //            {
            //                for (int i3 = 0; razdel3.Count > i3; i3++)
            //                {
            //                    otv = request.getRequest("https://bike18.ru/products/category/" + categoryId3[i3].ToString() + "/page/all");
            //                    MatchCollection categoryId4 = new Regex("(?<=id=\"item).*?(?=\">)").Matches(otv);
            //                    MatchCollection razdel4 = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*(?=\" class=\"blue\">)").Matches(otv);
            //                    MatchCollection tovar4 = new Regex("(?<=-text-center\"><a href=\").*?(?=\" >)").Matches(otv);
            //                    if (razdel4.Count != 0 && categoryId4.Count == razdel4.Count)
            //                    {
            //                        for (int i4 = 0; razdel4.Count > i4; i4++)
            //                        {
            //                            otv = request.getRequest("https://bike18.ru/products/category/" + categoryId4[i4].ToString() + "/page/all");
            //                            MatchCollection categoryId5 = new Regex("(?<=id=\"item).*?(?=\">)").Matches(otv);
            //                            MatchCollection razdel5 = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*(?=\" class=\"blue\">)").Matches(otv);
            //                            MatchCollection tovar5 = new Regex("(?<=-text-center\"><a href=\").*?(?=\" >)").Matches(otv);
            //                            if (razdel5.Count != 0 && categoryId5.Count == razdel5.Count)
            //                            {
            //                                for (int i5 = 0; razdel5.Count > i5; i5++)
            //                                {
            //                                    otv = request.getRequest("https://bike18.ru/products/category/" + categoryId5[i5].ToString() + "/page/all");
            //                                    MatchCollection categoryId6 = new Regex("(?<=id=\"item).*?(?=\">)").Matches(otv);
            //                                    MatchCollection razdel6 = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*(?=\" class=\"blue\">)").Matches(otv);
            //                                    MatchCollection tovar6 = new Regex("(?<=-text-center\"><a href=\").*?(?=\" >)").Matches(otv);
            //                                    if (razdel5.Count != 0 && categoryId6.Count == razdel6.Count)
            //                                    {
            //                                        for (int i6 = 0; razdel6.Count > i6; i6++)
            //                                        {
            //                                            otv = request.getRequest("https://bike18.ru/products/category/" + categoryId6[i6].ToString() + "/page/all");
            //                                            MatchCollection categoryId7 = new Regex("(?<=id=\"item).*?(?=\">)").Matches(otv);
            //                                            MatchCollection razdel7 = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*(?=\" class=\"blue\">)").Matches(otv);
            //                                            MatchCollection tovar7 = new Regex("(?<=-text-center\"><a href=\").*?(?=\" >)").Matches(otv);
            //                                            if (razdel7.Count != 0 && categoryId7.Count == razdel7.Count)
            //                                            {

            //                                            }
            //                                            else
            //                                            {
            //                                                Tovar(cookie, tovar7, document);
            //                                            }
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        Tovar(cookie, tovar6, document);
            //                                    }
            //                                }
            //                            }
            //                            else
            //                            {
            //                                Tovar(cookie, tovar5, document);
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        Tovar(cookie, tovar4, document);
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                Tovar(cookie, tovar3, document);
            //            }
            //        }
            //    }
            //    else
            //    {

            //    }
            //}
            #endregion
            document.Save("1.xml");
            MessageBox.Show(count.ToString());
        }

        private string ReturnUrlAllTovar(string url)
        {
            string urlAllTovar = "";
            string otv = request.getRequest(url);
            MatchCollection categoryIdCollection = new Regex("(?<=categoryId\" value=\").*?(?=\">)").Matches(otv);
            string categoryId = categoryIdCollection[1].ToString();
            urlAllTovar = "https://bike18.ru/products/category/" + categoryId + "/page/all";
            return urlAllTovar;
        }

        private void Tovar(CookieContainer cookie, MatchCollection tovar, XmlDocument document)
        {
            for (int i = 0; tovar.Count > i; i++)
            {
                string otv = "";
                string urlTovar = tovar[i].ToString();

                List<string> listTovar = nethouse.GetProductList(cookie, urlTovar);
                if(listTovar.Count != 0) { 
                string id = listTovar[0].ToString();
                string group = listTovar[3].ToString();

                string marketCategory = listTovar[45].ToString();
                string paramName = "";
                string paramValue = "";
                string unit = "";
                string vendor = "";
                string url = urlTovar;
                string price = listTovar[9].ToString();
                string currencyId = "RUR";
                string categoryId = listTovar[2].ToString();
                string picture = listTovar[32].ToString();
                string name = listTovar[4].ToString();
                string description = EditDescription(listTovar[7].ToString());
                string available = "";
                if (listTovar[43].ToString() == "0")
                    available = "\"false\"";
                else
                    available = "\"true\"";

                string urlTovarAttrib = "";

                List<string> param = new List<string>();
                if (group != "0")
                {
                    if (group == "")
                    {
                        group = listTovar[46].ToString();
                        urlTovarAttrib = "https://bike18.nethouse.ru/api/v1/catalog/attributes/" + group + "/" + group;
                    }
                    else if (group == "0")
                    {
                        group = listTovar[46].ToString();
                        urlTovarAttrib = "https://bike18.nethouse.ru/api/v1/catalog/attributes/" + "0" + "/" + group;
                    }
                    else
                        urlTovarAttrib = "https://bike18.nethouse.ru/api/v1/catalog/attributes/" + group + "/" + group;

                    HttpWebResponse res = null;
                    HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(urlTovarAttrib);
                    req.Accept = "application/json, text/plain, */*";
                    req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
                    req.Method = "GET";
                    req.CookieContainer = cookie;
                    res = (HttpWebResponse)req.GetResponse();
                    StreamReader ressr = new StreamReader(res.GetResponseStream());
                    otv = ressr.ReadToEnd();
                    res.Close();



                    dynamic stuff1 = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(otv);
                    string ssss = stuff1.ToString();


                    string primaryKey = "";
                    string attributeId = "";
                    string empty = "";
                    string valueId = "";
                    string valueText = "";
                    string chekbox = "";

                    MatchCollection attributes = new Regex("primaryKey[\\w\\W]*?(?=primaryKey)").Matches(ssss);
                    string atributeTovar = listTovar[40].ToString() + "primaryKey";
                    MatchCollection atributesTovar = new Regex("primaryKey.*?(?=primaryKey)").Matches(atributeTovar);
                    ///////////////////////


                    ///////////////////////////

                    foreach (Match s in atributesTovar)
                    {
                        string strTovar = s.ToString() + "&";
                        if (strTovar.Contains("4113"))
                        {

                        }
                        primaryKey = new Regex("(?<=primaryKey]=).*?(?=&)").Match(strTovar).ToString();
                        attributeId = new Regex("(?<=attributeId]=).*?(?=&)").Match(strTovar).ToString();
                        empty = new Regex("(?<=empty]=).*?(?=&)").Match(strTovar).ToString();
                        valueId = new Regex("(?<=valueId]=).*?(?=&)").Match(strTovar).ToString();
                        chekbox = new Regex("(?<=checkbox]=)[\\w\\W]*?(?=&)").Match(strTovar).ToString();


                        if (chekbox != "")
                        {
                            if (chekbox == "1")
                                paramValue = "есть";
                        }
                        else
                            paramValue = new Regex("(?<=text]=)[\\w\\W]*?(?=&)").Match(strTovar).ToString();

                        foreach (Match ss in attributes)
                        {
                            string str = ss.ToString();
                            paramName = new Regex("(?<=name\": \")[\\w\\W]*?(?=\")").Match(str).ToString();
                            unit = new Regex("(?<=unit\": \")[\\w\\W]*?(?=\")").Match(str).ToString();
                            if (str.Contains(primaryKey))
                            {
                                bool b = false;
                                if (str.Contains("vendor"))
                                {
                                    b = true;
                                }
                                if (str.Contains("options"))
                                {
                                    MatchCollection options = new Regex("valueId\": [\\w\\W]*?(?=})").Matches(str);
                                    if (options.Count == 0)
                                    {
                                        if (unit == "")
                                            param.Add(paramName + ";" + paramValue);
                                        else
                                            param.Add(paramName + ";" + paramValue + ";" + unit);
                                    }
                                    else
                                    {
                                        foreach (Match sss in options)
                                        {
                                            string str2 = sss.ToString();
                                            if (str2.Contains(valueId))
                                            {
                                                if (b)
                                                {
                                                    valueText = new Regex("(?<=valueText\": \")[\\w\\W]*?(?=\")").Match(str2).ToString();
                                                    vendor = valueText;
                                                    param.Add("vendor;" + valueText);
                                                }
                                                else
                                                {
                                                    valueText = new Regex("(?<=valueText\": \")[\\w\\W]*?(?=\")").Match(str2).ToString();
                                                    param.Add(paramName + ";" + valueText);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }



                XmlNode element = document.CreateElement("offer");
                document.DocumentElement.AppendChild(element); // указываем родителя

                XmlAttribute attribute = document.CreateAttribute("available"); // создаём атрибут
                attribute.Value = available; // устанавливаем значение атрибута
                element.Attributes.Append(attribute); // добавляем атрибут

                attribute = document.CreateAttribute("id"); // создаём атрибут
                attribute.Value = id; // устанавливаем значение атрибута
                element.Attributes.Append(attribute); // добавляем атрибут

                XmlNode subElement;

                if (marketCategory != "")
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

                if (vendor != "")
                {
                    subElement = document.CreateElement("vendor");
                    subElement.InnerText = vendor;
                    element.AppendChild(subElement);
                }
                if (param.Count > 0)
                {

                    foreach (string s in param)
                    {
                        string str = s;
                        if (!str.Contains("vendor"))
                        {
                            string[] strParamProduct = str.Split(';');
                            string namesParamProduct = strParamProduct[0];
                            string valueParamProduct = strParamProduct[1];
                            string unitParamProduct = "";
                            if (strParamProduct.Length > 2)
                                unitParamProduct = strParamProduct[2];

                            if (valueParamProduct == "")
                            {

                            }

                            if (unitParamProduct != "")
                            {
                                XmlElement xE = document.CreateElement("param");
                                xE.SetAttribute("name", namesParamProduct);
                                xE.SetAttribute("unit", unitParamProduct);
                                xE.InnerText = valueParamProduct;
                                element.AppendChild(xE);
                            }
                            else
                            {
                                XmlElement xE = document.CreateElement("param");
                                xE.SetAttribute("name", namesParamProduct);
                                xE.InnerText = valueParamProduct;
                                element.AppendChild(xE);
                            }



                        }
                    }

                    //XmlAttribute attribute = document.CreateAttribute("available"); // создаём атрибут
                    //attribute.Value = available; // устанавливаем значение атрибута
                    //element.Attributes.Append(attribute); // добавляем атрибут

                    //subElement = document.CreateElement("param");
                    //subElement.InnerText = vendor;
                    //element.AppendChild(subElement);

                    //attribute = subElement.Attributes.Append(document.CreateAttribute("name", paramName)); // создаём атрибут
                    //attribute.Value = paramValue; // устанавливаем значение атрибута
                    //element.Attributes.Append(attribute); // добавляем атрибут
                }
            }
                count++;
            }
            
        }

        private string EditDescription(string descript)
        {
            MatchCollection tags = new Regex("<.*?>").Matches(descript);
            foreach (Match ss in tags)
            {
                descript = descript.Replace(ss.ToString(), "");
            }
            descript.Trim();
            return descript;
        }
    }
}
