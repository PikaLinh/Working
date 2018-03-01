formatNumberForGird();

$(document).ready(function () {
    // Khách hàng
    var CustomerId = $("#IdCustomer").val();
    var CustomerName = $("#FullName").val();
    Select2_Custom("/Sell/GetCustomerId", "CustomerId", CustomerId, CustomerName, "divCustomerId");
    formatNumberForGird();
});

// Load Sản phẩm từ Select2
//Select2_CustomForList("/ImportMaster/GetProductId", "ProductId");

// Tỉnh
var ProvinceId = $("input[name='IdProvince']").val();
var ProvinceName = $("input[name='ProvinceName']").val();
Select2_Custom("/Sell/GetProvinceId", "ProvinceId", ProvinceId, ProvinceName, "divProvinceId");

// Quận-Huyện
var url = "/Sell/GetDistrictByProvinceId?ProvinceIdSelected=" + ProvinceId;
var DistrictId = $("input[name='IdDistrict']").val();
var DistrictName = $("input[name='DistrictName']").val();
Select2_Custom(url, "DistrictId", DistrictId, DistrictName, "divDistrictId");

//Tổng tiền
TotalPrice();

// Load Sản phẩm từ Select2
Select2_CustomForList("/Sell/GetProductId", "ProductId");

// Bước  : Xử lý btnUpdate
$(document).on("click", "#btnUpdate", function () {
    //var $btn = $(this).button('loading');
    loading2();
    if ($("select[name='PaymentMethodId']").val() == 0) {
        $("#divPopup #content").html("Vui lòng chọn phương thức thanh toán");
        $("#divPopup").modal("show");
        $("body").removeClass("loading2");
    }
    else {
        var data = $("#frmList").serialize() + "&" + $("#frmHeader").serialize();
        $.ajax({
            type: "POST",
            url: "/PreOrderMaster/Update",
            data: data,
            success: function (data) {
                // $btn.button('reset');
                //$("#tblImportDetail tbody").html(data);
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

