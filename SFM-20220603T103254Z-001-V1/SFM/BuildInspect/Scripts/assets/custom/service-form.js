$(document).ready(function(){
    $.extend(true, $.fn.datetimepicker.defaults, {
        icons: {
            time: 'fas fa-clock',
            date: 'fas fa-calendar',
            up: 'fas fa-arrow-up',
            down: 'fas fa-arrow-down',
            previous: 'fas fa-chevron-left',
            next: 'fas fa-chevron-right',
            today: 'fas fa-calendar-check',
            clear: 'fas fa-trash-alt',
            close: 'fas fa-times-circle'
        }
    });

    $('.priority-dropdown').select2({
        theme: "bootstrap4",
        placeholder: "Select a priority",
    });
    
    $('#service-date').datepicker({
        format : "DD-MM-YYYY"
    });

    $('#technician-datetime').datetimepicker({
        format : "DD-MMM-YYYY hh:mm A"
    });

    //Service Signature
    var serviceSignature = $('#service-signature');
    serviceSignature.jSignature({color:"#00f",lineWidth:3, height: 120});
    var serviceSignDataPair = serviceSignature.jSignature('getData', 'svgbase64');

    serviceSignature.bind('change', function(e) {
      var serviceData = serviceSignature.jSignature('getData');
      $('#service-signature-capture').val(serviceData);
    });

    $('.service-clear-button').on('click', function(e) {
        e.preventDefault();
       serviceSignature.jSignature('reset');
    });

    //Technician Signature
    var technicianSignature = $('#technician-signature');
    technicianSignature.jSignature({color:"#00f",lineWidth:3, height: 120});
    technicianSignature.find('.jSignature').css("width", "100%");
    var technicianSignDataPair = technicianSignature.jSignature('getData', 'svgbase64');

    technicianSignature.bind('change', function(e) {
      var technicianData = technicianSignature.jSignature('getData');
      $('#technician-signature-capture').val(technicianData);
    });

    $('.technician-clear-button').on('click', function(e) {
        e.preventDefault();
       technicianSignature.jSignature('reset');
    });

    //Customer Signature
    var customerSignature = $('#customer-signature');
    customerSignature.jSignature({color:"#00f",lineWidth:3, height: 120});
    customerSignature.find('.jSignature').css("width", "100%");
    var customerSignDataPair = customerSignature.jSignature('getData', 'svgbase64');

    customerSignature.bind('change', function(e) {
      var customerData = customerSignature.jSignature('getData');
      $('#customer-signature-capture').val(customerData);
    });
    
    $('.technician-clear-button').on('click', function(e) {
        e.preventDefault();
       customerSignature.jSignature('reset');
    });


	$( '#serviceform' ).validate({
        rules: {
            caserefno: "required",
            servicedatetime: "required",
            customername: "required",
            customeraddress: "required",
            personcontact: "required",
            contactlocation: "required",
            contactroomno: {
                required: true,
                number: true
            },
            workrequests: "required",
            personname: "required",
            workcompletion: "required",
            workremarks: "required",
            clientcomments: "required"
        },
        messages: {
            caserefno: "Please enter Case Ref No.",
            servicedatetime: "Please enter Date & Time",
            customername: "Please enter Customer Name",
            customeraddress: "Please enter Customer Address",
            personcontact: "Please enter Customer Contact",
            contactlocation: "Please enter Contact Location",
            contactroomno: "Please enter valid Room No.",
            workrequests: "Please enter Work Request",
            personname: "Please enter Person Name",
            workcompletion: "Please fill in Work Completion",
            workremarks: "Please fill in Work Remarks",
            clientcomments: "Please fill in Client Comments"
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
    });


});