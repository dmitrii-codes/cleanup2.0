function getLocation(){
    navigator.geolocation.getCurrentPosition(showUserPosition);
}
function showUserPosition(position){
    var lat = position.coords.latitude;
    document.getElementById("Lat").value = lat;
    var lng = position.coords.longitude;
    document.getElementById("Lng").value = lng;
}

$(document).ready(function(){
    getLocation();
})