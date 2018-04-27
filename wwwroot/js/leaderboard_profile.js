$(document).ready(function(){
    $("#scorer-btn").click(function(){
        $(".scores-leaders").removeClass("hide-display");
        $(".token-leaders").addClass("hide-display");
    });
    $("#token-btn").click(function(){
        $(".token-leaders").removeClass("hide-display");
        $(".scores-leaders").addClass("hide-display");
    });
    $("#attending-btn").click(function(){
        $("#attending-div").removeClass("hide-display");
        $("#created-div").addClass("hide-display");
    });
    $("#created-btn").click(function(){
        $("#created-div").removeClass("hide-display");
        $("#attending-div").addClass("hide-display");
    });



});