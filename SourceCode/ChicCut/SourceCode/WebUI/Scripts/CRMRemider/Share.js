$("#Price").number(true);
// Khách hàng
Select2_Custom("/CashReceiptVoucher/GetCustomerID", "CustomerId");

// Nhà cung cấp
Select2_Custom("/ImportMaster/GetSuplierID", "SupplierId");

//#region Đối tượng
function ObjectVisible(ObjectName) {
    $(".Objectvisible").hide();
    $(".ObjectValue").empty().trigger('change');
    if (ObjectName == 'KH') {
        $("#divCustomerId2").show();
    }
    else {
        $("#divSupplierId2").show();
    }
}

$("input[name='ObjectId']").on("change", function () {
    var ObjectName = $(this).attr("id");
    //alert(ObjectName);
    ObjectVisible(ObjectName);
});
//#endregion

//#region Visible Tần suất
function PeriodTypeVisible(PeriodType) {
    $(".PeriodTypeVisible").hide();
    if (PeriodType == "MOTLAN") {
        $(".PeriodTypeMOTLAN").show();
    }
    else {
        $(".PeriodTypeDINHKY").show();
    }
}
$("input[name='PeriodType']").on("change", function () {
    var PeriodTypeId = $(this).attr("id");
    PeriodTypeVisible(PeriodTypeId);
});

//#endregion

//#region Visible NDays
function NDaysVisible() {
    if ($("#PeriodCode").val() == "NNgay")
    {
        $(".NDaysVisible").show();
    }
    else
    {
        $(".NDaysVisible").hide();
        $("#NDays").val("");
    }
}
$(document).on("change", "#PeriodCode", function () {
    NDaysVisible();
    GetNextDateReminder();
})

//#endregion

//#region Visible Mẫu Email
function EmailTemplateVisible()
{
    if ($("#isEmailNotified").prop("checked")) {
        $(".EmailTemplateVisible").show();
        $(".CCEmailVisible").show();
        $("#btnPreviewEmail").css("display", "inline-block");
    }
    else
    {
        $(".EmailTemplateVisible").hide();
        $(".CCEmailVisible").hide();
        // Bỏ mẫu , CCEmail, DivPara
        $("#EmailTemplateId option").removeAttr("selected");
        GetHrefEmailTemplate(0);
        $("#isCCEmail").removeAttr("checked");
        VisibleEmailOfEmployee();
        $("#divEmailParameter").css("display", "none");
        $("#EmailParaContent").html("");
        $("#btnPreviewEmail").css("display", "none");
    }
}

$(document).on("change", "#isEmailNotified", function () {
    EmailTemplateVisible();
})

//#endregion

//#region Visible Mẫu SMS
function SMSTemplateVisible() {
    if ($("#isSMSNotifred").prop("checked")) {
        $(".SMSTemplateVisible").show();
        $(".CCSMSVisible").show();
        $("#btnPreviewSMS").css("display", "inline-block");
    }
    else {
        $(".SMSTemplateVisible").hide();
        $(".CCSMSVisible").hide();
        // Bỏ mẫu , CCSMS, DivPara
        $("#SMSTemplateId option").removeAttr("selected");
        GetHrefSMSTemplate(0);
        $("#isCCSMS").removeAttr("checked"); 
        VisibleSMSOfEmployee();
        $("#divSMSParameter").css("display", "none");
        $("#SMSParaContent").html("");
        $("#btnPreviewSMS").css("display", "none");
    }
}

$(document).on("change", "#isSMSNotifred", function () {
    SMSTemplateVisible();

})
//#endregion

//#region Xem chi tiết template
function GetHrefEmailTemplate(id) {
    $("#ViewEmailTemplate").attr("href", "/CRMEmailTemplate/Details/" + id);
}
function GetHrefSMSTemplate(id) {
    $("#ViewSMSTemplate").attr("href", "/CRMSMSTemplate/Details/" + id);
}

//#endregion

