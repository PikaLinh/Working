// Bước 3 : Xử lý btnSave
$(document).on("click", "#btnUpdate", function () {
    Updateform();
});


function Updateform() {
    if ($("select[name='ServiceCategoryId']").val() == "" || $("select[name='ServiceCategoryId']").val() == null || $("#Price").val() == "" || $("#QuantificationName").val() == "" || $("select[name='HairTypeId']").val() == "" || $("select[name='HairTypeId']").val() == null) {
        $("#divPopup #content").html("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
        $("#divPopup").modal("show");
        $("body").removeClass("loading2");
    }
    else {
        var data = $("#frmHeader").serialize();
        $.ajax({
            type: "POST",
            url: "/MasterChicCutService/Update",
            data: data,
            success: function (data) {
                if (data == "success") {
                    window.location.assign("/MasterChicCutService/Index");
                }
                else {
                    $("#divPopup #content").html(data);
                    $("#divPopup").modal("show");
                }
            }
        });
    }
}