$(document).ready(function(){
	$(".slick-slider").slick({
        dots: true,
        draggable: true,
        infinite: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        touchThreshold: 1000,
        autoplay: true,
        autoplaySpeed: 3000
    });
	
	$( "#loginForm" ).validate( {
        rules: {
            userPassword: {
                required: true,
                minlength: 5
            },
            userName: {
                required: true
            }
        },
        messages: {
            userPassword: {
                required: "Please provide a password",
                minlength: "Your password must be at least 5 characters long"
            },
            userName: "Please enter a valid user name"
        },
        errorElement: "em",
        errorPlacement: function ( error, element ) {
            // Add the `invalid-feedback` class to the error element
            error.addClass( "invalid-feedback" );
            if ( element.prop( "type" ) === "checkbox" ) {
                error.insertAfter( element.next( "label" ) );
            } else {
                error.insertAfter( element );
            }
        },
        highlight: function ( element, errorClass, validClass ) {
            $( element ).addClass( "is-invalid" ).removeClass( "is-valid" );
        },
        unhighlight: function (element, errorClass, validClass) {
            $( element ).addClass( "is-valid" ).removeClass( "is-invalid" );
        }
    } );
});