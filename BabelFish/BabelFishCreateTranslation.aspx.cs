using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using umbraco.cms.businesslogic.language;
using umbraco.cms.businesslogic.web;
using umbraco.BusinessLogic;
using umbraco.BasePages;

namespace BabelFish
{
    public partial class BabelFishCreateTranslation : BasePage
    {
        private int NodeID;
        private string PrimaryLanguageISO;

        private Document ParentDocument;

        private Document TranslationFolder;

        public static string BabelFishFolderDocTypeAlias = "BabelFishFolder";
        public static string BabelFishFolderName = System.Web.Configuration.WebConfigurationManager.AppSettings["BabelFish:TranslationsFolderName"];
        public static string LanguagePropertyAlias = System.Web.Configuration.WebConfigurationManager.AppSettings["BabelFish:LanguageParameterAlias"];
        public static string PropertySuffix = "Translation";        

        private DocumentType BabelFishFolderDocType = DocumentType.GetByAlias(BabelFishFolderDocTypeAlias);        

        private string TranslationDocTypeAlias;

        private List<string> CurrentTranslatedLanguages = new List<string>();
        private List<string> NewTranslatedLanguages = new List<string>();

        private CheckBoxList cbl = new CheckBoxList();

        private User CurrentUser;

        private void Page_Init(object sender, EventArgs e)
        {
            Authorize();

            //this setting determines your 'base' language
            PrimaryLanguageISO = BabelFish.Extensions.GetLanguage();

            //get information about the node to be translated
            NodeID = Convert.ToInt32(HttpContext.Current.Request.QueryString["nodeID"]);
            ParentDocument = new Document(NodeID);

            wrapperDiv.Controls.Add(cbl);

            TranslationDocTypeAlias = ParentDocument.ContentType.Alias + PropertySuffix;

            CheckForTranslationFolder();
        }

        private void Page_Load(object sender, EventArgs e)
        {

        }

        private void Page_PreRender(object sender, EventArgs e)
        {

            if (IsPostBack)
            {
                foreach (ListItem lang in cbl.Items)
                {
                    if (lang.Selected)
                    {
                        NewTranslatedLanguages.Add(lang.Value);
                    }
                }

                //test for the translation folder
                if (TranslationFolder == null)
                {
                    TranslationFolder = CreateTranslationFolder();
                    TranslationFolder.Publish(CurrentUser);
                }

                int count = 1;

                DocumentType translationDocType = DocumentType.GetByAlias(TranslationDocTypeAlias);

                foreach (string langISO in NewTranslatedLanguages)
                {
                    //we make sure the new translations inherit properly from the parent.
                    if (count == 1)
                    {
                        //DocumentType parentDocTypes;
                        ////here we catch an issue with v4.11.x, the result is some properties are not copied down
                        //try
                        //{
                        //    parentDocTypes= new DocumentType(translationDocType.Parent.Id);

                        //}
                        //catch (Exception e2)
                        //{
                        DocumentType parentDocTypes = new DocumentType(translationDocType.MasterContentType);
                            
                        //}
                            
                        translationDocType.allowedTemplates = parentDocTypes.allowedTemplates;
                        translationDocType.IconUrl = parentDocTypes.IconUrl;
                        translationDocType.DefaultTemplate = parentDocTypes.DefaultTemplate;
                        translationDocType.Save();
                        
                    }

                    string translationName = langISO;

                    Document newLangDocument = Document.MakeNew(translationName, translationDocType, CurrentUser, TranslationFolder.Id);
                    CopyProperties(ParentDocument, newLangDocument);

                    newLangDocument.getProperty(LanguagePropertyAlias).Value = langISO;
                    newLangDocument.Save();
                }

                BasePage.Current.ClientTools.ReloadActionNode(true, true).CloseModalWindow();
            }
            else
            {
                if (TranslationFolder != null)
                {
                    SetCurrentTranslatedLanguages();
                }


                bool hasAtLeastOne = false;

                foreach (Language lang in Language.GetAllAsList())
                {
                    string langISO = lang.CultureAlias.ToLower().Substring(0, 2);

                    if (!CurrentTranslatedLanguages.Contains(langISO))
                    {
                        if (langISO != PrimaryLanguageISO)
                        {
                            hasAtLeastOne = true;

                            cbl.Items.Add(new ListItem("<img src='/css/images/flags/" + langISO + ".png'/>" + lang.FriendlyName + " (" + lang.CultureAlias + ")", langISO));
                        }
                    }
                }

                if (!hasAtLeastOne)
                {
                    footer.Visible = false;
                    HtmlGenericControl div = new HtmlGenericControl("div");
                    wrapperDiv.Controls.Add(div);
                    div.InnerHtml = umbraco.library.GetDictionaryItem("BabelFishNoLanguages");// "There are no languages available that have not already been translated.";
                }
                else
                {

                }
            }
        }

        private Document CreateTranslationFolder()
        {
            Document translationFolder = Document.MakeNew(BabelFishFolderName, BabelFishFolderDocType, CurrentUser, ParentDocument.Id);

            translationFolder.sortOrder = -1;

            translationFolder.Save();
            translationFolder.Publish(CurrentUser);

            umbraco.library.UpdateDocumentCache(translationFolder.Id);
            return translationFolder;
        }

        private void CopyProperties(Document fromDoc, Document toDoc)
        {
            foreach (umbraco.cms.businesslogic.propertytype.PropertyType propertyType in ParentDocument.ContentType.PropertyTypes)
            {
                toDoc.getProperty(propertyType.Alias).Value = fromDoc.getProperty(propertyType.Alias).Value;
            }
        }

        private void CheckForTranslationFolder()
        {
            if (BabelFishFolderDocType != null)
            {
                foreach (Document child in ParentDocument.Children)
                {
                    if (child.ContentType.Alias == BabelFishFolderDocType.Alias)
                    {
                        TranslationFolder = child;
                        return;
                    }
                }
            }
        }

        private void SetCurrentTranslatedLanguages()
        {
            foreach (Document translation in TranslationFolder.Children)
            {
                try
                {
                    CurrentTranslatedLanguages.Add(translation.getProperty(LanguagePropertyAlias).Value.ToString().ToLower());
                }
                catch (Exception e) { }
            }
        }

        public void Authorize()
        {
            CurrentUser = umbraco.BusinessLogic.User.GetCurrent();

            if (CurrentUser == null)
            {
                HttpContext.Current.Response.StatusCode = 403;
                HttpContext.Current.Response.End();
            }
        }
    }
}