
$(document).ready(function () {
    //#region Thông tin khách hàng
    // Khách hàng
});

$(document).on("click", ".HairTypeId", function () {
    $(".HairTypeId").removeClass("btn-danger");
    $(this).addClass("btn-danger");
    var HairTypeId = $(this).data("hairtypeid");
    GetServicePartital(HairTypeId);
});

//Xử lý dropdown KH => không cho chọn phục vụ 1 KH 2 lần cùng lúc
$(document).on("change", "select[name='CustomerId']", function () {
    var CustomerId = $("select[name='CustomerId']").val();
    if (CustomerId != "") {
        $.ajax({
            type: "POST",
            url: "/DailyChicCutOrder/CheckCustomerExist",
            data: {
                CustomerId: CustomerId
            },
            success: function (data) {
                if (data.Message) {
                    Alert_Popup(data.Message);
                }
            }
        });
    }
});
//Xử lý nút đóng trong modal thông báo => reset dropdownlist
$(document).on("click", "#btnCancel", function () {
    $("#divPopup").modal("hide");
    $("select[name='CustomerId']").val("").trigger("change");
})
function Alert_Popup(Centent_Alert) {
    $.ajax({
        type: "POST",
        url: "/DailyChicCutOrder/Alert",
        data: {
            Content: Centent_Alert
        },
        datatype: "json",
        success: function (jsondata) {
            $("#divAlert").html(jsondata)
            $("#divPopup").modal("show");
            $('#divPopup').modal({
                backdrop: 'static',
                keyboard: false,
            })
        }
    });
    return false;
}
// Xử lý details-btndelete
$(document).on("click", ".details-btndelete", function () {
    var data = $("#frmList").serializeArray();
    var dataRow = $(this).data("row");
    var ServiceId = $("input[name='details[" + dataRow + "].ServiceId']").val();
    var ProductId = $("input[name='details[" + dataRow + "].ProductId']").val();
    var OrderId = $("#OrderId").val();
    $.ajax({
        type: "POST",
        url: "/DailyChicCutOrder/_DeleteDetailInnerInfo?ServiceId=" + ServiceId + "&OrderId=" + OrderId + "&ProductId=" + ProductId,
        data: data,
        success: function (data) {
            $("#tblOrderDetail tbody").html(data);
            TotalPrice();
        }
    });
    // $btn.button('reset');
    return false;
});

function GetServicePartital(HairTypeId) {
    if (HairTypeId != undefined) {
        $.ajax({
            type: "POST",
            url: "/DailyChicCutOrder/_GetServicePartital?HairTypeId=" + HairTypeId,
            success: function (data) {
                $("#contentService").html(data);
            }
        });
    }
}

//#region Thông tin khách hàng
// Thay đổi Mã Kh => Lấy được thông tin khách hàng tương ứng


function CustomerInfo(CustomerId) {
    if (CustomerId != undefined && CustomerId != "") {
        $.ajax({
            type: "POST",
            url: "/Sell/GetProfileByCustomerId?CustomerID=" + CustomerId,
            success: function (data) {
                $("input[name='IdentityCard']").val(data.IdentityCard);
                $("input[name='FullName']").val(data.FullName);
                $("input[name='IdentityCard']").val(data.IdentityCard);
                $("input[name='Phone']").val(data.Phone);
                $("input[name='Gender']").each(function () {
                    $(this).removeAttr("checked");
                });
                $("#Gender").val(data.Gender);
                if (data.Gender == true) {
                    $("#Nam").prop("checked", true);
                }
                else {
                    $("#Nu").prop("checked", true);
                }
                var Gender = $("input[name='Gender']:checked").val();
                var HairTypeId = $(".HairTypeId").filter(".btn-danger").data("hairtypeid");
                GetServicePartital(HairTypeId);

                var OrderId = $("#OrderId").val();
                if (Number(OrderId) > 0) {
                    $("select[name='CustomerId']").attr('disabled', 'disabled');
                    $("#Phone").attr('disabled', 'disabled');
                }

            }
        });
    }
}
//#endregion

//#region Thêm mới khách hàng
$(document).on("click", "#btn-add-new-customer", function () {
    $("#Mes-error-add-new-customer").html("");
    $("#divPopupOrder").modal("hide");
    $.ajax({
        type: "POST",
        url: "/DailyChicCutOrder/_AddNewCustomerPartital",
        success: function (data) {
            $("#divPopup-add-new-customer #contentInfoCustomer").html(data);
            $("#divPopup-add-new-customer").modal("show");
        }
    });
});
$(document).on("click", "#btnSave-add-new-customer", function () {
    if ($("#frm-add-new-customer").valid()) {
        loading2();
        var data = $("#frm-add-new-customer").serialize();
        $.ajax({
            type: "POST",
            url: "/DailyChicCutOrder/_SaveAddNewCustomerPartital",
            data: data,
            success: function (response) {
                if (response.resuilt == "success") {
                    $("#divPopup-add-new-customer").modal("hide");
                    $("#divPopupOrder").modal("show");
                    Select2_Custom("/Sell/GetCustomerId", "CustomerId", response.CustomerId, response.CustomerName, "divCustomerId");
                    CustomerInfo(response.CustomerId);
                }
                else {
                    $("#Mes-error-add-new-customer").html(response.resuilt);
                }
            }
        });
    }
});
//#endregion