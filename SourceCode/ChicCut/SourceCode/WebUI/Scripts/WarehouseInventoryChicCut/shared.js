function GetData() {
    var data = $("#frmList").serializeArray();
    return data;
}

// Bước 1 xử lý thêm dòng mới trong CreateList
$("#btnAddNewRow").unbind("click").click(function () {
    var data = GetData();
    $.ajax({
        type: "POST",
        data: data,
        url: "/WarehouseInventoryChicCut/_CreateDetailListInner",
        success: function (data) {
            $("#tblWarehouseInventoryDetail tbody").html(data);
            $("select[name='ProductId']").trigger("change");
        }
    });
});

// Bước 2 xử lý detail-btndelete
$(document).on("click", ".detail-btndelete", function () {

    var data = GetData();
    var removeId = $(this).data("row");
    $.ajax({
        type: "POST",
        url: "/WarehouseInventoryChicCut/_DeletelistInner?RemoveId=" + removeId,
        data: data,
        success: function (data) {
            $("#tblWarehouseInventoryDetail tbody").html(data);
            Total();
        }
    });
});

