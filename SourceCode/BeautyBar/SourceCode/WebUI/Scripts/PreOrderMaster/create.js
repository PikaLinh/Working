
$(document).ready(function () {
    // Khách hàng
    var CustomerId = $("#IdCustomer").val();
    var CustomerName = $("#FullName").val();
    Select2_Custom("/Sell/GetCustomerId", "CustomerId", CustomerId, CustomerName, "divCustomerId");
    Select2_Custom("/Sell/GetSaleId", "SaleId");
    formatNumberForGird();
});
// Tỉnh
var ProvinceId = $("input[name='IdProvince']").val();
var ProvinceName = $("input[name='ProvinceName']").val();
Select2_Custom("/Sell/GetProvinceId", "ProvinceId", ProvinceId, ProvinceName, "divProvinceId");

// Quận-Huyện
var url = "/Sell/GetDistrictByProvinceId?ProvinceIdSelected=" + ProvinceId;
var DistrictId = $("input[name='IdDistrict']").val();
var DistrictName = $("input[name='DistrictName']").val();
Select2_Custom(url, "DistrictId", DistrictId, DistrictName, "divDistrictId");
//// Tỉnh
//Select2_Custom("/Sell/GetProvinceId", "ProvinceId");

//// Quận - Huyện
//Select2_Custom("/Sell/GetDistrictByProvinceId", "DistrictId");

// Bước 3 : Xử lý btnSave
$(document).on("click", "#btnSave", function () {
    loading2();
    //var $btn = $(this).button('loading');
    if ($("select[name='PaymentMethodId']").val() == 0) {
        $("#divPopup #content").html("Vui lòng chọn 'phương thức thanh toán'");
        $("#divPopup").modal("show");
        $("body").removeClass("loading2");
    }
    else {
        var data = $("#frmList").serialize() + "&" + $("#frmHeader").serialize();
        //var data = $("#frmHeader").serialize();
        $.ajax({
            type: "POST",
            url: "/PreOrderMaster/Save",
            data: data,
            success: function (data) {
                // $btn.button('reset');
                //$("#tblImportDetail tbody").html(data);
                //$btn.button('reset');
                if (data == "success") {
                    window.location.assign("/PreOrderMaster/Index");
                }
                else {
                    $("#divPopup #content").html(data);
                    $("#divPopup").modal("show");
                }
            }
        });
    }
});
