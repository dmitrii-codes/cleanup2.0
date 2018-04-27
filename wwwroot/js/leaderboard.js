$(document).ready(function(){
    $("#scorer-btn").click(function(){
        $(".scores-leaders").removeClass("hide-display");
        $(".token-leaders").addClass("hide-display");
    });
    $("#token-btn").click(function(){
        $(".token-leaders").removeClass("hide-display");
        $(".scores-leaders").addClass("hide-display");
    });


});