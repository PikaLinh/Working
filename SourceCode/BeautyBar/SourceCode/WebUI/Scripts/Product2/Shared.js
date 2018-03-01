
formatnumber();
function numberWithCommas(x) {// định dạng số phân cách hàng nghìn
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
function changeCOGS() {
    $('#COGS').val(Number($('#ImportPrice').val()) * Number($('#ExchangeRate').val()) + Number($('#ShippingFee').val()));
    $("#COGSDisplay").html(numberWithCommas(Number($('#ImportPrice').val()) * Number($('#ExchangeRate').val()) + Number($('#ShippingFee').val())));
}
$(document).on("change", "#CurrencyId", function () {
    var CurrencyId = $(this).val();
    $.ajax({
        type: "POST",
        url: "/Product/GetExchangeRateBy",
        data: {
            CurrencyIdSelect: CurrencyId
        },
        dataType: "json",
        success: function (jsonData) {
            //var listItems = "";

            //$.each(jsonData, function (i, item) {
            //    listItems += item.ExchangeRate;
            //});

            //$("#ExchangeRate").html(listItems);
            $("#ExchangeRate").val(jsonData);
            changeCOGS();
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
$(document).on("change", "#isHot", function () {
    // alert($('#isHot').prop("checked"));
    if ($('#isHot').prop("checked") == true) {
        $('#divOrderBy').show();
    }
    else {
        $('#divOrderBy').hide();
        $('#OrderBy').val("");
    }
});

function GetProductStoreCode() {
    var StoreIdSelected = $("select[name='StoreId']").val();
    var ProductTypeIdSelected = $("select[name='ProductTypeId']").val();
    var CategoryIdSelect = $("select[name='CategoryId']").val();
    var ProductStoreCodeMark = $("#ProductStoreCodeMark").val();
    $.ajax({
        type: "POST",
        url: "/Product/UpdateProductStoreCode",
        data: {
            StoreId: StoreIdSelected,
            ProductTypeId: ProductTypeIdSelected,
            CategoryId: CategoryIdSelect,
            ProductStoreCodeMark: ProductStoreCodeMark
        },
        success: function (ProductStoreCode) {
            $("#divOrderCode h4 span").html(ProductStoreCode);
            $("#ProductStoreCode").val(ProductStoreCode);
            //alert(ProductStoreCode);
        }
    });
}

function formatnumber() {
    $("#ImportPrice").number(true);
    $("#ShippingFee").number(true);
    $("#ExchangeRate").number(true, 2);
    $("#COGS").number(true);
    $("#Price1").number(true);
    $("#Price2").number(true);
    $("#Price3").number(true);
    $("#Price4").number(true);
    $("#ShippingWeight").number(true);
}

$(document).on("click", "#RefreshProductStoreCode", function () {
    // alert("1");
    GetProductStoreCode();
});


//#region Số lượng cảnh báo
$(document).on("click", "#SettingQtyAlert", function () {
    $("#QtyAlertList").modal("show");
});
//#endregion 