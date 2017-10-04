using NehouseLibrary;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using xNet.Net;

namespace Bike18YML
{
    public partial class Form1 : Form
    {
        Thread forms;

        int count = 0;
        int countPosition = 0;
        nethouse nethouse = new nethouse();
        List<List<string>> allTovars = new List<List<string>>();
        List<XElement> categories = new List<XElement>();
        XElement categoriesElement = new XElement("categories");
        CookieDictionary cookie;

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

            cookie = nethouse.CookieNethouse(tbLogin.Text, tbPassword.Text);

            if (cookie.Count != 4)
            {
                MessageBox.Show("Введены не верные пароли!");
                return;
            }

            Thread tabl = new Thread(() => CreateYML());
            forms = tabl;
            forms.IsBackground = true;
            forms.Start();
        }

        private void CreateYML()
        {
            ControlsFormEnabledFalse();
            File.Delete("erorTovar");

            FileInfo file = new FileInfo("Прайс.xlsx");
            ExcelPackage p = new ExcelPackage(file);

            ExcelWorksheet w = p.Workbook.Worksheets[1];
            int q = w.Dimension.Rows;
            pb.Invoke(new Action(() => pb.Maximum = q));
            for (int i = 2; q >= i; i++)
            {
                CheckCountPosition(countPosition);

                List<string> tovar = new List<string>();
                string idTovar = w.Cells[i, 1].Value.ToString();

                tovar.Add(w.Cells[i, 1].Value.ToString());

                if (w.Cells[i, 2].Value != null)
                    tovar.Add(w.Cells[i, 2].Value.ToString());
                else
                    tovar.Add("");

                if (w.Cells[i, 3].Value != null)
                    tovar.Add(w.Cells[i, 3].Value.ToString());
                else
                    tovar.Add("");

                if (w.Cells[i, 4].Value != null)
                    tovar.Add(w.Cells[i, 4].Value.ToString());
                else
                    tovar.Add("");

                if (w.Cells[i, 5].Value != null)
                    tovar.Add(w.Cells[i, 5].Value.ToString());
                else
                    tovar.Add("");

                if (w.Cells[i, 6].Value != null)
                    tovar.Add(w.Cells[i, 6].Value.ToString());
                else
                    tovar.Add("");

                if (w.Cells[i, 7].Value != null)
                {
                    if (w.Cells[i, 7].Value.ToString() == "0")
                        continue;
                    else
                        tovar.Add(w.Cells[i, 7].Value.ToString());
                }
                else
                    tovar.Add("");

                if (w.Cells[i, 8].Value != null)
                    tovar.Add(w.Cells[i, 8].Value.ToString());
                else
                    tovar.Add("");

                if (w.Cells[i, 9].Value != null)
                    tovar.Add(w.Cells[i, 9].Value.ToString());
                else
                    tovar.Add("");

                if (w.Cells[i, 10].Value != null)
                    tovar.Add(w.Cells[i, 10].Value.ToString());
                else
                    tovar.Add("");

                if (w.Cells[i, 11].Value != null)
                    tovar.Add(w.Cells[i, 11].Value.ToString());
                else
                    tovar.Add("");

                if (w.Cells[i, 12].Value != null)
                    tovar.Add(w.Cells[i, 12].Value.ToString());
                else
                    tovar.Add("");

                if (w.Cells[i, 13].Value != null)
                    tovar.Add(w.Cells[i, 13].Value.ToString());
                else
                    tovar.Add("");

                if (w.Cells[i, 14].Value != null)
                    tovar.Add(w.Cells[i, 14].Value.ToString());
                else
                    tovar.Add("");

                if (w.Cells[i, 15].Value != null)
                    tovar.Add(w.Cells[i, 15].Value.ToString());
                else
                    tovar.Add("");
                //Tovar(cookie, idTovar);
                Tovar2(cookie, tovar);
                pb.Invoke(new Action(() => pb.Value = i));
                countPosition++;
            }

            CreateSaveYML(allTovars);

            ControlsFormEnabledFalse();

            MessageBox.Show("Добавлено товаров: " + count.ToString() + " из " + (q - 1));
        }

        private void CheckCountPosition(int countPosition)
        {
            if(countPosition >= 500)
            {
                Thread.Sleep(300000);
                countPosition = 0;
            }
        }

