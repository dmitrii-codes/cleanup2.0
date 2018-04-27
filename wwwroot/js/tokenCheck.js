$(document).ready(function(){
    var remainingTokens = document.getElementById("remainingTokens").innerHTML;
    if (remainingTokens <= 0)
    {
        alert("Insufficient tokens to report trash, go and help out more!");
    }
    $("#form").submit(function(event){
        event.preventDefault();
        if (remainingTokens <= 0)
        {
            alert("Insufficient tokens to report trash, go and help out more!");
        }
        else{
            console.log("I got here")
            $("#form").unbind('submit').submit();
        }
    })
})