//#region Thay đổi Tempalte => Lấy danh sách Para.
$(document).on("change", "#EmailTemplateId", function () {
    GetHrefEmailTemplate($(this).val());
    // Hiển thị danh sách tham số tương ứng
   $.ajax({
        type: 'POST',
        url: "/CRMRemider/_EmailParameter?EmailTemplateId=" + $(this).val(),
        success: function (data) {
            $("#EmailParaContent").html(data);
            VisibleEmailPara();
        }
   });
  
});
$(document).on("change", "#SMSTemplateId", function () {
    GetHrefSMSTemplate($(this).val());
    // Hiển thị danh sách tham số tương ứng
    $.ajax({
        type: 'POST',
        url: "/CRMRemider/_SMSParameter?SMSTemplateId=" + $(this).val(),
        success: function (data) {
            $("#SMSParaContent").html(data);
            VisibleSMSPara();
        }
    });
});
//#endregion

//#region Lấy Email, SĐT Của nhân viên
function GetEmailOfEmployee(id) {
    $.ajax({
        type: 'POST',
        url: "/CRMRemider/GetEmailOfEmployee?id=" + id,
        success: function (data) {
           // $('#Employeevalue').tagsinput('destroy');
            $("#EmployeeTag").val(data);
            $("#EmployeeTag").focus();
        }
    });
}
function GetSMSOfEmployee(id) {
    $.ajax({
        type: 'POST',
        url: "/CRMRemider/GetSMSOfEmployee?id=" + id,
        success: function (data) {
            $("#SMSOfEmployee").val(data);
        }
    });
}
//#endregion

$(document).on("change", "#EmployeeId", function () {
    // GetEmailOfEmployee($(this).val());
    VisibleEmailOfEmployee();
    VisibleSMSOfEmployee();
});

//#region Ẩn/ Hiện Email Nhân viên
function VisibleEmailOfEmployee() {
    if ($("#isCCEmail").prop("checked")) {
        $("#EmailOfEmployee").removeAttr("disabled");
        GetEmailOfEmployee($("#EmployeeId").val());
        $("#divEmailOfEmployee").css("display", "block");
    }
    else {
        $("#EmailOfEmployee").val("");
        $("#EmailOfEmployee").attr("disabled", true);
        $("#divEmailOfEmployee").css("display", "none");
    }
}

$(document).on("change", "#isCCEmail", function () {
    VisibleEmailOfEmployee();
});

//#endregion

//#region Ẩn/ Hiện SMS Nhân viên
function VisibleSMSOfEmployee() {
    if ($("#isCCSMS").prop("checked")) {
        $("#SMSOfEmployee").removeAttr("disabled");
        GetSMSOfEmployee($("#EmployeeId").val());
    }
    else {
        $("#SMSOfEmployee").val("");
        $("#SMSOfEmployee").attr("disabled", true);
    }
}
$(document).on("change", "#isCCSMS", function () {
    VisibleSMSOfEmployee();
});
//#endregion

//#region Xác định ngày thông báo tiếp theo
$(document).on("change", "#ExpiryDate,#StartDate, input[name='DaysPriorNotice']", function () {
    //alert("123");
    GetNextDateReminder();
});

$(document).on("keyup", "input[name='DaysPriorNotice'],#NDays", function () {
    GetNextDateReminder();
    //alert("123");
});
function GetNextDateReminder() {
    if (($("#PeriodCode").val() != "NNgay") || ($("#PeriodCode").val() == "NNgay" && Number($("#NDays").val()) > 0) ) {
        var data = $("#frmHeader").serialize();
        $.ajax({
            type: "POST",
            url: "/CRMRemider/GetNextDateReminder",
            data: data,
            success: function (data) {
                $("#NextDateRemind").html(data);
                $("#NextDateRemindPreview").val(data);
            }
        });
    }
}

//#endregion

//#region Thay đổi loại tham sô Email => xác định loại giá trị (textbox || dropdownlist)
$(document).on("change", ".detail-ValueType", function () {
    var row = $(this).data("row");
    ValueOfEmailPara(row);
});

