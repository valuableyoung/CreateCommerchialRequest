using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CreateCommerchialRequest
{
    public class generateXml
    {
        public string generateIdDocXml(string idDoc="" , string[] idDocs = null)
        {
            string typeXml = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>";
            XDocument xdoc2 = new XDocument();
            //XElement elem = new XElement("row");
            //XAttribute atrib = new XAttribute("idDoc", "idDoc");
            //XElement idtovdoc = new XElement("iddoc", idDoc);
            //elem.Add(idtovdoc);

            //XElement param = new XElement("param");
            //param.Add(elem);
            //xdoc2.Add(param);

            ////typeXml += xdoc2;
            //return xdoc2.ToString();


            if (idDocs == null)
            {
                
                XElement elem = new XElement("row");
                XAttribute atrib = new XAttribute("idDoc", "idDoc");
                XElement idtovdoc = new XElement("iddoc", idDoc);
                elem.Add(idtovdoc);

                XElement param = new XElement("param");
                param.Add(elem);
                xdoc2.Add(param);
            }
            else
            {
 
                   
                XElement elem = new XElement("row");
                XAttribute atribdRowZKP = new XAttribute("idrowzkp", "idrowzkp");
                XElement idtovdocdRowZKP = new XElement("idrowzkp", idDocs[0]);
                elem.Add(idtovdocdRowZKP);

                
                
                XAttribute atribKol = new XAttribute("kol", "kol");
                XElement idtovdocKol = new XElement("kol", idDocs[1]);
                elem.Add(idtovdocKol);


                
                XAttribute atribPrice = new XAttribute("price", "price");
                XElement idtovdocPrice = new XElement("price", idDocs[2]);
                elem.Add(idtovdocPrice);



                XElement param = new XElement("param");
                param.Add(elem);
                
                
                xdoc2.Add(param);



            }
            return xdoc2.ToString();
        }


        public string generateFindTovXml(string oem, int idkontr, int cat)
        {
            string typeXml = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>";
            XDocument xdoc = new XDocument();
            XElement elem = new XElement("row");
            XAttribute atrib = new XAttribute("idcust", "idcust");
            XElement idcust = new XElement("idcust", idkontr);
            elem.Add(idcust);

            XAttribute atrib1 = new XAttribute("category", "category");
            XElement category = new XElement("category", cat);
            elem.Add(category);

            XAttribute atrib2 = new XAttribute("ntov", "ntov");
            XElement ntov = new XElement("ntov", "");
            elem.Add(ntov);



            XElement param = new XElement("param");
            param.Add(elem);
            xdoc.Add(param);

            return xdoc.ToString();

            //           < findtov >
            //< row >
            // < idcust /> -Код покупателя - Заполнить
            //  < category /> -Категория покупателя - Заполнить
            //    < ntov /> -пусто
            //    < idtm /> -код бренда.Заполнить для поиска по Артикулу.

            //    < ntm /> -Бренд.Заполнить для поиска по Артикулу.
            // < idtov4 /> -пусто
            // < ordern /> -пусто
            // < idvendor /> -пусто
            // < idoem /> -Артикул - заполнить
            // < pcross /> -Признак поиска с кроссами(1 / 0) - заполнить
            //  < idtov /> -код товара
            //   < idtoplevel /> -код уровня при поиске по Дереву классификатора
            // < whocall /> -пусто
            // < fassortkontr /> -пусто
            // < foursklad /> -Признак "поиск только по нашему складу"(1 да / 0 нет
            //   < page /> -страница, которую отображаем(если 0, то все)
            //   < falltov /> -использовать ли ограничение на прайс поставщиков в 5000 наименований(0 - да, 1 - нет)
            //  </ row >
            // </ findtov >

        }

        public string generateUserData(string login, string password)
        {
            //password = password.MD5Hash();
            string typeXml = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>";
            XDocument xdoc = new XDocument();
            XElement row = new XElement("row");
            XAttribute atr = new XAttribute("login", "password");
            XElement user = new XElement("userlogin", $"{login}");
            XElement passw = new XElement("userpass", $"{password}");
            row.Add(user);
            row.Add(passw);

            XElement loginpass = new XElement("loginpass");
            loginpass.Add(row);
            xdoc.Add(loginpass);

            typeXml += xdoc;
            return typeXml;
        }

        public string generateIdZakZNPXml(string idZak)
        {
            string typeXml = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>";
            XDocument xdoc2 = new XDocument();
            XElement elem = new XElement("row");
            XAttribute atrib = new XAttribute("idZak", "idZak");
            XElement idtovdoc = new XElement("idZak", idZak);
            elem.Add(idtovdoc);

            XElement param = new XElement("param");
            param.Add(elem);
            xdoc2.Add(param);

            //typeXml += xdoc2;
            return xdoc2.ToString();
        }

    }

    public static class Md5
    {

        public static string MD5Hash(this string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input.Trim()));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }

}