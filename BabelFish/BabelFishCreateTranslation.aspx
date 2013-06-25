<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BabelFishCreateTranslation.aspx.cs" Inherits="BabelFish.BabelFishCreateTranslation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head >
        <link rel="stylesheet" type="text/css" href="/umbraco/plugins/BabelFish/BabelFishCreateTranslation.css" />
        
        
        <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.9.0/jquery.min.js" type="text/javascript"></script>
        <script src="/umbraco/plugins/BabelFish/BabelFishCreateTranslation.js"></script>
        
        <script src="/umbraco_client/Application/NamespaceManager.js"></script>
        <script src="/umbraco_client/Application/UmbracoClientManager.js"></script>
    </head>
<body>
    <form id="form1" runat="server">
        <h1>Select language(s):</h1>
        <div id="wrapperDiv" class="wrapperDiv" runat="server"></div>

        <div id="footer" class="footer" runat="server">
            <input id="createButton" class="createButton" type="button" value="Create"/>
        </div>
    </form>
</body>
</html>
