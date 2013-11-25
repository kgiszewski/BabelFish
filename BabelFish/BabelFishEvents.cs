using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.web;
using umbraco.businesslogic;
using umbraco.cms.presentation.Trees;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace BabelFish
{
    public class AddTranslationAction : ApplicationEventHandler
    {
        public AddTranslationAction()
        {
            BaseTree.BeforeNodeRender += new BaseTree.BeforeNodeRenderEventHandler(this.BaseTree_BeforeNodeRender);
        }

        private void BaseTree_BeforeNodeRender(ref XmlTree sender, ref XmlTreeNode node, EventArgs e)
        {
            //LogHelper.Info<AddTranslationAction>(node.NodeType.ToLower());

            switch (node.NodeType.ToLower())
            {
                case "content":              
                    try
                    {
                        var document = ApplicationContext.Current.Services.ContentService.GetById(Convert.ToInt32(node.NodeID));
                        
                        var translationDocType = ApplicationContext.Current.Services.ContentTypeService.GetContentType(document.ContentType.Alias + BabelFishCreateTranslation.PropertySuffix);

                        /*
                        LogHelper.Info<AddTranslationAction>("translationDocType=>" + (translationDocType == null).ToString());
                        LogHelper.Info<AddTranslationAction>("document.ContentType=>" + (document.ContentType == null).ToString());
                        LogHelper.Info<AddTranslationAction>("translationDocType.ParentId=>" + (translationDocType.ParentId).ToString());
                        LogHelper.Info<AddTranslationAction>("document.ContentType.Id=>" + (document.ContentType.Id).ToString());
                        LogHelper.Info<AddTranslationAction>("translationDocType.PropertyTypeExists=>" + translationDocType.PropertyTypeExists(BabelFishCreateTranslation.LanguagePropertyAlias).ToString());
                        */

                        if (
                            translationDocType != null &&
                            document.ContentType != null &&
                            (translationDocType.ParentId == document.ContentType.Id) && 
                            translationDocType.PropertyTypeExists(BabelFishCreateTranslation.LanguagePropertyAlias))
                        {
                            node.Menu.Insert(7, ContextMenuSeperator.Instance);
                            node.Menu.Insert(8, ActionCreateTranslation.Instance);                    
                        }

                        //remove 'create' for 'BabelFishTranslationFolder'
                        if (document.ContentType.Alias == BabelFishCreateTranslation.BabelFishFolderDocTypeAlias)
                        {
                            node.Menu.Remove(ActionNew.Instance);
                        }

                        //remove 'create' for 'Translation' doctype
                        if (document.ContentType.Alias.EndsWith(BabelFishCreateTranslation.PropertySuffix))
                        {
                            node.Menu.Remove(ActionNew.Instance);

                            try
                            {
                                node.Icon = document.GetValue<string>(BabelFishCreateTranslation.LanguagePropertyAlias) + ".png";
                            } 
                            catch {}
                        }
                    }
                    catch (Exception e2)
                    {
                        LogHelper.Error<AddTranslationAction>(e2.Message, e2);
                    }

                    break;
            }
        }
    }
}