        private void CreateSaveYML(List<List<string>> allTovars)
        {

            List<XElement> offersTovars = new List<XElement>();
            DateTime thisDate = DateTime.Now;
            string date = thisDate.ToString(thisDate.ToString("yyyy-MM-dd H:mm"));

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
                List<string> images = new List<string>();
                List<string> tovar = allTovars[i];
                XElement offer = new XElement("param", i);
                XAttribute idProd = null;
                XAttribute available = null;
                XElement urlTovar = null;
                XElement market_category = null;
                XElement priceTovar = null;
                XElement oldPriceTovar = null;
                XElement currencyIdTovar = null;
                XElement categoryIdTovar = null;
                XElement pictureTovar = null;
                XElement nameTovar = null;
                XElement descriptionTovar = null;
                XElement vendorTovar = null;
                XElement param = new XElement("param", i);
                XAttribute paramName = null;

                List<XElement> paramList2 = new List<XElement>();
                for (int t = 0; tovar.Count > t; t++)
                {
                    string str = tovar[t].ToString();

                    string[] arrayStr = str.Split(';');
                    if (arrayStr.Length == 1 || str.Contains("description-") || str.Contains("picture-"))
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
                        else if (category[0].ToString().Contains("oldPrice"))
                        {
                            oldPriceTovar = new XElement("oldprice", category[1].ToString());
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
                            foreach (string s in arrayStr)
                            {
                                if (s.Contains("http"))
                                {
                                    string urlImg = s.Replace("\\/", "/").Replace("picture-", "");
                                    images.Add(urlImg);
                                }
                            }
                        }
                        else if (category[0].ToString().Contains("name"))
                        {

                            for (int x = 0; tovar.Count > x; x++)
                            {
                                if (tovar[x].ToString().Contains("name-") && !tovar[x].ToString().Contains("param"))
                                    nameTovar = new XElement("name", tovar[x].ToString().Replace("name-", ""));
                            }

                            /*if (oldPriceTovar == null)
                            {
                                nameTovar = new XElement("name", tovar[7].ToString().Replace("name-", ""));
                            }
                            else
                                nameTovar = new XElement("name", tovar[8].ToString().Replace("name-", ""));*/
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
                                string strParam = arrayStr[3].ToString();
                                string strunit = arrayParam[1].ToString();
                                string strname = arrayUnit[1].ToString();

                                param = new XElement("param", new XAttribute("name", strname), new XAttribute("unit", strunit), strParam);
                                paramList2.Add(param);
                            }
                        }
                    }
                }
                XElement sales_notes = new XElement("sales_notes", "Работаем по предоплате. Выгодные условия");
                offer.Add(sales_notes);
                offer.Add(market_category);
                offer.Add(urlTovar);
                offer.Add(priceTovar);
                offer.Add(oldPriceTovar);
                offer.Add(currencyIdTovar);
                offer.Add(categoryIdTovar);
                foreach (string s in images)
                {
                    pictureTovar = new XElement("picture", s);
                    offer.Add(pictureTovar);
                }
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
            string otv = nethouse.getRequest(url);
            MatchCollection categoryIdCollection = new Regex("(?<=categoryId\" value=\").*?(?=\">)").Matches(otv);
            string categoryId = categoryIdCollection[1].ToString();
            urlAllTovar = "https://bike18.ru/products/category/" + categoryId + "/page/all";
            return urlAllTovar;
        }

        private void Tovar2(CookieDictionary cookie, List<string> tovar)
        {

            string idTovar = tovar[0];
            string articleTovat = tovar[1];
            string nameTovar = tovar[2];
            string priceTovar = tovar[3];
            string newPriceTovar = tovar[4];
            string categoryTovar = tovar[5];
            string nalichieTovar = tovar[6];
            string postavkaTovar = tovar[7];
            string srokPostavkiTovar = tovar[8];
            string miniTextTovar = tovar[9];
            string fullTextTovar = tovar[10];
            string titleTovar = tovar[11];
            string descriptionTovar = tovar[12];
            string keywordsTovar = tovar[13];
            string slugTovar = tovar[14];

            string otv = "";
            List<string> tovarAtribute = new List<string>();
            string urlTovar = "https://bike18.ru/products/" + idTovar;

            List<string> listTovar = nethouse.GetProductList(cookie, urlTovar);
            if(listTovar == null || listTovar.Count == 0)
             {
                StreamWriter sr = new StreamWriter("erorTovar", true, Encoding.GetEncoding(1251));
                sr.WriteLine(idTovar);
                sr.Close();
            }
            else 
            {
                string id = idTovar;

                string group = listTovar[3].ToString();
                //if(group == "0")
                //    group = listTovar[41].ToString();
                if (slugTovar != "")
                    urlTovar = "https://bike18.ru/products/" + slugTovar;
                string marketCategory = listTovar[45].ToString();
                string paramName = "";
                string paramValue = "";
                string unit = "";
                string vendor = "";
                string url = urlTovar;
                string price = null;
                string oldPrice = null;
                if (newPriceTovar == "")
                    price = priceTovar;
                else
                {
                    price = newPriceTovar;
                    oldPrice = priceTovar;
                }

                string currencyId = "RUR";
                string categoryId = listTovar[2].ToString();
                string picture = "";// listTovar[32].ToString();

                MatchCollection allPictures = ReturnImagesTovar(cookie, id);

                string name = nameTovar;
                string description = EditDescription(miniTextTovar);
                string descriptionFull = EditDescription(fullTextTovar);
                description = " " + description + "<br />   " + descriptionFull;
                description = description.Replace("<br />\n", "\n").Replace("<br /><br />", "<br />").Replace("\n\n", "\n");
                description = description.Replace("<br />\n", "\n").Replace("<br /><br />", "<br />").Replace("\n\n", "\n");
                string available = "";
                if (nalichieTovar == "0")
                    available = "\"false\"";
                else
                    available = "\"true\"";

                string urlTovarAttrib = "";

                List<string> param = new List<string>();

                if (group != "00")
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
                    try
                    {
                        if (urlTovarAttrib != "https://bike18.nethouse.ru/api/v1/catalog/attributes/0/0")
                            otv = nethouse.getRequest(cookie, urlTovarAttrib);
                        //HttpWebResponse res = null;
                        //HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(urlTovarAttrib);
                        //req.Accept = "application/json, text/plain, */*";
                        //req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
                        //req.Method = "GET";
                        //req.CookieContainer = cookie;
                        //res = (HttpWebResponse)req.GetResponse();
                        //StreamReader ressr = new StreamReader(res.GetResponseStream());
                        //otv = ressr.ReadToEnd();
                        //res.Close();
                    }
                    catch
                    {
                        otv = "1";
                    }

                    if(otv != "")
                    {
                        if (categories.Count == 0)
                        {
                            ReturnCategories(cookie);
                        }
                        if (otv == "err")
                            return;
                        dynamic stuff1 = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(otv);
                        string ssss = stuff1.ToString();



                        string primaryKey = "";
                        string attributeId = "";
                        string empty = "";
                        string valueId = "";
                        string valueText = "";
                        string chekbox = "";

                        MatchCollection attributes = new Regex("(?<=primaryKey)[\\w\\W]*?(?=primaryKey)").Matches(ssss);
                        MatchCollection attributes2 = new Regex("(?<=primaryKey)[\\w\\W]*?(?=})").Matches(ssss);
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
                                                if (str2.Contains(valueId) && valueId != "")
                                                {
                                                    if (b && valueId != "")
                                                    {
                                                        valueText = new Regex("(?<=valueText\": \")[\\w\\W]*?(?=\")").Match(str2).ToString();
                                                        vendor = valueText;
                                                        param.Add("vendor;" + valueText);
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        valueText = new Regex("(?<=valueText\": \")[\\w\\W]*?(?=\")").Match(str2).ToString();
                                                        param.Add(paramName + ";" + valueText);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                    /* if (atributesTovar.Count != 0)
                     {
                         for (int i = atributesTovar.Count - 1; atributesTovar.Count > i; i++)
                         {
                             string strTovar = atributesTovar[i].ToString() + "&";

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

                             foreach (Match ss in attributes2)
                             {
                                 string str = ss.ToString();
                                 paramName = new Regex("(?<=name\": \")[\\w\\W]*?(?=\")").Match(str).ToString();
                                 unit = new Regex("(?<=unit\": \")[\\w\\W]*?(?=\")").Match(str).ToString();
                                 if (unit == "\\")
                                     unit = "\"";
                                 if (str.Contains(primaryKey) && primaryKey != "")
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
                                                 if (str2.Contains(valueId) && valueId != "")
                                                 {
                                                     if (b && valueId != "")
                                                     {
                                                         valueText = new Regex("(?<=valueText\": \")[\\w\\W]*?(?=\")").Match(str2).ToString();
                                                         vendor = valueText;
                                                         param.Add("vendor;" + valueText);
                                                         break;
                                                     }
                                                     else
                                                     {
                                                         valueText = new Regex("(?<=valueText\": \")[\\w\\W]*?(?=\")").Match(str2).ToString();
                                                         param.Add(paramName + ";" + valueText);
                                                         break;
                                                     }
                                                 }
                                             }
                                         }
                                     }
                                 }
                             }
                         }
                     }*/
                }


                tovarAtribute.Add("offer;id" + "-" + id + ";available" + "-" + available);
                if (marketCategory != "")
                    tovarAtribute.Add("market_category-" + marketCategory);
                tovarAtribute.Add("url-" + url);
                tovarAtribute.Add("price-" + price);
                if (oldPrice != null)
                    tovarAtribute.Add("oldPrice-" + oldPrice);
                tovarAtribute.Add("currencyId-" + currencyId);
                tovarAtribute.Add("categoryId-" + categoryId);
                string img = "picture-" + picture;
                int countImages = 0;
                foreach (Match ss in allPictures)
                {
                    string str = ss.ToString();
                    if (str.Contains(".jpg") || str.Contains(".jpeg") || str.Contains(".png") || str.Contains(".JPG"))
                    {
                        if (countImages > 9)
                            continue;
                        if (str.Contains("i.siteapi.org"))
                        {
                            img = img + ";http://" + str;
                            img = img.Replace("\\/", "/").Replace("///", "//");
                            img = img.Replace("\\/", "/").Replace("///", "//");
                            countImages++;
                        }
                        else
                        {
                            img = img + ";http://bike18.ru" + str;
                            img = img.Replace("\\/", "/").Replace("///", "//");
                            countImages++;
                        }
                    }
                }
                if (img != "picture-http://")
                    tovarAtribute.Add(img);
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

        private void Tovar(CookieDictionary cookie, string idTovar)
        {
            string otv = "";
            List<string> tovarAtribute = new List<string>();
            string urlTovar = "https://bike18.ru/products/" + idTovar;

            List<string> listTovar = nethouse.GetProductList(cookie, urlTovar);
            if (listTovar.Count != 0)
            {
                string id = listTovar[0].ToString();
                string group = listTovar[3].ToString();
                if (listTovar[1].ToString() != "")
                    urlTovar = "https://bike18.ru/products/" + listTovar[1].ToString();
                string marketCategory = listTovar[45].ToString();
                string paramName = "";
                string paramValue = "";
                string unit = "";
                string vendor = "";
                string url = urlTovar;
                string price = null;
                string oldPrice = null;
                if (listTovar[10].ToString() == "")
                    price = listTovar[9].ToString();
                else
                {
                    price = listTovar[10].ToString();
                    oldPrice = listTovar[9].ToString();
                }

                string currencyId = "RUR";
                string categoryId = listTovar[2].ToString();
                string picture = listTovar[32].ToString();

                MatchCollection allPictures = ReturnImagesTovar(cookie, id);

                string name = listTovar[4].ToString();
                string description = EditDescription(listTovar[7].ToString());
                string descriptionFull = EditDescription(listTovar[7].ToString());
                description = " " + description + "<br />   " + descriptionFull;
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

                    otv = nethouse.getRequest(cookie, urlTovarAttrib);

                    //HttpWebResponse res = null;
                    //HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(urlTovarAttrib);
                    //req.Accept = "application/json, text/plain, */*";
                    //req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
                    //req.Method = "GET";
                    //req.CookieContainer = cookie;
                    //res = (HttpWebResponse)req.GetResponse();
                    //StreamReader ressr = new StreamReader(res.GetResponseStream());
                    //otv = ressr.ReadToEnd();
                    //res.Close();

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

                    MatchCollection attributes = new Regex("(?<=primaryKey)[\\w\\W]*?(?=primaryKey)").Matches(ssss);
                    MatchCollection attributes2 = new Regex("(?<=primaryKey)[\\w\\W]*?(?=})").Matches(ssss);
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
                                            if (str2.Contains(valueId) && valueId != "")
                                            {
                                                if (b && valueId != "")
                                                {
                                                    valueText = new Regex("(?<=valueText\": \")[\\w\\W]*?(?=\")").Match(str2).ToString();
                                                    vendor = valueText;
                                                    param.Add("vendor;" + valueText);
                                                    break;
                                                }
                                                else
                                                {
                                                    valueText = new Regex("(?<=valueText\": \")[\\w\\W]*?(?=\")").Match(str2).ToString();
                                                    param.Add(paramName + ";" + valueText);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (atributesTovar.Count != 0)
                    {
                        for (int i = atributesTovar.Count - 1; atributesTovar.Count > i; i++)
                        {
                            string strTovar = atributesTovar[i].ToString() + "&";

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

                            foreach (Match ss in attributes2)
                            {
                                string str = ss.ToString();
                                paramName = new Regex("(?<=name\": \")[\\w\\W]*?(?=\")").Match(str).ToString();
                                unit = new Regex("(?<=unit\": \")[\\w\\W]*?(?=\")").Match(str).ToString();
                                if (unit == "\\")
                                    unit = "\"";
                                if (str.Contains(primaryKey) && primaryKey != "")
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
                                                if (str2.Contains(valueId) && valueId != "")
                                                {
                                                    if (b && valueId != "")
                                                    {
                                                        valueText = new Regex("(?<=valueText\": \")[\\w\\W]*?(?=\")").Match(str2).ToString();
                                                        vendor = valueText;
                                                        param.Add("vendor;" + valueText);
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        valueText = new Regex("(?<=valueText\": \")[\\w\\W]*?(?=\")").Match(str2).ToString();
                                                        param.Add(paramName + ";" + valueText);
                                                        break;
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


                tovarAtribute.Add("offer;id" + "-" + id + ";available" + "-" + available);
                if (marketCategory != "")
                    tovarAtribute.Add("market_category-" + marketCategory);
                tovarAtribute.Add("url-" + url);
                tovarAtribute.Add("price-" + price);
                if (oldPrice != null)
                    tovarAtribute.Add("oldPrice-" + oldPrice);
                tovarAtribute.Add("currencyId-" + currencyId);
                tovarAtribute.Add("categoryId-" + categoryId);
                string img = "picture-http://" + picture;
                foreach (Match ss in allPictures)
                {
                    string str = ss.ToString();
                    img = img + ";http://" + str;
                }
                tovarAtribute.Add(img);
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

        private MatchCollection ReturnImagesTovar(CookieDictionary cookie, string id)
        {
            string otv = nethouse.getRequest(cookie, "https://bike18.nethouse.ru/api/catalog/productmedia?id=" + id);
            MatchCollection images = new Regex("(?<=\"src\":\").*?(?=\")").Matches(otv);
            if (images.Count == 0)
                images = new Regex("(?<=formats\":{\"raw\":\").*?(?=\",\")").Matches(otv);
            return images;
        }

        private void ReturnCategories(CookieDictionary cookie)
        {
            List<string> counts = new List<string>();
            string otv = "";

            otv = nethouse.getRequest(cookie, "https://bike18.nethouse.ru/api/catalog/categoriesselect");

            //HttpWebResponse res = null;
            //HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://bike18.nethouse.ru/api/catalog/categoriesselect");
            //req.Accept = "application/json, text/plain, */*";
            //req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
            //req.Method = "GET";
            //req.CookieContainer = cookie;
            //res = (HttpWebResponse)req.GetResponse();
            //StreamReader ressr = new StreamReader(res.GetResponseStream());
            //otv = ressr.ReadToEnd();
            //res.Close();

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

                    File.WriteAllText("category.txt", str.ToString(), Encoding.GetEncoding(1251));
                    strCategor = File.ReadAllLines("category.txt", Encoding.GetEncoding(1251));
                    File.Delete("category.txt");

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
                if (ss.ToString() != "<br />")
                {
                    if (ss.ToString() == "</p>" || ss.ToString().Contains("<li") || ss.ToString().Contains("<a"))
                        descript = descript.Replace(ss.ToString(), " ");

                    if (ss.ToString().Contains("<p"))
                        descript = descript.Replace(ss.ToString(), "<br />");

                    descript = descript.Replace(ss.ToString(), "");
                }
            }
            descript.Trim();
            return descript;
        }

        private void ControlsFormEnabledTrue()
        {
            btnStart.Invoke(new Action(() => btnStart.Enabled = true));
            tbLogin.Invoke(new Action(() => tbLogin.Enabled = true));
            tbPassword.Invoke(new Action(() => tbPassword.Enabled = true));
        }

        private void ControlsFormEnabledFalse()
        {
            btnStart.Invoke(new Action(() => btnStart.Enabled = false));
            tbLogin.Invoke(new Action(() => tbLogin.Enabled = false));
            tbPassword.Invoke(new Action(() => tbPassword.Enabled = false));
        }
    }
}
