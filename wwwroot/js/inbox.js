$(document).ready(function() {
    $(".open-msg").click(function() {
        $(".msg-window").removeClass("hide-display");
    });
    $("#pend-messages").click(function() {
        $(".msg-window").removeClass("hide-display");
    });
    $(".user-data").click(function() {
        $(".msg-window").addClass("hide-display");
    });
});