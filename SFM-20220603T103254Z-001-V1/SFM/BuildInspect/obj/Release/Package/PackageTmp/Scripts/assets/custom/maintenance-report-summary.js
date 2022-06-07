$(document).ready(function(){
    $(".maintenance-status-dropdown").select2({
        theme: "bootstrap4",
        placeholder: "Select a Inspection Status",
    });

    $(".maintenance-zone-dropdown").select2({
        theme: "bootstrap4",
        placeholder: "Select a Zone",
    });

    $(".maintenancesubservice-dropdown").select2({
        theme: "bootstrap4",
        placeholder: "Select a Sub Service",
    });

    $(".maintenance-frequency-dropdown").select2({
        theme: "bootstrap4",
        placeholder: "Select a frequency",
    });

    $(".maintenance-school-type-dropdown").select2({
        theme: "bootstrap4",
        placeholder: "Select a School Type",
    });

    $(".maintenance-school-list-dropdown").select2({
        theme: "bootstrap4",
        placeholder: "Select a School",
    });

    $('#maintenance-form-table').DataTable({
        responsive: true
    });

    $('#maintenance-inspection-from-date').datepicker({
        format : "DD-MM-YYYY",
        autoHide: true
    });

    $('#maintenance-inspection-to-date').datepicker({
        format : "DD-MM-YYYY",
        autoHide: true
    });

    $('.maintenance-generate').on('click', function(e){
        $('.maintenance-report-display').fadeIn();
    });

    $('.maintenance-reset').on('click', function(e){
        $('#maintenance-inspection-from-date').val('');
        $('#maintenance-inspection-to-date').val('');
        $(".maintenance-status-dropdown").select2("val", "All");
        $(".maintenance-zone-dropdown").select2("val", "All");
        $(".maintenance-subservice-dropdown").select2("val", "All");
        $(".maintenance-frequency-dropdown").select2("val", "All");
        $(".maintenance-school-type-dropdown").select2("val", "All");
        $(".maintenance-school-list-dropdown").select2("val", "All");
    });
});