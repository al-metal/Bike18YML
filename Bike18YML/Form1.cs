using Bike18;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bike18YML
{
    public partial class Form1 : Form
    {
        nethouse nethouse = new nethouse();
        httpRequest request = new httpRequest();
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
                    Tovar(cookie, tovar2);
                }
                else if(razdel2.Count != 0)
                {

                }
                else
                {

                }

            }

        }

        private void Tovar(CookieContainer cookie, MatchCollection tovar)
        {
            for(int i = 0; tovar.Count > i; i++)
            {
                string urlTovar = tovar[i].ToString();
                List<string> listTovar = nethouse.GetProductList(cookie, urlTovar);
                string id = listTovar[0].ToString();
                string available = "";
                if (listTovar[43].ToString() == "0")
                    available = "\"false\"";
                else
                    available = "\"true\"";
                string marketCategory = "";//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
                string url = urlTovar;
                string price = listTovar[9].ToString();
                string currencyId = "RUR";
                string categoryId = listTovar[2].ToString();
                string picture = listTovar[32].ToString();
                string name = listTovar[4].ToString();
                string description = EditDescription(listTovar[7].ToString());
                
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
