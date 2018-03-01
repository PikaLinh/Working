//Sản phẩm
Select2_Custom("/ImportMaster/GetProductId", "ProductId");

//#region Tab SP hiện có
$(document).on("click", "#btnsearchproduct", function () {
        loading2();
        $.ajax({
            type: "POST",
            url: "/InventoryReportChicCut/_ProductPartial",
            data: $("#frmtimproduct").serialize(),
            success: function (data) {
                $("#contentproduct").html(data);
                //recreateDatatable();
                Reinitialize('#dataTable1');
                $("body").removeClass("loading2");
            }
        });
});

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

//#region Export
$(document).on("click", "#btnExport", function () {
    loading2();
    $.ajax({
        type: "POST",
        url: "/InventoryReportChicCut/Export",
        data: $("#frmtimproduct").serialize(),
        success: function (data) {
            var response = data;
            window.location = '/InventoryReportChicCut/Download?fileGuid=' + response.FileGuid
                              + '&filename=' + response.FileName;
            $("body").removeClass("loading2");
        }
    });
});
//#endregion
