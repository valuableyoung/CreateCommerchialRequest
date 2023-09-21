using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Web.Services.Protocols;
using System.Net;
using CreateCommerchialRequest;
using CreateCommerchialRequest.Database;
//using CreateCommerchialRequest.Helpers;

namespace AddEASSpace
{
    class WStoEAS
    {
        ws_online5clonService ws_eas_clon; //= new ws_online5clonService();
        generateXml gx;

        public WStoEAS()
        {
            /*Change Arhipova 28.12.2022 теперь определяем на момент создания объекта куда подключать будем*/
            string str_con;
            string strdbcon = Connection.ConnectionString;  // "Data Source=DBSRV2;Initial Catalog=Clon;Integrated Security=True";
            if (strdbcon.ToLower().Contains("real"))
            {
                string ss = GetData(); /*В зависимости от того что за веб сервис для реальной БД активен выбираем вариант подключения */
                if (ss.Contains("Ошибка определения активного EAS"))
                {
                    /*чего делать то */
                    str_con = "";
                    return; ///????? наверное так нельзя, но зато грохнем создание
                }
                else
                {
                    if (ss.Contains("duplicate"))
                    { str_con = "http://easkis5duplicate"; }
                    else
                    { str_con = "http://easkis5"; }
                }
            }
            else
            {
                str_con = "http://easkis5clon";
            }

            TuningClass(str_con);
            TuningMethods(str_con);

            ws_eas_clon = new ws_online5clonService(str_con);
            /*End Change Arhipova 28.12.2022 */
            gx = new generateXml();


        }

        /*Какой веб сервис для реальной БД активен*/
        public static string GetData()
        {
            try
            {
                string url = @"https://arkona36.ru/eolite/settings/eas_server.eas";
                var data = GetActiveEAS(url);
                return data;
            }
            catch (Exception ex)
            {
                CreateCommerchialRequest.Helpers.UniLogger.WriteLog("", 1, "Ошибка определения активного EAS : " + ex.Message);
                return "Ошибка определения активного EAS. " + ex.Message;
            }

        }

        /// <summary>
        /// Настройка атрибутов класса
        /// </summary>
        void TuningClass(string Connect)
        {
            var curtype = typeof(ws_online5clonService);
            TypeInfo tInfo = curtype.GetTypeInfo();
            var wsattrs = tInfo.GetCustomAttributes();

            SetValueOfAttributeClass(wsattrs, Connect);
        }
        /// <summary>
        /// Настройка атрибутов методов класса
        /// </summary>
        void TuningMethods(string Connect)
        {
            var curtype = typeof(ws_online5clonService);
            SoapRpcMethodAttribute srpc = new SoapRpcMethodAttribute();

            var soaprpctype = srpc.GetType();
            MethodInfo[] mi2 = curtype.GetMethods();

            SetValueOfAttributeMethod(mi2, soaprpctype, Connect);
        }

        void SetValueOfAttributeClass(IEnumerable<Attribute> wsattrs, string Connect)
        {
            foreach (var attr in wsattrs)
            {
                if (attr.GetType().Name.Equals("WebServiceBindingAttribute"))
                {
                    Type t = attr.GetType();

                    var wsprops = t.GetRuntimeProperties();

                    foreach (var prop in wsprops)
                    {
                        if (prop.Name.Equals("Namespace"))
                        {
                            prop.SetValue(attr, Connect);
                        }
                    }
                }
            }
        }

        void SetValueOfAttributeMethod(MethodInfo[] mi2, Type soaprpctype, string Connect)
        {
            foreach (var mtd in mi2)
            {
                if (mtd.CustomAttributes.Count() > 0)
                {
                    var att = mtd.GetCustomAttribute(soaprpctype);
                    if (att != null)
                    {
                        PropertyInfo pi_request = soaprpctype.GetProperty("RequestNamespace");
                        pi_request.SetValue(att, Connect);

                        PropertyInfo pi_response = soaprpctype.GetProperty("ResponseNamespace");
                        pi_response.SetValue(att, Connect);
                    }
                }
            }
        }

        public static string GetActiveEAS(string url)
        {
            try
            {
                var client = new System.Net.Http.HttpClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                var content = client.GetStringAsync(url).Result;

                //Console.WriteLine(content);
                return content;

            }
            catch (System.Net.Http.HttpRequestException e)
            {
                //Console.WriteLine("ошибка :{0} ", e.Message);
                return e.Message;
            }
        }

