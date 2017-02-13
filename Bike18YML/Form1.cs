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
using System.Xml.Linq;

namespace Bike18YML
{
    public partial class Form1 : Form
    {
        int count = 0;
        nethouse nethouse = new nethouse();
        httpRequest request = new httpRequest();
        List<List<string>> allTovars = new List<List<string>>();

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

            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "no"));
            // создаем первый элемент
            //XElement yml_catalog = new XElement("yml_catalog");
            //XAttribute dateAtrb = new XAttribute("date", date);
            // создаем атрибут
            XAttribute iphoneNameAttr = new XAttribute("name", "iPhone 6");
            XElement iphoneCompanyElem = new XElement("company", "Apple");
            XElement iphonePriceElem = new XElement("price", "40000");
            // добавляем атрибут и элементы в первый элемент
            //yml_catalog.Add(iphoneNameAttr);
            //yml_catalog.Add(iphoneCompanyElem);
            //yml_catalog.Add(iphonePriceElem);

            // создаем второй элемент
            XElement galaxys5 = new XElement("phone");
            XAttribute galaxysNameAttr = new XAttribute("name", "Samsung Galaxy S5");
            XElement galaxysCompanyElem = new XElement("company", "Samsung");
            XElement galaxysPriceElem = new XElement("price", "33000");
            galaxys5.Add(galaxysNameAttr);
            galaxys5.Add(galaxysCompanyElem);
            galaxys5.Add(galaxysPriceElem);
            // создаем корневой элемент
            XElement yml_catalog = new XElement("yml_catalog");
            XElement shop = new XElement("shop");
            XElement name = new XElement("name", "BIKE18.RU");
            XElement company = new XElement("company", "BIKE18.RU");
            XElement url = new XElement("url", "https://bike18.ru");
            XElement currencies = new XElement("currencies");
            XElement currencieRate = new XElement("currencie");

            XAttribute dateAtrb = new XAttribute("date", date);
            yml_catalog.Add(dateAtrb);

            XAttribute currencieAtrb = new XAttribute("rate", "1");
            XAttribute idCurrencieAtrb = new XAttribute("id", "RUR");
            currencieRate.Add(currencieAtrb);
            currencieRate.Add(idCurrencieAtrb);


            yml_catalog.Add(shop);
            currencies.Add(currencieRate);

            shop.Add(name);
            shop.Add(company);
            shop.Add(url);
            shop.Add(currencies);
            // добавляем в корневой элемент
            //tehno.Add(yml_catalog);
            //tehno.Add(galaxys5);
            // добавляем корневой элемент в документ
            xdoc.Add(yml_catalog);


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
            for (int r = 0; razdel.Count > r; r++)
            {
                otv = request.getRequest("https://bike18.ru/products/category/" + categoryId[r].ToString() + "/page/all");
                MatchCollection razdel2 = new Regex("(?<=id=\"item).*?(?=\">)").Matches(otv);
                MatchCollection categoryId2 = new Regex("(?<=id=\"item).*?(?=\">)").Matches(otv);
                MatchCollection tovar2 = new Regex("(?<=-text-center\"><a href=\").*?(?=\" >)").Matches(otv);
                if (tovar2.Count != 0)
                    Tovar(cookie, tovar2);
                else
                {
                    for (int i = 0; razdel2.Count > i; i++)
                    {
                        otv = request.getRequest("https://bike18.ru/products/category/" + razdel2[i].ToString() + "/page/all");
                        MatchCollection categoryId3 = new Regex("(?<=id=\"item).*?(?=\">)").Matches(otv);
                        MatchCollection razdel3 = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*(?=\" class=\"blue\">)").Matches(otv);
                        MatchCollection tovar3 = new Regex("(?<=-text-center\"><a href=\").*?(?=\" >)").Matches(otv);
                        if (tovar3.Count != 0)
                            Tovar(cookie, tovar3);
                        else
                        {
                            for (int i3 = 0; razdel3.Count > i3; i3++)
                            {
                                otv = request.getRequest("https://bike18.ru/products/category/" + categoryId3[i3].ToString() + "/page/all");
                                MatchCollection categoryId4 = new Regex("(?<=id=\"item).*?(?=\">)").Matches(otv);
                                MatchCollection razdel4 = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*(?=\" class=\"blue\">)").Matches(otv);
                                MatchCollection tovar4 = new Regex("(?<=-text-center\"><a href=\").*?(?=\" >)").Matches(otv);
                                if (tovar4.Count != 0)
                                    Tovar(cookie, tovar4);
                                else
                                {
                                    for (int i4 = 0; razdel4.Count > i4; i4++)
                                    {
                                        otv = request.getRequest("https://bike18.ru/products/category/" + categoryId4[i4].ToString() + "/page/all");
                                        MatchCollection categoryId5 = new Regex("(?<=id=\"item).*?(?=\">)").Matches(otv);
                                        MatchCollection razdel5 = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*(?=\" class=\"blue\">)").Matches(otv);
                                        MatchCollection tovar5 = new Regex("(?<=-text-center\"><a href=\").*?(?=\" >)").Matches(otv);
                                        if (tovar5.Count != 0)
                                            Tovar(cookie, tovar5);
                                        else
                                        {
                                            for (int i5 = 0; razdel5.Count > i5; i5++)
                                            {
                                                otv = request.getRequest("https://bike18.ru/products/category/" + categoryId5[i5].ToString() + "/page/all");
                                                MatchCollection categoryId6 = new Regex("(?<=id=\"item).*?(?=\">)").Matches(otv);
                                                MatchCollection razdel6 = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*(?=\" class=\"blue\">)").Matches(otv);
                                                MatchCollection tovar6 = new Regex("(?<=-text-center\"><a href=\").*?(?=\" >)").Matches(otv);
                                                if (tovar6.Count != 0)
                                                    Tovar(cookie, tovar6);
                                                else
                                                {
                                                    for (int i6 = 0; razdel6.Count > i6; i6++)
                                                    {
                                                        otv = request.getRequest("https://bike18.ru/products/category/" + categoryId6[i6].ToString() + "/page/all");
                                                        MatchCollection categoryId7 = new Regex("(?<=id=\"item).*?(?=\">)").Matches(otv);
                                                        MatchCollection razdel7 = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*(?=\" class=\"blue\">)").Matches(otv);
                                                        MatchCollection tovar7 = new Regex("(?<=-text-center\"><a href=\").*?(?=\" >)").Matches(otv);
                                                        if (tovar7.Count != 0)
                                                            Tovar(cookie, tovar7);
                                                        else
                                                        {

                                                        }
                                                    }

                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            //сохраняем документ
            xdoc.Save("1.xml");
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

        private void Tovar(CookieContainer cookie, MatchCollection tovar)
        {
            for (int i = 0; tovar.Count > i; i++)
            {
                string otv = "";
                List<string> tovarAtribute = new List<string>();
                string urlTovar = tovar[i].ToString();
                //urlTovar = "https://bike18.ru/products/minitraktor-dt-15-plug-i-pochvofreza";

                List<string> listTovar = nethouse.GetProductList(cookie, urlTovar);
                if (listTovar.Count != 0)
                {
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



                    //XmlNode element = document.CreateElement("offer");
                    //document.DocumentElement.AppendChild(element); // указываем родителя
                    tovarAtribute.Add("offer;id" + "-" + id + ";available" + "-" + available);
                    if (marketCategory != "")
                        tovarAtribute.Add("market_category-" + marketCategory);
                    tovarAtribute.Add("url-" + url);
                    tovarAtribute.Add("price-" + price);
                    tovarAtribute.Add("currencyId-" + currencyId);
                    tovarAtribute.Add("categoryId-" + categoryId);
                    tovarAtribute.Add("picture-" + picture);
                    tovarAtribute.Add("name-" + name);
                    tovarAtribute.Add("description-" + description);
                    if (vendor != "")
                        tovarAtribute.Add("vendor-" + vendor);


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
                                    tovarAtribute.Add("param;" + "unit-" + unitParamProduct + ";name-" + namesParamProduct + ";" + valueParamProduct);
                                    //    XmlElement xE = document.CreateElement("param");
                                    //xE.SetAttribute("name", namesParamProduct);
                                    //xE.SetAttribute("unit", unitParamProduct);
                                    //xE.InnerText = valueParamProduct;
                                    //element.AppendChild(xE);
                                }
                                else
                                {
                                    tovarAtribute.Add("param;" + "name-" + namesParamProduct + ";" + valueParamProduct);
                                    //    XmlElement xE = document.CreateElement("param");
                                    //xE.SetAttribute("name", namesParamProduct);
                                    //xE.InnerText = valueParamProduct;
                                    //element.AppendChild(xE);
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
                allTovars.Add(tovarAtribute);
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
