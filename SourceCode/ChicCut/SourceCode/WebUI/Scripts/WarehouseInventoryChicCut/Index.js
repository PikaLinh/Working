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
$(document).ready(function () {
    ////TAB RADIO MANAGEMENT BEGIN ***********************************************************************************
    $("ul.nav-tabs li:first").addClass("active").show(); //Activate first tab
    $(".tab-pane:first").addClass("active").show(); //Show first tab content
});

//#endregion

//#region Huỷ phiếu kiểm kho 
$(document).on("click", ".btn-xoa", function (e) {
    var id = $(this).data("id");
    var code = $(this).data("code");
    $("#idDelete").val(id);
    $(".modal-body strong").html(code);
    //alert(id);
});
$('#confirm-delete').on('click', '.btn-ok', function (e) {
    var $modalDiv = $(e.delegateTarget);
    var id = $("#idDelete").val();
    $.ajax({
        url: '/WarehouseInventoryChicCut/Cancel?id=' + id,
        success: function (data) {
            if (data == "success") {
                window.location.assign("/WarehouseInventoryChicCut/Index")
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

//#region Tạo xuất nhập kho 
$(document).on("click", ".bt-autoimex", function (e) {
    var id = $(this).data("id");
    var code = $(this).data("code");
    $("#idDelete").val(id);
    $(".modal-body strong").html(code);
    //alert(id);
});
$('#confirm-autoimex').on('click', '.btn-ok', function (e) {
    var $modalDiv = $(e.delegateTarget);
    var id = $("#idDelete").val();
    $.ajax({
        url: '/WarehouseInventoryChicCut/Autoimex?id=' + id,
        success: function (data) {
            if (data == "success") {
                window.location.assign("/WarehouseInventoryChicCut/Index")
            }
            else {
                $("#divPopup #content").html(data);
                $("#divPopup").modal("show");
            }
        }
    })
     $.post('/api/record/' + id).then()
    $modalDiv.addClass('loading');
    setTimeout(function () {
        $modalDiv.modal('hide').removeClass('loading');
    }, 1000)
});
//#endregion

//#region Tab Sản Phẩm


//#region Cập nhật tồn sản phẩm
//$(document).on("click", ".btn-update", function (e) {
//    var tontrongdatabase = $(this).data("tontrongdatabase");
//    var id = $(this).data("id");
//    var masterid = $(this).data("row");
//    //alert(tontrongdatabase);
//    $.ajax({
//        url: '/WarehouseInventory/UpdateInventoryProduct?tontrongdatabase=' + tontrongdatabase + "&id=" + id,
//        success: function (data) {
//            if (data == "success") {
//                $.ajax({
//                    type: "POST",
//                    url: "/WarehouseInventory/WarehouseInventoryProducts?id=" + masterid,
//                    success: function (data) {
//                        $("#contentWarehouseInventoryProducts").html(data);
//                        Reinitialize('#datatableproduct');
//                    }
//                });
//            }
//            else {
//                $("#divPopup #content").html(data);
//                $("#divPopup").modal("show");
//            }
//        }
//    })
//});
//#endregion

////#endregion

////#region: khi click SP kiểm kho
//$(document).on("click", "#SPKiemKho", function () {
//    //alert("SPKiemKho");
//    loading2();
//    $.ajax({
//        type: "POST",
//        url: '/WarehouseInventory/WarehouseInventoryProducts?id=0',
//        success: function (data) {
//            $("#contentWarehouseInventoryProducts").html(data);
//            $("body").removeClass("loading2");
//            Reinitialize('#datatableproduct');
//        }
//    });
//});
////#endregion