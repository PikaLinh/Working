
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
            url: "/PreImportMaster/Save",
            data: data,
            success: function (data) {
                // $btn.button('reset');
                //$("#tblImportDetail tbody").html(data);
                //$btn.button('reset');
                if (data == "success") {
                    window.location.assign("/PreImportMaster/Index");
                }
                else {
                    $("#divPopup #content").html(data);
                    $("#divPopup").modal("show");
                }
            }
        });
    }
});