function ValueOfEmailPara(row) {
    var ValueType = $("select[name='detail[" + row + "].ValueType']").val();
    // alert(ValueType);
    if (ValueType == "AUTO") {
        $("input[name='detail[" + row + "].Value']").css("display", "none");
        $("input[name='detail[" + row + "].Value']").attr("disabled", true);

        $("select[name='detail[" + row + "].Value']").css("display", "block");
        $("select[name='detail[" + row + "].Value']").attr("disabled", false);
    }
    else {
        $("select[name='detail[" + row + "].Value']").css("display", "none");
        $("select[name='detail[" + row + "].Value']").attr("disabled", true);

        $("input[name='detail[" + row + "].Value']").css("display", "block")
        $("input[name='detail[" + row + "].Value']").attr("disabled", false);
    }
}

//#endregion

//#region Thay đổi loại tham sô SMS => xác định loại giá trị (textbox || dropdownlist)
$(document).on("change", ".SMSPara-ValueType", function () {
    var row = $(this).data("row");
    ValueOfSMSPara(row);
});

function ValueOfSMSPara(row) {
    var ValueType = $("select[name='SMSPara[" + row + "].ValueType']").val();
    // alert(ValueType);
    if (ValueType == "AUTO") {
        $("input[name='SMSPara[" + row + "].Value']").css("display", "none");
        $("input[name='SMSPara[" + row + "].Value']").attr("disabled", true);

        $("select[name='SMSPara[" + row + "].Value']").css("display", "block");
        $("select[name='SMSPara[" + row + "].Value']").attr("disabled", false);
    }
    else {
        $("select[name='SMSPara[" + row + "].Value']").css("display", "none");
        $("select[name='SMSPara[" + row + "].Value']").attr("disabled", true);

        $("input[name='SMSPara[" + row + "].Value']").css("display", "block")
        $("input[name='SMSPara[" + row + "].Value']").attr("disabled", false);
    }
}

//#endregion

//#region Check hiển thị Danh sách Email Para
function VisibleEmailPara() {
    if ($("#CheckHasEmailPara").val() == "true") {
        $("#divEmailParameter").css("display", "block");
    }
    else {
        $("#divEmailParameter").css("display", "none");
    }

    // Xét hiển thì loại (textbox, drop) cho Giá trị
    $(".detail-ValueType").each(function () {
        var row = $(this).data("row");
        ValueOfEmailPara(row);
    });
}
//#endregion

//#region Check hiển thị Danh sách SMS Para
function VisibleSMSPara() {
    if ($("#CheckHasSMSPara").val() == "true") {
        $("#divSMSParameter").css("display", "block");
    }
    else {
        $("#divSMSParameter").css("display", "none");
    }

    // Xét hiển thì loại (textbox, drop) cho Giá trị
    $(".SMSPara-ValueType").each(function () {
        var row = $(this).data("row");
        ValueOfSMSPara(row);
    });
}
//#endregion

//#region Check isValid khi submit Form
function IsValid() {
    if (
            ($("#RemiderName").val() == "" )
         || (($("select[name='CustomerId']").val() == null || $("select[name='CustomerId']").val() == "") && ($("select[name='SupplierId']").val() == null || $("select[name='SupplierId']").val() == ""))
         || ($("#PeriodCode").val() == "NNgay" && $("#NDays").val() == "")
         || ($("#isEmailNotified").prop("checked") && $("#EmailTemplateId").val() == "")
         || ($("#isSMSNotifred").prop("checked") && $("#SMSTemplateId").val() == "")
         || ($("#isEmailNotified").prop("checked") == false && $("#isSMSNotifred").prop("checked") == false)
         || ($("input[name='PeriodType']:checked").attr("id") == "MOTLAN" && $("#ExpiryDate").val() == "")
         || ($("input[name='PeriodType']:checked").attr("id") == "DINHKY" && $("#StartDate").val() == "")
        )
    {
        return false;
    }
    else
        return true;
}
//#endregion

