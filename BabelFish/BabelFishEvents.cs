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


namespace BabelFish
{
    public class AddTranslationAction : ApplicationStartupHandler
    {
        public AddTranslationAction()
        {
            BaseTree.BeforeNodeRender += new BaseTree.BeforeNodeRenderEventHandler(this.BaseTree_BeforeNodeRender);
        }

        private void BaseTree_BeforeNodeRender(ref XmlTree sender, ref XmlTreeNode node, EventArgs e)
        {
            switch (node.NodeType.ToLower())
            {
                case "content":

                    try
                    {
                        //add 'Create Translation' for nodes that have a translation child AND have a 'language' property
                        Document document = new Document(Convert.ToInt32(node.NodeID));
                        
                        DocumentType translationDocType = DocumentType.GetByAlias(document.ContentType.Alias + BabelFishCreateTranslation.PropertySuffix);

                        if (translationDocType != null && (translationDocType.MasterContentType==document.ContentType.Id) && translationDocType.getPropertyType(BabelFishCreateTranslation.LanguagePropertyAlias) != null)
                        {

                            node.Menu.Insert(7, ContextMenuSeperator.Instance);
                            node.Menu.Insert(8, new ActionCreateTranslation());
                            break;                            
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

                            try{
                                node.Icon=document.getProperty(BabelFishCreateTranslation.LanguagePropertyAlias).Value+".png";
                            } catch {}
                        }


                    }
                    catch{

                    }

                    break;
            }
        }
    }
}