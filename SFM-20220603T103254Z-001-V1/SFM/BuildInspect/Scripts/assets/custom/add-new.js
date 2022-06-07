$(document).ready(function(){
	$( "#addNewItem" ).validate( {
        rules: {
            companyrefno: "required",
            companyname: "required",
            companyaddress: "required",
            companyphone: {
                required: true,
                number: true
            },
            companyemail: {
                required: true,
                email: true
            },
            companylogo: {
                required: true,
                extension: 'jpg|png|gif'
            }
        },
        messages: {
            companyrefno: "Please enter company ref. no.",
            companyname: "Please enter company name",
            companyaddress: "Please enter company address",
            companyphone: "Please enter valid phone no.",
            companyemail: "Please enter valid email address",
            companylogo: {
                required: "Please enter valid image",                  
                extension: "select valid file format"
            }
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