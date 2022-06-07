$(document).ready(function(){
	$(".slick-slider").slick({
        dots: true,
        slidesToShow: 1,
        slidesToScroll: 1
    });

	$( "#registerForm" ).validate( {
        rules: {
            userFullName: "required",
            userPassword: {
                required: true,
                minlength: 5
            },
            userPasswordRep: {
                required: true,
                minlength: 5,
                equalTo: "#userPassword"
            },
            userName: {
                required: true
            },
            userCheck: "required"
        },
        messages: {
            userFullName: "Please enter your firstname",
            userPassword: {
                required: "Please provide a password",
                minlength: "Your password must be at least 5 characters long"
            },
            userPasswordRep: {
                required: "Please provide a password",
                minlength: "Your password must be at least 5 characters long",
                equalTo: "Please enter the same password as above"
            },
            userName: "Please enter a valid user name",
            userCheck: "Please accept our policy"
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