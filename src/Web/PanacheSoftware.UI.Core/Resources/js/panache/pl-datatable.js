$(document).ready(function () {
    $('.datatable').DataTable({
        responsive: {
            details: {
                type: 'column'
            }
        },
        columnDefs: [
            { "orderable": false, "targets": 0 }
        ],
        order: [1, 'asc']
    });
});