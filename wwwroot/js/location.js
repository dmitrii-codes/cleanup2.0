function getLocation(){
    navigator.geolocation.getCurrentPosition(showPosition, showError);
}
function showPosition(position){
    var lat = position.coords.latitude;
    document.getElementById("Lat").value = lat;
    var lng = position.coords.longitude;
    document.getElementById("Lng").value = lng;
}

function showError(error) {
    switch(error.code) {
        case error.PERMISSION_DENIED:
            console.log("User denied the request for Geolocation.") 
            break;
        case error.POSITION_UNAVAILABLE:
        console.log("Location information is unavailable.") 
            break;
        case error.TIMEOUT:
        console.log("The request to get user location timed out.") 
            break;
        case error.UNKNOWN_ERROR:
        console.log("An unknown error occurred.") 
            break;
    }
}

$(document).ready(function(){
    getLocation();
})