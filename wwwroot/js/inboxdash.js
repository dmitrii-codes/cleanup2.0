$(document).ready(function() {
    $(".open-msg").click(function() {
        $(".msg-window").removeClass("hide-display");
    });
    $("#pend-messages").click(function() {
        $(".msg-window").removeClass("hide-display");
    });
    $("#map").click(function() {
        $(".msg-window").addClass("hide-display");
    });
});