
// Bước 3 : Xử lý btnSave
$(document).on("click", "#btnSave", function () {
    var data = $("#frmList").serialize() + "&" + $("#frmHeader").serialize();
    $.ajax({
        type: "POST",
        url: "/WarehouseInventoryChicCut/Save",
        data: data,
        success: function (data) {
            if (data == "success") {
                window.location.assign("/WarehouseInventoryChicCut/Index");
            }
            else {
                Alert_Popup(data);
            }
        }
    });
});

// Change scanBarcode
$(document).ready(function () {
    $("#scanBarcode").keypress(function (e) {
        if (e.which == 13) {
            //Loại bỏ sự kiện mặc định của enter
            e.preventDefault();
            //$("#btnAddNewRow").trigger("click");
            ////return luôn để nó đừng post lên
            //return false;

            //Bước 1: Kiem tra ma vach cua san pham
            //1.1. Ton tai => thuc hien add vao danh sach
            //1.2. neu khong ton tai => thong bao loi
            //Bước 2:
            //Nhấn nút add
            //AddItem
            var data = $("#frmList").serializeArray();
            $.ajax({
                type: "POST",
                url: "/WarehouseInventoryChicCut/_CreateDetailListInner",
                data: data,
                success: function (result) {
                    if (result != "Mã vạch này chưa tồn tại!" && result != "Mã vạch của sản phẩm này đã tồn tại trong danh sách kiểm kho!") {
                        $("#btnAddNewRow").trigger("click");
                    }
                    else if (result == "Mã vạch này chưa tồn tại!") {
                        Alert_Popup(result);
                    }
                    else {
                        Alert_Popup(result);
                    }
                    //Để textbox trở lại trạng thái rỗng
                    $("#scanBarcode").val("");
                    $("#scanBarcode").focus();
                    $("#ProQty").val(1);
                    
                },
            });

        }
    });
});


$("select[name='CategoryId']").on("change", function (e) {
    var cate = $("select[name='CategoryId']").val();
    $.ajax({
        type: "POST",
        url: "/WarehouseInventoryChicCut/_CreateDetailListInnerCategory?id=" + cate,
        success: function (data) {
            $("#tblWarehouseInventoryDetail tbody").html(data);
            Total();
        }
    });
});

