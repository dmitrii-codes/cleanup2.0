$(document).ready(function() {
    var remainingTokens = document.getElementById("remainingTokens").innerHTML;
    if (remainingTokens <= 0) {
        alert("Insufficient number of tokens to report trash, go and help out more!");
    }
    $("#form").submit(function(event) {
        event.preventDefault();
        if (remainingTokens <= 0) {
            alert("Insufficient number of tokens to report trash, go and help out more!");
        } else {
            $("#form").unbind('submit').submit();
        }
    })
})