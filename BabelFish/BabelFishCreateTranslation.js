$(function(){
    
    $(".createButton").click(function(){
        
        if($("input[type=checkbox]:checked").length>0){
            $("form").submit();
        }
        
    });
});