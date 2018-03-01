

// Bước 3 : Xử lý btnSave
$(document).on("click", "#btnSave", function () {
    //SubmitForm("Save");
    Saveform("None");
});
$(document).on("click", "#btnSaveAndRedirectToURL", function () {
    //SubmitForm("Save");
    Saveform("RedirectToURL");
});

function Saveform(returnURL) {
    if ($("select[name='ServiceCategoryId']").val() == "" || $("select[name='ServiceCategoryId']").val() == null || $("#Price").val() == "" || $("#QuantificationName").val() == "" || $("select[name='HairTypeId']").val() == "" || $("select[name='HairTypeId']").val() == null) {
        $("#divPopup #content").html("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
        $("#divPopup").modal("show");
        $("body").removeClass("loading2");
    }
    else {
        var data = $("#frmHeader").serialize();
        $.ajax({
            type: "POST",
            url: "/MasterChicCutService/Save?returnURL=" + returnURL,
            data: data,
            success: function (data) {
                if (data == "success") {
                    window.location.assign("/MasterChicCutService/Index");
                }
                else if (data == "ErrorInfo") {
                    $("#divPopup #content").html("Vui lòng kiểm tra lại thông tin không hợp lệ !");
                    $("#divPopup").modal("show");
                }
                else if (data == "ErrorOccur") {
                    $("#divPopup #content").html("Xảy ra lỗi trong quá trình thêm mới dịch vụ !");
                    $("#divPopup").modal("show");
                }
                else{
                    window.location.assign(data);
                }
            }
        });
    }
}