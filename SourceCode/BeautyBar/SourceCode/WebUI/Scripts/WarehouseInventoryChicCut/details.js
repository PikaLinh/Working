function Reinitialize(datatableName) {
    $(datatableName).DataTable({
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
}
//#region Xoá sản phẩm
$(document).on("click", ".btn-xoa", function (e) {
    var id = $(this).data("id");
    var name = $(this).data("name");
    var masterid = $(this).data("row");
    console.log(masterid);
    $("#idDeleteProduct").val(id);
    $("#masterId").val(masterid);
    $(".modal-body strong").html(name);
    //alert(name);
});

$('#confirm-delete-product').on('click', '.btn-ok', function (e) {
    var $modalDiv = $(e.delegateTarget);
    var id = $("#idDeleteProduct").val();
    var masterid = $("#masterId").val();
    $.ajax({
        url: '/WarehouseInventoryChicCut/DeleteProduct?id=' + id,
        success: function (data) {
            if (data == "success") {
                window.location.assign("/WarehouseInventoryChicCut/WarehouseInventoryProducts?id=" + masterid)
            }
            else {
                $("#divPopup #content").html(data);
                $("#divPopup").modal("show");
            }
        }
    })
    // $.post('/api/record/' + id).then()
    $modalDiv.addClass('loading');
    setTimeout(function () {
        $modalDiv.modal('hide').removeClass('loading');
    }, 1000)
});
//#endregion

//#region Cập nhật tồn sản phẩm
$(document).on("click", ".btn-update", function (e) {
    var tontrongdatabase = $(this).data("tontrongdatabase");
    var id = $(this).data("id");
    var masterid = $(this).data("row");
    //alert(tontrongdatabase);
    $.ajax({
        url: '/WarehouseInventoryChicCut/UpdateInventoryProduct?tontrongdatabase=' + tontrongdatabase + "&id=" + id,
        success: function (data) {
            if (data == "success") {
                $.ajax({
                    type: "POST",
                    url: "/WarehouseInventory/WarehouseInventoryProducts?id=" + masterid,
                    success: function (data) {
                        $("#contentWarehouseInventoryProducts").html(data);
                        Reinitialize('#datatableproduct');
                    }
                });
            }
            else {
                $("#divPopup #content").html(data);
                $("#divPopup").modal("show");
            }
        }
    })
});
//#endregion