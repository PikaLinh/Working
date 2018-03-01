// Bước 1 xử lý thêm dòng mới trong CreateList
$("#btnAddNewRow").unbind("click").click(function () {
    var data = $("#frmList").serializeArray();
    $.ajax({
        type: "POST",
        url: "/CRMSMSTemplate/_CreatelistInner",
        data: data,
        success: function (data) {
            $("#tblSMSPara tbody").html(data);
        }
    });
});
// Bước 2 xử lý detail-btndelete
$(document).on("click", ".detail-btndelete", function () {
    var removeId = $(this).data("row");
    var EmailParameterId = $("input[name='detail[" + (removeId - 1) + "].SMSParameterId']").val();
    var CheckReferenced = $("#CheckReferenced").val();
    //TH1 : Đang bị tham chiếu tới và có EmailParameterId : Thông báo Những tham số đánh dấu STT màu đỏ chỉ được sửa (không xoá) + lên controller : Sửa và thêm
    if ((EmailParameterId != 0) && CheckReferenced == "true") {
        $("#divPopup #content").html("Những tham số đánh dấu <span class=\"color-red\">STT màu đỏ </span> chỉ được sửa (không xoá)");
        $("#divPopup").modal("show");
    }
    else { //TH2 : Không bị tham chiếu tới : Xoá list trên giao diện + lên coltroller : Xoá (duyệt những para trong db không có trong list), Sửa, Thêm
        var data = $("#frmList").serializeArray();
        var removeId = $(this).data("row");
        $.ajax({
            type: "POST",
            url: "/CRMSMSTemplate/_DeletelistInner?RemoveId=" + removeId,
            data: data,
            success: function (data) {
                $("#tblSMSPara tbody").html(data);
            }
        });
    }
});

// Bước 3 : Xử lý button Save
function SubmitForm(action) {
    loading2();
    var CheckEmailparaEmpty = false;
    $(".detail-Name").each(function () {
        if ($(this).val() == "") {
            CheckEmailparaEmpty = true;
        }
    });
    if ($("#SMSTemplateName").val() == "" || $("#SMSContent").val() == "" || CheckEmailparaEmpty == true) {
        $("#divPopup #content").html("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
        $("#divPopup").modal("show");
        $("body").removeClass("loading2");
    }
    else {
        for (instance in CKEDITOR.instances) {
            CKEDITOR.instances[instance].updateElement();
        }
        var data = $("#frmList").serialize() + "&" + $("#frmHeader").serialize();
        $.ajax({
            type: "POST",
            url: "/CRMSMSTemplate/" + action,
            data: data,
            success: function (data) {
                if (data == "success") {
                    window.location.assign("/CRMSMSTemplate/Index");
                }
                else {
                    $("#divPopup #content").html(data);
                    $("#divPopup").modal("show");
                }
            }
        });
    }
}