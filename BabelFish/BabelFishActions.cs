using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.interfaces;

namespace BabelFish
{
    public class ActionCreateTranslation : IAction
    {
        //private static ActionView instance = new ActionView();
        private string _alias = "BabelFishCreateTranslation";
        private string _path;

        //public static ActionView Instance
        //{
        //    get { return instance; }
        //}

        #region IAction Members
        public string Alias
        {
            get
            {
                return _alias;
            }
        }

        public bool CanBePermissionAssigned
        {
            get
            {
                return false;
            }
        }

        public string Icon
        {
            get
            {
                return "/umbraco/plugins/BabelFish/CreateTranslation.png";
            }
        }

        public string JsFunctionName
        {
            get
            {
                return "BabelFishCreateTranslation();";

            }
        }

        public string JsSource
        {
            get
            {
                return "/umbraco/plugins/BabelFish/BabelFishActionCreateTranslation.js";
            }
        }

        public char Letter
        {
            get
            {
                return ')';
            }
        }

        public bool ShowInNotifier
        {
            get
            {
                return false;
            }
        }

        public ActionCreateTranslation()
        {

        }

        #endregion
    }
}