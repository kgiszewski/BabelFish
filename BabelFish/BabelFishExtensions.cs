using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Threading;
using System.Globalization;
using Umbraco.Core.Models;
using Umbraco.Core;
using Umbraco.Web;
using umbraco.cms.businesslogic.language;

namespace BabelFish
{
    public static class Extensions
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
            foreach (umbraco.cms.businesslogic.language.Language lang in umbraco.cms.businesslogic.language.Language.GetAllAsList())
            {
                if (lang.CultureAlias.Substring(0, 2) == langISO)
                {
                    return lang.CultureAlias;
                }
            }

            return "en-US";//default
        }

        public static string NiceUrlWithBabelFish(this IPublishedContent content)
        {
            if (content == null)
                return "";

            return BabelFish.BfHelper.TranslateUrl(content);
        }

        public static string NiceUrlWithBabelFish(this IPublishedContent content, string language, int index)
        {
            var url = BabelFish.BfHelper.TranslateUrl(content);

            string[] pathPieces = url.Split('/');

            pathPieces[index] = language;

            return string.Join("/", pathPieces);
        }

        public static IPublishedContent TranslateWithBabelFish(this IPublishedContent content)
        {
            return BabelFish.BfHelper.Translate(content);
        }
    }
}