
$(document).ready(function () {
    formatNumberForGird();
    TotalWeight();
    if ($("select[name='CurrencyId']").val() != 1) {
        $("#ExchangeRate").val($("#ExchangeRateEdit").val());
        $("#divExchangeRate").show();
    }
});
// Nhà cung cấp
Select2_Custom("/ImportMaster/GetSuplierID", "SupplierId", $("#SupplierIdTempt").val(), $("#SupplierName").val(), "divSupplierId");

// Load Sản phẩm từ Select2
Select2_CustomForList("/ImportMaster/GetProductId", "ProductId");

// Bước  : Xử lý btnUpdate
$(document).on("click", "#btnUpdate", function () {
    //var $btn = $(this).button('loading');
    var data = $("#frmList").serialize() + "&" + $("#frmHeader").serialize();
    $.ajax({
        type: "POST",
        url: "/PreImportMaster/Update",
        data: data,
        success: function (data) {
           // $btn.button('reset');
            //$("#tblImportDetail tbody").html(data);
            if (data == "success") {
                window.location.assign("/PreImportMaster/Index");
            }
            else {
                $("#divPopup #content").html(data);
                $("#divPopup").modal("show");
            }
        }
    });

});
