using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using umbraco.BusinessLogic;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace BabelFish
{
    public class BfHelper
    {
        public static IPublishedContent Translate(IPublishedContent content)
        {
            string langISO = Extensions.GetLanguage();
            //Log.Add(LogTypes.Custom, 0, "langISO=>" + langISO);

            try
            {
                if (langISO.Contains(','))
                {
                    //Log.Add(LogTypes.Debug, 0, "Translator detected=>" + langISO);
                    langISO = langISO.Split(',')[0];
                }

                if (langISO == System.Web.Configuration.WebConfigurationManager.AppSettings["BabelFish:PrimaryLanguage"])
                {
                    return content;
                }
                else
                {
                    IEnumerable<IPublishedContent> list =
                        content
                        .Children
                        .Where(o => o.DocumentTypeAlias == BabelFishCreateTranslation.BabelFishFolderDocTypeAlias)
                        .First()
                        .Children;                        
                        
                    foreach(IPublishedContent translation in list){

                        //Log.Add(LogTypes.Custom, 0, "Checking=>" +translation.Name +" " +translation.GetProperty(BabelFishCreateTranslation.LanguagePropertyAlias).Value);

                        string thisTranslationISO = translation.GetProperty(BabelFishCreateTranslation.LanguagePropertyAlias).Value.ToString();

                        if (thisTranslationISO == langISO)
                        {
                            return translation;
                        }
                    }
                    return content;
                }
            }
            catch (Exception e)
            {
                //Log.Add(LogTypes.Custom, 0, "Translate exeption=>" + e.Message);
                return content;
            }
        }

        public static Boolean HasTranslation(IPublishedContent content)
        {
            string langISO = Extensions.GetLanguage();
            //Log.Add(LogTypes.Custom, 0, "langISO=>" + langISO);

            try
            {
                if (langISO.Contains(','))
                {
                    //Log.Add(LogTypes.Debug, 0, "Translator detected=>" + langISO);
                    langISO = langISO.Split(',')[0];
                }

                if (langISO == System.Web.Configuration.WebConfigurationManager.AppSettings["BabelFish:PrimaryLanguage"])
                {
                    return true;
                }
                else
                {
                    IEnumerable<IPublishedContent> list =
                        content
                        .Children
                        .Where(o => o.DocumentTypeAlias == BabelFishCreateTranslation.BabelFishFolderDocTypeAlias)
                        .First()
                        .Children;

                    foreach (IPublishedContent translation in list)
                    {

                        //Log.Add(LogTypes.Custom, 0, "Checking=>" +translation.Name +" " +translation.GetProperty(BabelFishCreateTranslation.LanguagePropertyAlias).Value);

                        string thisTranslationISO = translation.GetProperty(BabelFishCreateTranslation.LanguagePropertyAlias).Value.ToString();

                        if (thisTranslationISO == langISO)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            catch (Exception e)
            {
                //Log.Add(LogTypes.Custom, 0, "Translate exeption=>" + e.Message);
                return false;
            }
        }

        public static string TranslateUrl(IPublishedContent content, Boolean translateContent=true)
        {
            string langISO = Extensions.GetLanguage();

            if (translateContent)
            {
                content = Translate(content);
            }

            //Log.Add(LogTypes.Custom, 0, "Lang=>"+langISO);

            if (langISO == System.Web.Configuration.WebConfigurationManager.AppSettings["BabelFish:PrimaryLanguage"])
            {
                //Log.Add(LogTypes.Custom, 0, "primary");
                return content.Url;
            }
            else
            {
                //Log.Add(LogTypes.Custom, 0, content.Url);

                List<string> pathPieces = new List<string>();
                pathPieces.AddRange(content.Url.Split('/').Where(o => o != ""));

                //replace the 'translations' element in the url
                if (pathPieces.Contains(BabelFishCreateTranslation.BabelFishFolderName.ToLower()))
                {
                    int index = pathPieces.FindIndex(o => o == BabelFishCreateTranslation.BabelFishFolderName.ToLower());
                    pathPieces[index] = langISO;

                    return "/" + String.Join("/", pathPieces);
                }

            }

            //Log.Add(LogTypes.Custom, 0, "default");
            return content.Url;
        }
    }
}