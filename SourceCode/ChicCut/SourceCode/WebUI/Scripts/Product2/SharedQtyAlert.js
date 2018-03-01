$(".QtyAlertList-QtyAlert").number(true);
LoadGridContentQtyAlert();
// Bước 1 xử lý thêm dòng mới trong CreateList
$(document).on("click", "#btnAddNewRowQtyAlert", function () {
    loading2();
    var data = $("#frmListQtyAlert").serializeArray();
    $.ajax({
        type: "POST",
        url: "/Product/_QtyAlertListInner",
        data: data,
        success: function (data) {
            $("#tblQtyAlertList tbody").html(data);
            $(".QtyAlertList-QtyAlert").number(true);
        }
    });
});


// Bước 2 xử lý detail-btndelete
$(document).on("click", ".detail-btndeleteQtyAlert", function () {
    //var $btn = $(this).button('loading');
    loading2();
    var data = $("#frmListQtyAlert").serializeArray();
    var removeId = $(this).data("row");
    $.ajax({
        type: "POST",
        url: "/Product/_DeletelistInnerQtyAlertList?RemoveId=" + removeId,
        data: data,
        success: function (data) {
            //$btn.button('reset');
            $("#tblQtyAlertList tbody").html(data);
            $(".QtyAlertList-QtyAlert").number(true);
        }
    });
    return false;
});

//#region Xử lý khi Huỷ/ Lưu
//#region Huỷ: load giá trị ban đầu
$(document).on("click", ".btn-cancel-QtyAlert", function () {
    loading2();
    $.ajax({
        type: "POST",
        url: "/Product/_QtyAlertList?ProductId=" + $("#ProductId").val(),
        success: function (data) {
            $("#QtyAlertList .modal-body").html(data);
            LoadGridContentQtyAlert();
            $("#QtyAlertError").html("");
        }
    });
});
//#endregion 

//#region Lưu:: Xét trùng kho,Số lượng > 0 và Giữ giá trị hiện tại
$(document).on("click", ".btn-save-QtyAlert", function () {
    loading2();
    var QtyAlert = 0;
    var ItemToCompare = "";
    var row = 0, rowInner = 0;
    var Mesg = "OK";
    $("select[name='Warehouse']").each(function () {
        row = $(this).data("row");
        ItemToCompare = $(this).val();
        $("input[name='QtyAlertList[" + row + "].WarehouseId']").val(ItemToCompare);// Gán giá trị cho drop load lần đầu chưa set value cho field hidden
        $("input[name='QtyAlertList[" + row + "].WarehouseName']").val($(this).find("option:selected").text());
        $("input[name='QtyAlertList[" + row + "].RolesId']").val($(".Roles_" + row).val());

        QtyAlert = $("input[name='QtyAlertList[" + row + "].QtyAlert']").val();
        if ((QtyAlert == "" || QtyAlert <= 0)) {
            Mesg = "'Số lượng' phải lớn hơn 0";
            return true;
        }
        //console.log(ItemToCompare);
        $("select[name='Warehouse']").each(function () {
            rowInner = $(this).data("row");
            if (rowInner != row) {
                if (ItemToCompare == $(this).val()) {
                    Mesg = "'Kho' không được trùng";
                    return true;
                }
            }
        });
       
    });
    if (Mesg != "OK") {//Thông báo lỗi
        $("#QtyAlertError").html(Mesg);
    }
    else {//Lưu thành công
        LoadGridContentQtyAlert();
        $("#QtyAlertError").html("");
        $("#QtyAlertList").modal("hide");
    }
    $("body").removeClass("loading2");
});
//#endregion

function LoadGridContentQtyAlert() {
    var lst = "";
    var Qty;
    $("select[name='Warehouse']").each(function () {
        Qty = $("input[name='QtyAlertList[" + $(this).data("row") + "].QtyAlert']").val().toString().replace(/\B(?=(?:\d{3})+(?!\d))/g, ",");
        lst += $(this).find("option:selected").text() + ": " + Qty + " <br /> ";
    });
    var DisplayClick = "";
    if (lst != "") {//duyệt có giá trị
        DisplayClick = " Nhấn <a style=\"cursor:pointer\" id=\"SettingQtyAlert\"><u>vào đây</u></a> để cài đặt cảnh báo";
    }
    else {
        DisplayClick = "Chưa cài đặt cảnh báo tồn kho <br> Nhấn <a style=\"cursor:pointer\" id=\"SettingQtyAlert\"><u>vào đây</u></a> để cài đặt cảnh báo";
    }
    $("#GridContentQtyAlert").html(lst + DisplayClick);
}
//#endregion