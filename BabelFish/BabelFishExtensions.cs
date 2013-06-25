using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using umbraco.BusinessLogic;
using umbraco.NodeFactory;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Threading;
using System.Globalization;

using umbraco.cms.businesslogic.language;

namespace BabelFish
{
    public class Extensions
    {
        public static string GetLanguage()
        {
            string languageParameter = HttpContext.Current.Request.QueryString["lang"];

            if (!String.IsNullOrEmpty(languageParameter))
            {
                if (languageParameter.Contains(','))
                {
                    languageParameter = languageParameter.Split(',')[0];
                }
            }
            else
            {
                try
                {
                    languageParameter = System.Web.Configuration.WebConfigurationManager.AppSettings["BabelFish:PrimaryLanguage"];
                }
                catch
                {
                    languageParameter = "en";
                }
            }

            return languageParameter;
        }

        public static XPathNodeIterator Translate(string nodeID)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml("<language/>");
            XPathNavigator xn = xd.CreateNavigator();

            string langISO = GetLanguage();

            try
            {
                if (langISO.Contains(','))
                {
                    //Log.Add(LogTypes.Debug, 0, "Translator detected=>" + langISO);
                    langISO = langISO.Split(',')[0];
                }

                if (langISO == System.Web.Configuration.WebConfigurationManager.AppSettings["BabelFish:PrimaryLanguage"])
                {
                    return umbraco.library.GetXmlNodeById(nodeID);
                }
                else
                {
                    Node rootNode = new Node(Convert.ToInt32(nodeID));

                    foreach (Node child in rootNode.ChildrenAsList)
                    {
                        if (child.NodeTypeAlias == BabelFishCreateTranslation.BabelFishFolderDocTypeAlias)
                        {
                            foreach (Node translation in child.ChildrenAsList)
                            {
                                if (translation.GetProperty(BabelFishCreateTranslation.LanguagePropertyAlias).Value == langISO)
                                {
                                    return umbraco.library.GetXmlNodeById(translation.Id.ToString());
                                }
                            }
                            return xn.Select("//noTranslation");
                        }
                    }
                    return xn.Select("//noTranslation");
                }
            }
            catch (Exception e)
            {
                return umbraco.library.GetXmlNodeById(nodeID);
            }
        }

        public static string NiceUrl(string nodeID)
        {
            string url = "";
            string langISO = GetLanguage();

            try
            {
                url = umbraco.library.NiceUrl(Convert.ToInt32(nodeID));
                //Log.Add(LogTypes.Custom, 0, "nice=>"+umbracoURL);
            }
            catch (Exception e)
            {
                //Log.Add(LogTypes.Custom, 0, "e=>" + e.Message);
            }

            if (langISO == System.Web.Configuration.WebConfigurationManager.AppSettings["BabelFish:PrimaryLanguage"])
            {

            }
            else
            {
                List<string> pathPieces = new List<string>();
                pathPieces.AddRange(url.Split('/').Where(o => o != ""));

                //replace the 'translations' element in the url
                if (pathPieces.Contains(BabelFishCreateTranslation.BabelFishFolderName.ToLower()))
                {
                    int index = pathPieces.FindIndex(o => o == BabelFishCreateTranslation.BabelFishFolderName.ToLower());
                    pathPieces[index] = langISO;

                    url = "/"+String.Join("/", pathPieces);
                }

            }
            //Log.Addoo(LogTypes.Custom, 0, "nicer=>" + url);
            return url;
        }

        public static string GetUrlParameter(string param)
        {
            try
            {
                return HttpContext.Current.Request.QueryString[param].Split(',')[0];
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public static string ImplodeNodes(XPathNodeIterator xi, string delimiter)
        {
            string implodedString = "";
            int count = 0;

            foreach (XPathNavigator node in xi)
            {
                if (count == 0)
                {
                    implodedString += node.Value.ToString();
                }
                else
                {
                    implodedString += delimiter + node.Value.ToString();
                }
                count++;
            }
            return implodedString;
        }

        public static string ToLower(string input)
        {
            return input.ToLower();
        }

        public static string ToUpper(string input)
        {
            return input.ToUpper();
        }

        public static string GetPageStatusCode()
        {
            return HttpContext.Current.Response.StatusCode.ToString();
        }

        public static string GetCurrentUrl()
        {
            return HttpContext.Current.Request.Url.AbsoluteUri;
        }

        public static XPathNodeIterator GetXmlFromUri(string uri, string nodeName)
        {

            WebRequest request = WebRequest.Create(uri);

            // Set the Method property of the request to GET.
            request.Method = "GET";

            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(responseStream);
            XmlDocument xd = new XmlDocument();
            string responseXml = reader.ReadToEnd();

            xd.LoadXml(responseXml);

            // Close the Stream object.
            responseStream.Close();

            XPathNavigator xn = xd.CreateNavigator();
            return xn.Select("//" + nodeName);

        }

        public static string Replace(string needle, string haystack, string replacement)
        {
            return haystack.Replace(needle, replacement);
        }

        public static string GetCurrentYear()
        {
            return DateTime.Today.Year.ToString();
        }

        public static string SetPageCulture()
        {
            string langISO=HttpContext.Current.Request.QueryString["lang"];

            if (langISO == null)
            {
                langISO = "en";
            }

            langISO = GetFullLangCode(langISO);

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(langISO);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(langISO);
            return Thread.CurrentThread.CurrentCulture.DisplayName;
        }

        public static string GetFullLangCode(string langISO)
        {
            foreach (Language lang in Language.GetAllAsList())
            {
                if (lang.CultureAlias.Substring(0, 2) == langISO)
                {
                    return lang.CultureAlias;
                }
            }

            return "en-US";//default
        }
    }
}