$(document).ready(function(){
    $('#electrical-servicelist-form-table').DataTable({
        responsive: true,
        dom: 'Bfrtip',
        buttons: [
            'copy', 'csv', 'excel', 'pdf', 'print'
        ]
    });

    $('#mechanical-servicelist-form-table').DataTable({
        responsive: true
    });

    $('#specialist-servicelist-form-table').DataTable({
        responsive: true
    });

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        $($.fn.dataTable.tables(true)).DataTable().columns.adjust().responsive.recalc();
    });
});