$(document).ready(function(){
    var remainingTokens = document.getElementById("remainingTokens").innerHTML;
    $('#newcleanup').submit(function(event) {
        event.preventDefault();
        if (remainingTokens <= 0)
        {
            alert("Insufficient tokens to report trash, go and help out more!");
        }
        else{
            var address = document.getElementById('address').value;
            if (address.length > 0){
                $.get("https://maps.googleapis.com/maps/api/geocode/json?address=" + address.split(' ').join('+') + "&key=AIzaSyBDTyblNr_Q6boR90WUWLlbhPVlc2u_6h8", function(response){
                if (response["status"]=="OK"){
                    $('#Lat').val(response["results"][0]["geometry"]["location"]["lat"]);
                    $('#Lng').val(response["results"][0]["geometry"]["location"]["lng"]);
                    $('#newcleanup').unbind('submit').submit(); // continue
                }
                else{
                    alert('Geocode was not successful for the following reason: ' + status);
                    $('#newcleanup').unbind('submit').submit(); // continue
                }
                });
            }
            else{
                $.get("https://maps.googleapis.com/maps/api/geocode/json?latlng=" + document.getElementById('Lat').value + "," + document.getElementById('Lng').value + "&key=AIzaSyBDTyblNr_Q6boR90WUWLlbhPVlc2u_6h8", function(response){
                    if (response["status"]=="OK"){
                        $('#address').val(response["results"][0]["formatted_address"]);
                        $('#newcleanup').unbind('submit').submit(); // continue
                    }
                    else{
                        alert('Geocode was not successful for the following reason: ' + status);
                        $('#newcleanup').unbind('submit').submit(); // continue
                    }
                });
            }
        }
    });
});