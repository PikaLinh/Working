if ($("#CurrencyId").val() == 1) {
    $("#ImportPrice").number(true);
    $("#ShippingFee").number(true);
    $("#ExchangeRate").number(true);
    $("#COGS").number(true);
}
else {
    $("#ImportPrice").number(true, 2);
    $("#ShippingFee").number(true, 2);
    $("#ExchangeRate").number(true, 2);
    $("#COGS").number(true, 2);
}
function changeCOGS() {
    if ($('#ShippingFee').val() != '' && $('#ImportPrice').val() != '' && $('#ExchangeRate').val() != '') {
        $('#COGS').val(Number($('#ImportPrice').val()) * Number($('#ExchangeRate').val()) + Number($('#ShippingFee').val()));
    }
}
$(document).on("change", "#CurrencyId", function () {
    var CurrencyId = $(this).val();
    $.ajax({
        type: "POST",
        url: "/Product2/GetExchangeRateBy",
        data: {
            CurrencyIdSelect: CurrencyId
        },
        dataType: "json",
        success: function (data) {
            $("#ExchangeRate").val(data);
        }

    });
    return false;
});
$(document).on("keyup", "#ShippingFee", function () {
    changeCOGS();
});
$(document).on("keyup", "#ImportPrice", function () {
    changeCOGS();
});
$(document).on("keyup", "#ExchangeRate", function () {
    changeCOGS();
});
$(document).on("change", "#isHot", function () {
    if ($("#isHot").prop("checked") == true) {
        $(".divOrderBY").show();
    }
    else {
        $(".divOrderBY").hide();
        $("#OrderBy").val("");
    }
});
$(document).on("change", "#CurrencyId", function () {
    if ($("#CurrencyId").val() == 1) {
        $("#ImportPrice").number(true);
        $("#ShippingFee").number(true);
        $("#ExchangeRate").number(true);
        $("#COGS").number(true);
    }
    else {
        $("#ImportPrice").number(true, 2);
        $("#ShippingFee").number(true, 2);
        $("#ExchangeRate").number(true, 2);
        $("#COGS").number(true, 2);
    }

});
//// Loại sản phẩm
//var ProvinceId = $("input[name='IdProvince']").val();
//var ProvinceName = $("input[name='ProvinceName']").val();
Select2_Custom("/Product/GetProductType", "ProductTypeId", null, null, "divProductType");