//#region Kiểm tra giá trị mỗi para có rỗng hay không
//Check Required fill value of Email Para
function RequiredFillValueEmailPara() {
    var RowNotValid = -1;
    $('.ValueEmailPara').each(function () {
        var row = $(this).data("row");
        if ($("select[name='detail["+ row +"].ValueType']").val() == "CUSTOM" && $(this).val() == "") {
            //console.log($(this).data("row"));
            RowNotValid = row;
            return false ;
        }
    });
    return RowNotValid;
}

//Check Required fill value of SMS Para
function RequiredFillValueSMSPara() {
    var RowNotValid = -1;
    $('.ValueSMSPara').each(function () {
        var row = $(this).data("row");
        if ($("select[name='SMSPara[" + row + "].ValueType']").val() == "CUSTOM" && $(this).val() == "") {
            //console.log($(this).data("row"));
            RowNotValid = row;
            return false;
        }
    });
    return RowNotValid;
}

//#endregion

//#region Ajax Submit Form
function AjaxSubmit(action) {

    var RowNotValidEmailPara = RequiredFillValueEmailPara();
    var RowNotValidSMSPara = RequiredFillValueSMSPara();
    var EmailOfEmployeeView = $("#EmailOfEmployeeView").val();
    if ($("#isEmailNotified").prop("checked") == true && $("#isCCEmail").prop("checked") == true) {
        $("#EmailOfEmployee").val(EmailOfEmployeeView);
    }
    else {
        $("#EmailOfEmployee").val("");
    }
    if (IsValid() == false) {
        $("#divPopup #content").html("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
        $("#divPopup").modal("show");
    } else if (($("#PeriodCode").val() == "NNgay" && Number($("#NDays").val()) <= 0)) {
        $("#divPopup #content").html("Vui lòng nhập 'Số N ngày' > 0");
        $("#divPopup").modal("show");
    } else if ($("#isEmailNotified").prop("checked") == true && $("#isCCEmail").prop("checked") == true && $("#EmailOfEmployee").val() == "") {
        $("#divPopup #content").html("Vui lòng nhập 'Sẽ CC cho Email này'");
        $("#divPopup").modal("show");
    } else if ($("#isSMSNotifred").prop("checked") == true &&  $("#isCCSMS").prop("checked") == true && $("#SMSOfEmployee").val() == "") {
        $("#divPopup #content").html("Vui lòng nhập 'SDT cho NV Q.lý'");
        $("#divPopup").modal("show");
    } else if (RowNotValidEmailPara != -1) {
        var EmailParameterName = $("input[name='detail[" + RowNotValidEmailPara + "].EmailParameterName']").val();
        $("#divPopup #content").html("Vui lòng nhập giá trị cho tham số '" + EmailParameterName + "'!");
        $("#divPopup").modal("show");
    } else if (RowNotValidSMSPara != -1) {
        var SMSParameterName = $("input[name='SMSPara[" + RowNotValidSMSPara + "].SMSParameterName']").val();
        $("#divPopup #content").html("Vui lòng nhập giá trị cho tham số '" + SMSParameterName + "'!");
        $("#divPopup").modal("show");
    } else {
        loading2();
        for (instance in CKEDITOR.instances) {
            CKEDITOR.instances[instance].updateElement();
        }
        var data = $("#frmHeader").serialize() + "&" + $("#frmEmailPara").serialize() + "&" + $("#frmSMSPara").serialize();
        $.ajax({
            type: "POST",
            url: "/CRMRemider/" + action,
            data: data,
            success: function (data) {
                if (data == "success") {
                    window.location.assign("/CRMRemider/Index");
                }
                else {
                    $("#divPopup #content").html(data);
                    $("#divPopup").modal("show");
                }
            }
        });
    }
}
//#endregion

//#region Xem trước Email sẽ gửi
$(document).on("click", "#btnPreviewEmail", function () {
    //alert("email");
    var RowNotValidEmailPara = RequiredFillValueEmailPara();
    var EmailOfEmployeeView = $("#EmailOfEmployeeView").val();
    $("#EmailOfEmployee").val(EmailOfEmployeeView);
    if (IsValid() == false) {
        $("#divPopup #content").html("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
        $("#divPopup").modal("show");
    } else if (($("#PeriodCode").val() == "NNgay" && Number($("#NDays").val()) <= 0)) {
        $("#divPopup #content").html("Vui lòng nhập 'Số N ngày' > 0");
        $("#divPopup").modal("show");
    } else if ($("#isEmailNotified").prop("checked") == true && $("#isCCEmail").prop("checked") == true && $("#EmailOfEmployee").val() == "") {
        $("#divPopup #content").html("Vui lòng nhập 'Sẽ CC cho Email này'");
        $("#divPopup").modal("show");
    } else if (RowNotValidEmailPara != -1) {
        var EmailParameterName = $("input[name='detail[" + RowNotValidEmailPara + "].EmailParameterName']").val();
        $("#divPopup #content").html("Vui lòng nhập giá trị cho tham số '" + EmailParameterName + "'!");
        $("#divPopup").modal("show");
    } else {
        loading2();
        for (instance in CKEDITOR.instances) {
            CKEDITOR.instances[instance].updateElement();
        }
        var data = $("#frmHeader").serialize() + "&" + $("#frmEmailPara").serialize();//+ "&" + $("#frmSMSPara").serialize();
        $.ajax({
            type: "POST",
            url: "/CRMRemider/_PreviewEmail",
            data: data,
            success: function (data) {
                 $("#divPreviewCRM .title").html("Nội dung Email sẽ gửi");
                 $("#divPreviewCRM .modal-body").html(data);
                 $("#divPreviewCRM").modal("show");
            }
        });
        
    }

});
//#endregion

//#region Xem trước SMS sẽ gửi
$(document).on("click", "#btnPreviewSMS", function () {
    //alert("email");
    var RowNotValidSMSPara = RequiredFillValueSMSPara();
    if (IsValid() == false) {
        $("#divPopup #content").html("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
        $("#divPopup").modal("show");
    } else if (($("#PeriodCode").val() == "NNgay" && Number($("#NDays").val()) <= 0)) {
        $("#divPopup #content").html("Vui lòng nhập 'Số N ngày' > 0");
        $("#divPopup").modal("show");
    } else if ($("#isSMSNotifred").prop("checked") == true && $("#isCCSMS").prop("checked") == true && $("#SMSOfEmployee").val() == "") {
        $("#divPopup #content").html("Vui lòng nhập 'SDT cho NV Q.lý'");
        $("#divPopup").modal("show");
    } else if (RowNotValidSMSPara != -1) {
        var SMSParameterName = $("input[name='SMSPara[" + RowNotValidSMSPara + "].SMSParameterName']").val();
        $("#divPopup #content").html("Vui lòng nhập giá trị cho tham số '" + SMSParameterName + "'!");
        $("#divPopup").modal("show");
    } else {
        loading2();
        for (instance in CKEDITOR.instances) {
            CKEDITOR.instances[instance].updateElement();
        }
        var data = $("#frmHeader").serialize() + "&" + $("#frmSMSPara").serialize();//+ "&" + $("#frmSMSPara").serialize();
        $.ajax({
            type: "POST",
            url: "/CRMRemider/_PreviewSMS",
            data: data,
            success: function (data) {
                $("#divPreviewCRM .title").html("Nội dung SMS sẽ gửi");
                $("#divPreviewCRM .modal-body").html(data);
                $("#divPreviewCRM").modal("show");
            }
        });

    }

});
//#endregion

//#region Xem trước SMS sẽ gửi
$(document).on("click", "#btnPreviewSMS", function () {
    //alert("sms");
    $("#divPreviewCRM .modal-body").html("");
    $("#divPreviewCRM").modal("show");
});
//#endregion