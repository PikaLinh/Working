
$(document).ready(function () {
    //#region Thông tin khách hàng
    // Khách hàng
});

//$(document).on("click", ".HairTypeId", function () {
//    $(".HairTypeId").removeClass("btn-danger");
//    $(this).addClass("btn-danger");
//    var HairTypeId = $(this).data("hairtypeid");
//    GetServicePartital(HairTypeId);
//});
$(document).on("change", "select[name='HairTypeId']", function () {
    var HairTypeId = $(this).val();
    //var HairTypeId = $("select[name = 'HairTypeId']").find("option:selected").val();
    GetServicePartital(HairTypeId);
});

$(document).on("change", "select[name='CustomerId']", function () {
    var CustomerId = $(this).val();
    $.ajax({
        type: "POST",
        url: "/DailyChicCutOrder/GetTreatment?CustomerId=" + CustomerId,
        data: {
            CustomerId: CustomerId,
        },
        success: function (data) {
            $("#tblOrderDetail tbody").html(data.Details);
            addButton();
            
            
        }
    });
});

//Hiển thị button Xem => Xem chi tiết liệu trình nhiều lần mà khách hàng đã sử dụng
function addButton() {
    $('.viewTreatmentDetails').each(function (i, obj) {
        var dataRow = $(this).data("row");
        var Price = $("input[name='details[" + dataRow + "].Price']").val();
        var UnitPrice = $("input[name='details[" + dataRow + "].UnitPrice']").val();
        if(Price == 0 && UnitPrice == 0)
        {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });
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
            addButton();
            TotalPrice();
        }
    });
    // $btn.button('reset');
    return false;
});

// Xử lý viewTreatmentDetails
$(document).on("click", ".viewTreatmentDetails", function () {
    var CustomerId = $("select[name='CustomerId']").val();
    var dataRow = $(this).data("row");
    var TreatmentId = $("input[name='details[" + dataRow + "].TreatmentId']").val();
    $.ajax({
        type: "POST",
        url: "/DailyChicCutOrder/GetTreatmentDetails?CustomerId=" + CustomerId + "&TreatmentId=" + TreatmentId,
        data: {
            CustomerId: CustomerId,
            TreatmentId: TreatmentId
        },
        success: function (data) {
            $("#divTreatmentPopup #content").html(data.Treatment);
            $("#divTreatmentPopup .modal-header h1").html(data.CusName);
            $("#divTreatmentPopup").modal("show");
            $('#divTreatmentPopup').modal({
                backdrop: 'static',
                keyboard: false,
            })
        }
    });
});

//Xử lý nút đóng trong modal chi tiết liệu trình
$(document).on("click", "#btnHuyTreatmentDetails", function () {
    $("#divTreatmentPopup").modal("hide");
})

//Lấy thông tin dịch vụ dựa vào loại giá
function GetServicePartital(HairTypeId) {
    if (HairTypeId != undefined && HairTypeId != "") {
        $.ajax({
            type: "POST",
            url: "/DailyChicCutOrder/_GetServicePartital?HairTypeId=" + HairTypeId,
            success: function (data) {
                //Lấy dữ liệu đổ vào tab content
                $(".tab-dv").remove();
                $("#tab-sp").appendTo($("#tab-content").html(data));
                $('.nav-tabs a[href="#tab-sp"]').tab('show');

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
                //var HairTypeId = $(".HairTypeId").filter(".btn-danger").data("hairtypeid");
                var HairTypeId = $("select[name = 'HairTypeId']").val();
                GetServicePartital(HairTypeId);

                var OrderId = $("#OrderId").val();
                //Nếu sửa thì disable 1 số field
                if (Number(OrderId) > 0) {
                    $("select[name='CustomerId']").attr('disabled', 'disabled');
                    $("select[name='HairTypeId']").attr('disabled', 'disabled');
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