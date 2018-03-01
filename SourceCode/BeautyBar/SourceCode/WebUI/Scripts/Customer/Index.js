$(document).on("click", "#btntim", function () {
    $.ajax({
        url: '/Customer/_SearchCustomer',
        data: $("#formload").serialize(),
        success: function (data) {
            $("#contentCustomer").html(data);

            $('.dataTable').DataTable({
                language: {
                    sProcessing: "Đang xử lý...",
                    sLengthMenu: "Xem _MENU_ mục",
                    sZeroRecords: "Không tìm thấy dữ liệu",
                    sInfo: "Đang xem _START_ đến _END_ trong tổng số _TOTAL_ mục",
                    sInfoEmpty: "Đang xem 0 đến 0 trong tổng số 0 mục",
                    sInfoFiltered: "(được lọc từ _MAX_ mục)",
                    sInfoPostFix: "",
                    sSearch: "Tìm nội dung:",
                    sUrl: "",
                    oPaginate: {
                        sFirst: "Đầu",
                        sPrevious: "&laquo;",
                        sNext: "&raquo;",
                        sLast: "Cuối"
                    },
                    columnDefs: [
                    { targets: [0, 1], visible: true },
                    { targets: 'no-sort', visible: false }
                    ]
                },
                "bLengthChange": false,
                "bInfo": false,
                //"bPaginate" : false,
                "sDom": '<"top"flp>rt<"bottom"i><"clear">',
            });

            $(".dataTables_filter").prepend("<span class='search-icon'></span>");
            $(".dataTables_filter").append($(".has-btn-add-new").html());
            $(".has-btn-add-new").html("");
        }
    })
});