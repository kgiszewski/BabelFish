using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace BabelFish
{
    public class BabelFishModule : IHttpModule
    {
        private List<string> IsoList = new List<string>();
        private string PrimaryLanguage = System.Web.Configuration.WebConfigurationManager.AppSettings["BabelFish:PrimaryLanguage"];
        public string BabelFishFolderName = System.Web.Configuration.WebConfigurationManager.AppSettings["BabelFish:TranslationsFolderName"];
        private string SelectedLanguage;

        public void Init(System.Web.HttpApplication app)
        {
            app.BeginRequest += new EventHandler(OnBeginRequest);
        }

        public void Dispose()
        {

        }

        public void OnBeginRequest(Object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;

            try
            {
                IsoList.AddRange(System.Web.Configuration.WebConfigurationManager.AppSettings["BabelFish:IsoList"].Split(','));
            }
            catch { }

            string path = app.Context.Request.Path;
            //app.Context.Response.Write("path=>"+path+"<br/>");

            List<string> pathPieces = new List<string>();
            pathPieces.AddRange(path.Split('/').Where(o => o != ""));  

            if (pathPieces.Count > 1)
            {
                //must be at something like mydomain.com/en/home or mydomain.com/somefolder/'langiso'/home
                //grab the second to last pieces
                SelectedLanguage = pathPieces[pathPieces.Count - 2];
                //app.Context.Response.Write("lang=>" + SelectedLanguage + pathPieces.Count+"<br/>");

                if (IsoList.Contains(SelectedLanguage) || SelectedLanguage==PrimaryLanguage)
                {
                    string newPath = "";
                    pathPieces.Remove(SelectedLanguage);

                    if (PrimaryLanguage == SelectedLanguage)
                    {
                        //primary language, just strip out the lang

                        newPath = String.Join("/", pathPieces);
                    }
                    else
                    {
                        //not primary language 
                        //the expected input will be mydomain.com/somefolder/'langiso'/home
                        //the expected output path will be mydomain.com/somefolder/'TranslationsFolderName'/home

                        string pageName = pathPieces.Last();
                        pathPieces.Remove(pageName);
                        pathPieces.Add(BabelFishFolderName.ToLower());
                        pathPieces.Add(pageName);

                        newPath = String.Join("/", pathPieces);
                    }

                    //app.Context.Response.Write("new=>~/" + newPath + "?lang=" + SelectedLanguage + RebuildQueryString(app) + "<br/>");
                    app.Context.RewritePath("~/" + newPath + "?lang=" + SelectedLanguage + RebuildQueryString(app), false);
                }
                else
                {
                    //must be on the root domain i.e.  mydomain.com/en  or mydomain.com
                }                
            }
        }

        string RebuildQueryString(HttpApplication app)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string key in app.Context.Request.QueryString.AllKeys)
            {
                if(key != "lang"){
                    sb.Append("&" + key + "=" + app.Context.Request.QueryString[key]);
                }
            }
            return sb.ToString();
        }
    }
}