        public static string GetValue(string xmlToStringFormat)
        {
            XmlDocument doc = new XmlDocument();

            doc.LoadXml(xmlToStringFormat);

            if (doc.SelectSingleNode("message/row/errmessage") != null)
            {
                var errnumber = doc.SelectSingleNode("message/row/errnumber").InnerText;

                return $@"Код ошибки {errnumber} {doc.SelectSingleNode("message/row/errmessage").InnerText}";
            }
            else
            {
                //var ansnumber = doc.SelectSingleNode("message/row/errnumber").InnerText;
                //return $@"Код ответа {ansnumber} {doc.SelectSingleNode("message/row/message").InnerText}";
                return $@"{doc.SelectSingleNode("message/row/message").InnerText}";
            }
        }

        public int rc_checkwork()
        {
            try
            {
                return ws_eas_clon.rc_checkwork();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public string rc_loadtovdoc(int iduser, string iddoc)
        {
            string xmlauth = "";
            string xmlrequest = "";
            string s = "";
            try
            {
                //                iduser = 549003;
                var login = DBUsers.getUserLoginPassword(iduser)["login"].ToString();
                var password = Md5.MD5Hash(DBUsers.getUserLoginPassword(iduser)["pass"].ToString());

                xmlauth = gx.generateUserData(login, password);
                xmlrequest = gx.generateIdDocXml(iddoc);

                if (ws_eas_clon == null)
                {
                    s = "Ошибка: Нет объекта ws_eas_clon!!!";
                }
                else
                {
                    s = ws_eas_clon.rc_loadtovdoc(xmlauth, xmlrequest);
                }

                return GetValue(s); //GetValue(ws_eas_clon.rc_loadtovdoc(xmlauth, xmlrequest));
            }
            catch (Exception ex)
            {
                UniLogger.WriteLog("", 1, $"Ошибка метода rc_loadtovdoc: {ex.Message} \r\n {xmlauth} \r\n {xmlrequest} \r\n {s}");
                return $"-1 {ex.Message}";
            }
        }

        public string rc_unloadtovdoc(int iduser, string iddoc)
        {
            try
            {
                //               iduser = 549003;

                //generateXml gx = new generateXml();

                var login = DBUsers.getUserLoginPassword(iduser)["login"].ToString();
                var password = Md5.MD5Hash(DBUsers.getUserLoginPassword(iduser)["pass"].ToString());

                string xmlauth = gx.generateUserData(login, password);
                string xmlrequest = gx.generateIdDocXml(iddoc);


                return GetValue(ws_eas_clon.rc_unloadtovdoc(xmlauth, xmlrequest));
            }
            catch (Exception ex)
            {
                // UniLogger.WriteLog("", 1, $"Ошибка метода rc_loadfindoc: {ex.Message} \r\n {xmlauth} \r\n {xmlrequest} \r\n {s}");
                return $"-1 {ex.Message}";
            }
        }

        public string rc_loadfindoc(int iduser, string iddoc)
        {
            string xmlauth = "";
            string xmlrequest = "";
            string s = "";

            try
            {
                //            iduser = 549003;



                var login = DBUsers.getUserLoginPassword(iduser)["login"].ToString();
                var password = Md5.MD5Hash(DBUsers.getUserLoginPassword(iduser)["pass"].ToString());

                xmlauth = gx.generateUserData(login, password);
                xmlrequest = gx.generateIdDocXml(iddoc);

                if (ws_eas_clon == null)
                {
                    s = "Ошибка: Нет объекта ws_eas_clon!!!";
                }
                else
                {
                    s = ws_eas_clon.rc_loadfindoc(xmlauth, xmlrequest);
                }
                return GetValue(s);
            }
            catch (Exception ex)
            {
                UniLogger.WriteLog("", 1, $"Ошибка метода rc_loadfindoc: {ex.Message} \r\n {xmlauth} \r\n {xmlrequest} \r\n {s}");
                return $"-1 {ex.Message}";
            }
        }

        public string rc_unloadfindoc(int iduser, string iddoc)
        {
            try
            {
                //            iduser = 549003;

                //generateXml gx = new generateXml();

                var login = DBUsers.getUserLoginPassword(iduser)["login"].ToString();
                var password = Md5.MD5Hash(DBUsers.getUserLoginPassword(iduser)["pass"].ToString());

                string xmlauth = gx.generateUserData(login, password);
                string xmlrequest = gx.generateIdDocXml(iddoc);

                return GetValue(ws_eas_clon.rc_unloadfindoc(xmlauth, xmlrequest));
            }
            catch (Exception ex)
            {
                return $"-1 {ex.Message}";
            }
        }

        public string rc_reservtovdoc(int iduser, string iddoc)
        {

            string xmlauth = "";
            string xmlrequest = "";
            string s = "";

            try
            {

                var login = DBUsers.getUserLoginPassword(iduser)["login"].ToString();
                var password = Md5.MD5Hash(DBUsers.getUserLoginPassword(iduser)["pass"].ToString());

                xmlauth = gx.generateUserData(login, password);
                xmlrequest = gx.generateIdDocXml(iddoc);

                if (ws_eas_clon == null)
                {
                    s = "Ошибка: Нет объекта ws_eas_clon!!!";
                }
                else
                {
                    s = ws_eas_clon.rc_reservtovdoc(xmlauth, xmlrequest);
                }
                return GetValue(s);
            }
            catch (Exception ex)
            {
                UniLogger.WriteLog("", 1, $"Ошибка метода rc_reservtovdoc: {ex.Message} \r\n {xmlauth} \r\n {xmlrequest} \r\n {s}");
                return $"-1 {ex.Message}";
            }
        }

        public string rc_findtov(int iduser, string oem) /*не дописано, тут надо передавать много параметров*/
        {
            try
            {

                var login = DBUsers.getUserLoginPassword(iduser)["login"].ToString();
                var password = Md5.MD5Hash(DBUsers.getUserLoginPassword(iduser)["pass"].ToString());

                string xmlauth = gx.generateUserData(login, password);
                string xmlrequest = gx.generateFindTovXml(oem, iduser, 31);

                var ret = ws_eas_clon.rc_findtov(xmlauth, xmlrequest);

                return GetValue(ret);
            }
            catch (Exception ex)
            {
                return $"-1 {ex.Message}";
            }
        }

        public string rc_createorderbuyZNP(int iduser, string idRowZKP = "0", string Kol = "0", string Price = "0")
        {
            string xmlauth = "";
            string xmlrequest = "";
            string s = "";
            try
            {
                //                iduser = 549003;
                var login = DBUsers.getUserLoginPassword(iduser)["login"].ToString();
                var password = Md5.MD5Hash(DBUsers.getUserLoginPassword(iduser)["pass"].ToString());

                xmlauth = gx.generateUserData(login, password);
                string[] sparams = new string[] { idRowZKP, Kol ,Price };
                 
                if(idRowZKP != "0")
                {
                    xmlrequest = gx.generateIdDocXml("",sparams);
                }
                 

                if (ws_eas_clon == null)
                {
                    s = "Ошибка: Нет объекта ws_eas_clon!!!";
                }
                else
                {

                    s =  ws_eas_clon.rc_createorderbuyzkptov(xmlauth, xmlrequest).ToString(); //rc_createorderbuyzkptov rc_createorderbuyZNP
                    //s = ws_eas_clon.rc_checkwork().ToString();

                    //s = ws_eas_clon.rc_reservtovdoc(xmlauth, xmlrequest).ToString();
                }

                return GetValue(s); 
            }
            catch (Exception ex)
            {
                 
                string exa = ex.Message.ToString() + ex.StackTrace;
                UniLogger.WriteLog("", 1, $"Ошибка метода rc_createorderbuyzkptov: {ex.Message} \r\n {xmlauth} \r\n {xmlrequest} \r\n {s}");
                return $"-1 {ex.Message}";
            }
        }

        public enum ColorFormat
        {
            RGB, RGBA, ARGB
        }

        public static int ColorToDecimal(Color color, ColorFormat format = ColorFormat.RGB)
        {
            switch (format)
            {
                default:
                case ColorFormat.RGB:
                    return color.R << 16 | color.G << 8 | color.B;
                case ColorFormat.RGBA:
                    return color.R << 24 | color.G << 16 | color.B << 8 | color.A;
                case ColorFormat.ARGB:
                    return color.A << 24 | color.R << 16 | color.G << 8 | color.B;
            }
        }

        public static Color DecimalToColor(int dec, ColorFormat format = ColorFormat.RGB)
        {
            switch (format)
            {
                default:
                case ColorFormat.RGB:
                    return Color.FromArgb((dec >> 16) & 0xFF, (dec >> 8) & 0xFF, dec & 0xFF);
                case ColorFormat.RGBA:
                    return Color.FromArgb(dec & 0xFF, (dec >> 24) & 0xFF, (dec >> 16) & 0xFF, (dec >> 8) & 0xFF);
                case ColorFormat.ARGB:
                    return Color.FromArgb((dec >> 24) & 0xFF, (dec >> 16) & 0xFF, (dec >> 8) & 0xFF, dec & 0xFF);
            }
        }

    }


}
