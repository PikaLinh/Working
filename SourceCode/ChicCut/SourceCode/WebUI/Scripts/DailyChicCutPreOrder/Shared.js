
$(document).ready(function () {
    //#region Thông tin khách hàng
    // Khách hàng
});
$(document).on("click", ".Gender", function () {
    $(".Gender").removeClass("btn-danger");
    $(this).addClass("btn-danger");
    var Gender = $(this).data("gender");
   // var HairTypeId = $(".HairTypeId").filter(".btn-danger").data("hairtypeid");
    GetServicePartital(Gender);
});

// Xử lý details-btndelete
$(document).on("click", ".details-btndelete", function () {
    var data = $("#frmList").serializeArray();
    var ServiceCategoryId = $(this).data("id");
    var PreOrderId = $("#PreOrderId").val();
    $.ajax({
        type: "POST",
        url: "/DailyChicCutPreOrder/_DeleteDetailInnerInfo?ServiceCategoryId=" + ServiceCategoryId + "&PreOrderId=" + PreOrderId,
        data: data,
        success: function (data) {
            $("#tblOrderDetail tbody").html(data);
            TotalPrice();
        }
    });
    // $btn.button('reset');
    return false;
});

function GetServicePartital(Gender) {
    if (Gender != undefined) {
        $.ajax({
            type: "POST",
            url: "/DailyChicCutPreOrder/_GetServicePartital?Gender=" + Gender ,
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
                //  $(".Gender").removeClass("btn-danger");
                $("#Gender").val(data.Gender);
                if (data.Gender == true) {
                    $("#Nam").prop("checked", true);
                    // $("#Mail").addClass("btn-danger");
                }
                else {
                    $("#Nu").prop("checked", true);
                    // $("#FeMail").addClass("btn-danger");
                }
                var Gender = $("input[name='Gender']:checked").val();
                // var HairTypeId = $(".HairTypeId").filter(".btn-danger").data("hairtypeid");
                GetServicePartital(Gender);

                var PreOrderId = $("#PreOrderId").val();
                if (Number(PreOrderId) > 0) {
                    $("select[name='CustomerId']").attr('disabled', 'disabled');
                    $("#Phone").attr('disabled', 'disabled');
                    //$("input[name='Gender']").attr('disabled', 'disabled');
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
        url: "/DailyChicCutPreOrder/_AddNewCustomerPartital",
        success: function (data) {
            $("#divPopup-add-new-customer #contentInfoCustomer").html(data);
            $("#divPopup-add-new-customer").modal("show");
        }
    });
});
$(document).on("click", "#btnSave-add-new-customer", function () {
    if ($("#frm-add-new-customer").valid())
    {
        loading2();
        var data = $("#frm-add-new-customer").serialize();
        $.ajax({
            type: "POST",
            url: "/DailyChicCutPreOrder/_SaveAddNewCustomerPartital",
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