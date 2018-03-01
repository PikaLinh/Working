$(document).ready(function () {
    DisplayConfigParentId($("#IsProduct").prop("checked"));
    DisplayConfigSelect2ParentProductId($("#isParentProduct").prop("checked"));
});
// Load Sản phẩm từ Select2
Select2_Custom("/Product/GetProductId", "ParentProductId", $("#IdParentProduct").val(), $("#NameParentProduct").val(), "divParentProductId");

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
        url: "/DynamicProduct/GetExchangeRateBy",
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
    var CategoryIdSelect = $("select[name='CategoryId']").val();
    var ProductStoreCodeMark = $("#ProductStoreCodeMark").val();
    $.ajax({
        type: "POST",
        url: "/DynamicProduct/UpdateProductStoreCode",
        data: {
            StoreId: StoreIdSelected,
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


// Bước 1 xử lý thêm dòng mới trong CreateList
$("#btnAddNewRow").unbind("click").click(function () {
    var data = $("#frmList").serializeArray();
    $.ajax({
        type: "POST",
        url: "/DynamicProduct/_CreatelistInner",
        data: data,
        success: function (data) {
            $("#tblImportDetail tbody").html(data);
            formatNumberForGird();
        }
    });
});

// Bước 2 xử lý detail-btndelete
$(document).on("click", ".detail-btndelete", function () {

    var data = $("#frmList").serializeArray();
    var removeId = $(this).data("row");
    $.ajax({
        type: "POST",
        url: "/DynamicProduct/_DeletelistInner?RemoveId=" + removeId,
        data: data,
        success: function (data) {
            $("#tblImportDetail tbody").html(data);
            formatNumberForGird();
        }
    });
    return false;
});


//Định dạng hiển thị số 
function formatNumberForGird() {
    $(".pricelist-Price").number(true);
}
$(document).on("keyup", "#Price", function () {
    //if ($(".pricelist-Price").val() == "")
    //{
    //    $(this).val(0);
    //}
    formatNumberForGird();
});

$(document).on("change", "#IsProduct", function () {
    DisplayConfigParentId($(this).prop("checked"));
});
function DisplayConfigParentId(IsProduct) {
    if (IsProduct == true) {
        $("#divSystemProduct").css("display", "block");
        $("#divParentProductIdToDisplay").css("display", "block");
    }
    else {
        $("#divSystemProduct").css("display", "none");
        $("#isParentProduct").prop("checked", false);
        $("select[name='ParentProductId']").empty().trigger('change');
    }
}

$(document).on("change", "#isParentProduct", function () {
    DisplayConfigSelect2ParentProductId($(this).prop("checked"));
    //alert("123");
});
function DisplayConfigSelect2ParentProductId(isParentProduct) {
    if (isParentProduct == false) {
        $("#divParentProductIdToDisplay").css("display", "block");
    }
    else {
        $("#divParentProductIdToDisplay").css("display", "none");
        $("select[name='ParentProductId']").empty().trigger('change');
    }
}



