function BabelFishCreateTranslation(){
    var nodeID = UmbClientMgr.mainTree().getActionNode().nodeId;
    
    if(nodeID != null){
        UmbClientMgr.openModalWindow('/umbraco/plugins/BabelFish/BabelFishCreateTranslation.aspx?nodeId=' + nodeID, "Babel Fish Create Translation", true, 380, 380, '','','', finished);
    }
}

function finished(){

}