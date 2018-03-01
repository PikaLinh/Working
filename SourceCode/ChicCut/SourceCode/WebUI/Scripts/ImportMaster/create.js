
// Nhà cung cấp
Select2_Custom("/ImportMaster/GetSuplierID", "SupplierId");

TotalWeight();


// Bước 3 : Xử lý btnSave
$(document).on("click", "#btnSave", function () {
    //var $btn = $(this).button('loading');
    if ($("select[name='SupplierId']").val() == null) {
        $("#divPopup #content").html("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
        $("#divPopup").modal("show");
    }
    else {
        var data = $("#frmList").serialize() + "&" + $("#frmHeader").serialize();
        $.ajax({
            type: "POST",
            url: "/ImportMaster/Save",
            data: data,
            success: function (data) {
                // $btn.button('reset');
                //$("#tblImportDetail tbody").html(data);
                //$btn.button('reset');
                if (data == "success") {
                    window.location.assign("/ImportMaster/Index");
                }
                else {
                    //alert(data);
                    Alert_Popup(data);
                }
            }
        });
    }
});

// Change scanBarcode
$(document).ready(function () {
    // ProQty
    $("#ProQty").keyup(function (e) {
        console.log(e);
        console.log($(this).val());

        if ($(this).val() == 0) {
            $(this).val(1);
        }
    });

    $("#scanBarcode").keyup(function (e) {
        //var leng = $("#scanBarcode").val().length;
        //if (leng == 8) {
        //$(this).attr("disabled", "disabled")
        if (e.which == 13) {
            //Loại bỏ sự kiện mặc định của enter
            e.preventDefault();
            //alert(leng);
            //Kiem tra ma vach cua san pham
            //1. Ton tai => thuc hien add vao danh sach
            //2. neu khong ton tai => thong bao loi
            //Bước 1 : AddItem
            var data = $("#frmList").serializeArray();
            $.ajax({
                type: "POST",
                url: "/ImportMaster/_CreatelistInner",
                data: data,
                success: function (result) {
                    if (result != "Mã vạch này chưa tồn tại!") {
                        $("#btnAddNewRow").trigger("click");
                    }
                    else {
                        Alert_Popup(result);
                    }
                    //Bước 2 :  $(this).val("");
                    $("#scanBarcode").val("");
                    $("#scanBarcode").focus();
                    $("#ProQty").val(1);
                },
            });
        }
    });
});
