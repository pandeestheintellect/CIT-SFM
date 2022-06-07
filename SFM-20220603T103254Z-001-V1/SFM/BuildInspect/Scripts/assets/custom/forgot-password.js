$(document).ready(function(){
	$(".slick-slider").slick({
        dots: true,
        slidesToShow: 1,
        slidesToScroll: 1
    });
	
	$( "#forgotPassword" ).validate( {
        rules: {
            userName: {
                required: true
            }
        },
        messages: {
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