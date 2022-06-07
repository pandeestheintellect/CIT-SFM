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

    $(".school-dropdown").select2({
        theme: "bootstrap4",
        placeholder: "Select a school",
    });
    
    $('#checklist-date').datepicker({
        format : "DD-MM-YYYY"
    });

    $('#contractor-datetime').datetimepicker({
      format : "DD-MM-YYYY hh:mm A"
    });

    $('#agent-datetime').datetimepicker({
      format : "DD-MM-YYYY hh:mm A"
    });

    $('#user-datetime').datetimepicker({
      format : "DD-MM-YYYY hh:mm A"
    });

    //Checkby Signature
    var checkbySignature = $('#checkby-signature');
    checkbySignature.jSignature({color:"#00f",lineWidth:3, height: 120});
    checkbySignature.find('.jSignature').css("width", "100%");
    var checkbySignDataPair = checkbySignature.jSignature('getData', 'svgbase64');

    checkbySignature.bind('change', function(e) {
      var checkbyData = checkbySignature.jSignature('getData');
      $('#checkby-signature-capture').val(checkbyData);
    });

    $('.checkby-clear-button').on('click', function(e) {
       e.preventDefault();
       checkbySignature.jSignature('reset');
    });
    
    //Verifiedby Signature
    var verifiedSignature = $('#verified-signature');
    verifiedSignature.jSignature({color:"#00f",lineWidth:3, height: 120});
    verifiedSignature.find('.jSignature').css("width", "100%");
    var verifiedSignDataPair = verifiedSignature.jSignature('getData', 'svgbase64');

    verifiedSignature.bind('change', function(e) {
      var verifiedData = verifiedSignature.jSignature('getData');
      $('#verified-signature-capture').val(verifiedData);
    });

    $('.verified-clear-button').on('click', function(e) {
       e.preventDefault();
       verifiedSignature.jSignature('reset');
    });

    //Endorsed Signature
    var endorsedSignature = $('#endorsed-signature');
    endorsedSignature.jSignature({color:"#00f",lineWidth:3, height: 120});
    endorsedSignature.find('.jSignature').css("width", "100%");
    var endorsedSignDataPair = endorsedSignature.jSignature('getData', 'svgbase64');

    endorsedSignature.bind('change', function(e) {
      var endorsedData = endorsedSignature.jSignature('getData');
      $('#endorsed-signature-capture').val(endorsedData);
    });

    $('.endorsed-clear-button').on('click', function(e) {
       e.preventDefault();
       endorsedSignature.jSignature("reset");
    });

    $('.done-check').on('change', function(){
      if (this.checked) {
        $(this).closest('div.form-check').find('.done-check').not(this).prop('checked', false);
      }
    });

	$( '#checklistform' ).validate({
        rules: {
            schooladdress: "required",
            checklisttel: {
                required: true,
                number: true
            },
            contactperson: "required",
            checklistfax: {
                required: true,
                number: true
            }
        },
        messages: {
            schooladdress: "Please enter address",
            checklisttel: "Please enter valid Tel No.",
            contactperson: "Please enter Contact Name",
            checklistfax: "Please enter valid Fax No."
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