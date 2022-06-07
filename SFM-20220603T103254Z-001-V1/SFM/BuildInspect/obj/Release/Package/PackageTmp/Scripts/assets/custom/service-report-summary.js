$(document).ready(function(){
    $(".service-status-dropdown").select2({
        theme: "bootstrap4",
        placeholder: "Select a Inspection Status",
    });

    $(".service-zone-dropdown").select2({
        theme: "bootstrap4",
        placeholder: "Select a Zone",
    });

    $(".service-subservice-dropdown").select2({
        theme: "bootstrap4",
        placeholder: "Select a Sub Service",
    });

    $(".service-frequency-dropdown").select2({
        theme: "bootstrap4",
        placeholder: "Select a frequency",
    });

    $(".service-school-type-dropdown").select2({
        theme: "bootstrap4",
        placeholder: "Select a School Type",
    });

    $(".service-school-list-dropdown").select2({
        theme: "bootstrap4",
        placeholder: "Select a School",
    });

    $('#servicelist-form-table').DataTable({
        responsive: true
    });

    $('#service-inspection-from-date').datepicker({
        format : "DD-MM-YYYY",
        autoHide: true
    });

    $('#service-inspection-to-date').datepicker({
        format : "DD-MM-YYYY",
        autoHide: true
    });

    $('.servicelist-generate').on('click', function(e){
        $('.summary-report-display').fadeIn();
    });

    $('.servicelist-reset').on('click', function(e){
        $('#service-inspection-from-date').val('');
        $('#service-inspection-to-date').val('');
        $(".service-status-dropdown").select2("val", "All");
        $(".service-zone-dropdown").select2("val", "All");
        $(".service-subservice-dropdown").select2("val", "All");
        $(".service-frequency-dropdown").select2("val", "All");
        $(".service-school-type-dropdown").select2("val", "All");
        $(".service-school-list-dropdown").select2("val", "All");
    });
});