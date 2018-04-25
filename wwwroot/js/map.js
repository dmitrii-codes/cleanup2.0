function initMap() {
    var map = new google.maps.Map(document.getElementById('map'), {
        //initial location and zoom
        center: { lat: parseFloat("47.617515"), lng: parseFloat("-122.201853") },
        zoom: 10,
        styles: mapStyle,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        disableDefaultUI: true
    });
    //infoWindow init
    map.addListener('click', function(){
        infoWindow.close();
    });
    const infoWindow = new google.maps.InfoWindow();
    infoWindow.setOptions({ pixelOffset: new google.maps.Size(0, -30) })
    //markers init
    var locations = []
    var ids = []
    for (var i = 0; i < 2; i++) { //iterate through activities
            locations.push({ lat: parseFloat($("#markerLng_" + i).html()), lng: parseFloat($("#markerLat_" + i).html()) })
            ids.push(i)
    }
    //markers creation
    var markers = locations.map(function (location, id) {
        var markers_id = ids[id % ids.length]
        var marker = new google.maps.Marker({
            position: location,
            icon: "https://image.ibb.co/bsqxwx/pin.png",
            title: $("#markerTitle_" + markers_id).html()
        });
        //show the info when clicked
        marker.addListener('click', function() {
        const content = $("#markerContent_" + markers_id).html()
        infoWindow.setContent(content);
        infoWindow.setPosition(location);
        infoWindow.open(map);
        });
        return marker
    });
    //clusters
    var markerCluster = new MarkerClusterer(map, markers, { imagePath: 'https://cdn.rawgit.com/googlemaps/js-marker-clusterer/gh-pages/images/m' });
    //infoWindow styling
    infoWindow.addListener('domready', function() {
        // Reference to the DIV that wraps the bottom of infowindow
        var iwOuter = $('.gm-style-iw');
        /* Since this div is in a position prior to .gm-div style-iw.
         * We use jQuery and create a iwBackground variable,
         * and took advantage of the existing reference .gm-style-iw for the previous div with .prev().
        */
        var iwBackground = iwOuter.prev();
        // Removes background shadow DIV
        iwBackground.children(':nth-child(2)').css({'display' : 'none'});
        // Removes white background DIV
        iwBackground.children(':nth-child(4)').css({'display' : 'none'});
        // Changes the desired tail shadow color.
        iwBackground.children(':nth-child(3)').find('div').children().css({'box-shadow': 'rgba(72, 181, 233, 0.6) 0px 1px 6px', 'z-index' : '1'});
        // Reference to the div that groups the close button elements.
        var iwCloseBtn = iwOuter.next();
        // Apply the desired effect to the close button
        iwCloseBtn.css({opacity: '1', right: '38px', top: '3px', border: '7px solid #b8d18c', 'border-radius': '14px', 'box-shadow': '0 0 5px black'});
        // If the content of infowindow not exceed the set maximum height, then the gradient is removed.
        // console.log($('.iw-content').height())
        // if($('.iw-content').height() < 140){
        //   $('.iw-bottom-gradient').css({display: 'none'});
        // }
        // // The API automatically applies 0.7 opacity to the button after the mouseout event. This function reverses this event to the desired value.
        iwCloseBtn.mouseout(function(){
          $(this).css({opacity: '1'});
        });
    });
}
// Style credit: https://snazzymaps.com/style/47/nature
const mapStyle = [
{
    "featureType": "landscape",
    "stylers": [
        {
            "hue": "#FFA800"
        },
        {
            "saturation": 0
        },
        {
            "lightness": 0
        },
        {
            "gamma": 1
        }
    ]
},
{
    "featureType": "road.highway",
    "stylers": [
        {
            "hue": "#53FF00"
        },
        {
            "saturation": -73
        },
        {
            "lightness": 40
        },
        {
            "gamma": 1
        }
    ]
},
{
    "featureType": "road.arterial",
    "stylers": [
        {
            "hue": "#FBFF00"
        },
        {
            "saturation": 0
        },
        {
            "lightness": 0
        },
        {
            "gamma": 1
        }
    ]
},
{
    "featureType": "road.local",
    "stylers": [
        {
            "hue": "#00FFFD"
        },
        {
            "saturation": 0
        },
        {
            "lightness": 30
        },
        {
            "gamma": 1
        }
    ]
},
{
    "featureType": "water",
    "stylers": [
        {
            "hue": "#00BFFF"
        },
        {
            "saturation": 6
        },
        {
            "lightness": 8
        },
        {
            "gamma": 1
        }
    ]
},
{
    "featureType": "poi",
    "stylers": [
        {
            "hue": "#679714"
        },
        {
            "saturation": 33.4
        },
        {
            "lightness": -25.4
        },
        {
            "gamma": 1
        }
    ]
},
{ 
    "featureType": "poi", 
    "elementType": "labels", 
    "stylers": 
    [ 
        { 
            "visibility": "off" 
        } 
    ] 
},
{ 
    "featureType": "landscape", 
    "elementType": "labels", 
    "stylers": 
    [ 
        { 
            "visibility": "off" 
        } 
    ] 
}, 
{ 
    "featureType": "transit", 
    "elementType": "labels", 
    "stylers": 
    [ 
        { 
            "visibility": "off" 
        } 
    ] 
}
]