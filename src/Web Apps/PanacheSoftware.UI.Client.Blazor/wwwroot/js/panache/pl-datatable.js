$(document).ready(function () {
    $('table.datatable').DataTable({
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
	$('.modal').on('shown.bs.modal', function (e) {
		$(this).find('table.datatable').each(function () {
			var dTable = $(this).DataTable();
			dTable.columns.adjust().draw();
		});
	});
});

//function PLregisterDataTables() {
//    $('table.datatable').DataTable({
//        responsive: {
//            details: {
//                type: 'column'
//            }
//        },
//        columnDefs: [
//            { "orderable": false, "targets": 0 }
//        ],
//        order: [1, 'asc']
//    });
//    $('.modal').on('shown.bs.modal', function (e) {
//        $(this).find('table.datatable').each(function () {
//            var dTable = $(this).DataTable();
//            dTable.columns.adjust().draw();
//        });
//    });
//}