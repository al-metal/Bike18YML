﻿using Bike18;
using Newtonsoft.Json;
using OfficeOpenXml;
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
        List<XElement> categories = new List<XElement>();
        XElement categoriesElement = new XElement("categories");

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

            CookieContainer cookie = nethouse.CookieNethouse(tbLogin.Text, tbPassword.Text);

            if (cookie.Count != 4)
            {
                MessageBox.Show("Введены не верные пароли!");
                return;
            }

            FileInfo file = new FileInfo("catalog-14.02.2017_21-17-31.xlsx");
            ExcelPackage p = new ExcelPackage(file);

            ExcelWorksheet w = p.Workbook.Worksheets[1];
            int q = w.Dimension.Rows;
            pb.Maximum = q;
            for (int i = 2; q >= i; i++)
            {
                string idTovar = w.Cells[i, 1].Value.ToString();
                Tovar(cookie, idTovar);
                pb.Value = i;
            }

            CreateSaveYML(allTovars);

            MessageBox.Show("Добавлено товаров: " + count.ToString() + " из " + q);
        }

        private void CreateSaveYML(List<List<string>> allTovars)
        {
            List<XElement> offersTovars = new List<XElement>();
            DateTime thisDate = DateTime.Now;
            string date = thisDate.ToString(thisDate.ToString("yyyy-mm-dd H:mm"));

            XDeclaration xDeclaration = new XDeclaration("1.0", "UTF-8", "no");
            XDocument xdoc = new XDocument(new XDocumentType("yml_catalog", null, "shops.dtd", null));
            xdoc.Declaration = xDeclaration;
            XDocumentType doctype = new XDocumentType("yml_catalog", null, "shops.dtd", null);
            XElement yml_catalog = new XElement("yml_catalog");
            XElement shop = new XElement("shop");
            XElement categoriesElement = new XElement("categories");
            XElement name = new XElement("name", "BIKE18.RU");
            XElement company = new XElement("company", "BIKE18.RU");
            XElement url = new XElement("url", "https://bike18.ru");
            XElement currencies = new XElement("currencies");
            XElement currencieRate = new XElement("currency");
            XElement offers = new XElement("offers");

            XAttribute dateAtrb = new XAttribute("date", date);
            yml_catalog.Add(dateAtrb);

            XAttribute currencieAtrb = new XAttribute("rate", "1");
            XAttribute idCurrencieAtrb = new XAttribute("id", "RUR");

            currencieRate.Add(idCurrencieAtrb);
            currencieRate.Add(currencieAtrb);

            yml_catalog.Add(shop);
            currencies.Add(currencieRate);
            for (int i = 0; allTovars.Count > i; i++)
            {
                List<string> tovar = allTovars[i];
                XElement offer = new XElement("param", i);
                XAttribute idProd = null;
                XAttribute available = null;
                XElement urlTovar = null;
                XElement market_category = null;
                XElement priceTovar = null;
                XElement currencyIdTovar = null;
                XElement categoryIdTovar = null;
                XElement pictureTovar = null;
                XElement nameTovar = null;
                XElement descriptionTovar = null;
                XElement vendorTovar = null;
                XElement param = new XElement("param", i);
                XAttribute paramName = null;
                XAttribute paramUnit = null;
                List<XElement> paramList2 = new List<XElement>();
                for (int t = 0; tovar.Count > t; t++)
                {
                    string str = tovar[t].ToString();
                    string[] arrayStr = str.Split(';');
                    if (arrayStr.Length == 1 || str.Contains("description-"))
                    {
                        string[] category = arrayStr[0].ToString().Split('-');
                        if (category[0].ToString().Contains("market_category"))
                        {
                            market_category = new XElement("market_category", category[1].ToString());
                        }
                        else if (category[0].ToString().Contains("url"))
                        {
                            urlTovar = new XElement("url", tovar[2].ToString().Replace("url-", ""));
                        }
                        else if (category[0].ToString().Contains("price"))
                        {
                            priceTovar = new XElement("price", category[1].ToString());
                        }
                        else if (category[0].ToString().Contains("currencyId"))
                        {
                            currencyIdTovar = new XElement("currencyId", category[1].ToString());
                        }
                        else if (category[0].ToString().Contains("categoryId"))
                        {
                            categoryIdTovar = new XElement("categoryId", category[1].ToString());
                        }
                        else if (category[0].ToString().Contains("picture"))
                        {
                            pictureTovar = new XElement("picture", category[1].ToString());
                        }
                        else if (category[0].ToString().Contains("name"))
                        {
                            nameTovar = new XElement("name", tovar[7].ToString().Replace("name-", ""));
                        }
                        else if (category[0].ToString().Contains("description"))
                        {
                            str = str.Replace("description-", "");
                            descriptionTovar = new XElement("description", str);
                        }
                        else if (category[0].ToString().Contains("vendor"))
                        {
                            vendorTovar = new XElement("vendor", category[1].ToString());
                        }
                    }
                    else
                    {
                        if (arrayStr[0].ToString().Contains("offer"))
                        {
                            offer = new XElement("offer");
                            string[] arrayID = arrayStr[1].ToString().Split('-');
                            string[] arrayAvailable = arrayStr[2].ToString().Split('-');

                            idProd = new XAttribute("id", arrayID[1].ToString());
                            available = new XAttribute("available", arrayAvailable[1].ToString().Replace("\"", ""));
                            offer.Add(idProd);
                            offer.Add(available);
                        }
                        else if (arrayStr[0].ToString().Contains("param"))
                        {
                            string[] arrayParam = arrayStr[1].ToString().Split('-');
                            if (arrayStr.Length == 3)
                            {
                                param = new XElement("param", arrayStr[2].ToString());
                                paramName = new XAttribute("name", arrayParam[1].ToString());
                                param.Add(paramName);
                                paramList2.Add(param);
                            }
                            else
                            {
                                string[] arrayUnit = arrayStr[2].ToString().Split('-');
                                param = new XElement("param", arrayStr[3].ToString());
                                paramName = new XAttribute("name", arrayParam[1].ToString());
                                paramUnit = new XAttribute("unit", arrayUnit[1].ToString());
                                param.Add(paramUnit);
                                param.Add(paramName);
                                paramList2.Add(param);
                            }
                        }
                    }
                }
                offer.Add(market_category);
                offer.Add(urlTovar);
                offer.Add(priceTovar);
                offer.Add(currencyIdTovar);
                offer.Add(categoryIdTovar);
                offer.Add(pictureTovar);
                offer.Add(nameTovar);
                offer.Add(descriptionTovar);
                if (vendorTovar != null)
                    offer.Add(vendorTovar);
                if (paramList2.Count != 0)
                {
                    foreach (XElement element in paramList2)
                    {
                        offer.Add(element);
                    }
                }
                offersTovars.Add(offer);
                count++;
            }

            foreach (XElement element in offersTovars)
            {
                offers.Add(element);
            }

            shop.Add(name);
            shop.Add(company);
            shop.Add(url);
            shop.Add(currencies);
            foreach (XElement element in categories)
            {
                categoriesElement.Add(element);
            }
            shop.Add(categoriesElement);
            shop.Add(offers);

            xdoc.Add(yml_catalog);

            //сохраняем документ
            xdoc.Save("bike18.xml");
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

        private void Tovar(CookieContainer cookie, string idTovar)
        {
            string otv = "";
            List<string> tovarAtribute = new List<string>();
            string urlTovar = "https://bike18.ru/products/" + idTovar;

            List<string> listTovar = nethouse.GetProductList(cookie, urlTovar);
            if (listTovar.Count != 0)
            {
                string id = listTovar[0].ToString();
                string group = listTovar[3].ToString();
                if(listTovar[1].ToString() != "")
                    urlTovar = "https://bike18.ru/products/" + listTovar[1].ToString();
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
                    if (categories.Count == 0)
                    {
                        ReturnCategories(cookie);
                    }

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
                            if (unit == "\\")
                                unit = "\"";
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
                            }
                            else
                            {
                                tovarAtribute.Add("param;" + "name-" + namesParamProduct + ";" + valueParamProduct);
                            }
                        }
                    }
                }
                allTovars.Add(tovarAtribute);
            }
        }

        private void ReturnCategories(CookieContainer cookie)
        {
            List<string> counts = new List<string>();
            string otv = "";
            HttpWebResponse res = null;
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://bike18.nethouse.ru/api/catalog/categoriesselect");
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
            List<string> arrayCategorie = new List<string>();
            MatchCollection arrayCategories = new Regex("(?<={)[\\w\\W]*?(?=\"desc\": )").Matches(ssss);
            foreach (Match str in arrayCategories)
            {
                string id = null;
                string categoryId = null;
                string name = null;
                if (str.ToString().Contains("categoryId"))
                {
                    StringReader strReader = new StringReader(str.ToString());
                    string[] strCategor = null;

                    File.WriteAllText("category", str.ToString(), Encoding.GetEncoding(1251));
                    strCategor = File.ReadAllLines("category", Encoding.GetEncoding(1251));
                    File.Delete("category");

                    for (int i = 0; strCategor.Length > i; i++)
                    {
                        string strCategoryId = strCategor[i];
                        if (strCategoryId.Contains("categoryId"))
                        {
                            id = new Regex("(?<=id\": ).*?(?=,)").Match(strCategor[i - 2]).ToString().Trim();
                            categoryId = new Regex("(?<=:).*?(?=,)").Match(strCategoryId).ToString().Trim();
                            name = new Regex("(?<=name\": \").*?(?=\")").Match(strCategor[i + 1]).ToString().Trim();

                            if (categoryId == "0")
                            {
                                categoriesElement = new XElement("category", name);
                                XAttribute atribId = new XAttribute("id", id);
                                categoriesElement.Add(atribId);
                                categories.Add(categoriesElement);
                            }
                            else
                            {
                                categoriesElement = new XElement("category", name);
                                XAttribute atribCategoryId = new XAttribute("parentId", categoryId);
                                XAttribute atribId = new XAttribute("id", id);
                                categoriesElement.Add(atribId);
                                categoriesElement.Add(atribCategoryId);
                                categories.Add(categoriesElement);
                            }
                        }
                    }
                }
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
