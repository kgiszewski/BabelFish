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
            var languageParameter = HttpContext.Current.Request.QueryString["lang"];

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
            var langISO = HttpContext.Current.Request.QueryString["lang"];

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

            return TranslateUrl(content);
        }

        public static string NiceUrlWithBabelFish(this IPublishedContent content, string language, int index)
        {
            var url = TranslateUrl(content);

            var pathPieces = url.Split('/');

            pathPieces[index] = language;

            return string.Join("/", pathPieces);
        }

        public static IPublishedContent TranslateWithBabelFish(this IPublishedContent content)
        {
            var langISO = Extensions.GetLanguage();

            try
            {
                if (langISO.Contains(','))
                {
                    langISO = langISO.Split(',')[0];
                }

                if (langISO == System.Web.Configuration.WebConfigurationManager.AppSettings["BabelFish:PrimaryLanguage"])
                {
                    return content;
                }
                else
                {
                    var list = content
                        .Children
                        .Where(o => o.DocumentTypeAlias == BabelFishCreateTranslation.BabelFishFolderDocTypeAlias)
                        .First()
                        .Children;

                    foreach (var translation in list)
                    {
                        var thisTranslationISO = translation.GetProperty(BabelFishCreateTranslation.LanguagePropertyAlias).Value.ToString();

                        if (thisTranslationISO == langISO)
                        {
                            return translation;
                        }
                    }
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static string TranslateUrl(IPublishedContent content)
        {
            var langISO = Extensions.GetLanguage();

            if (langISO == System.Web.Configuration.WebConfigurationManager.AppSettings["BabelFish:PrimaryLanguage"])
            {
                return content.Url;
            }
            else
            {
                var pathPieces = new List<string>();
                pathPieces.AddRange(content.Url.Split('/').Where(o => o != ""));

                //replace the 'translations' element in the url
                if (pathPieces.Contains(BabelFishCreateTranslation.BabelFishFolderName.ToLower()))
                {
                    var index = pathPieces.FindIndex(o => o == BabelFishCreateTranslation.BabelFishFolderName.ToLower());
                    pathPieces[index] = langISO;

                    return "/" + String.Join("/", pathPieces);
                }
            }

            return content.Url;
        }
    }
}