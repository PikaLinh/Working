$("#divExchangeRate").hide();
$("#ExchangeRate").val(1);
$("#ExchangeDate").val("2016-05-01");


function CalTotalPrice() {// Tính tổng cộng thành tiền khi thay đổi Giảm giá và VAT
    var TongTien = $("#ViewBagSumPrice").val();

}
$(document).on("keyup", "#ManualDiscount", function () {
    TotalWeight();
    //RemainingAmount();
});


$("select[name='ManualDiscountType']").on("change", function (e) {
    TotalWeight();
    //RemainingAmount();
});
$(document).on("keyup", "#VATValue", function () {
    TotalWeight();
    //RemainingAmount();
});
$(document).on("keyup", "#GuestAmountPaid", function () {
    RemainingAmount();
});
function RemainingAmount() {
    var SumPrice = $("#ViewBagTotalPrice").val();
    var GuestAmountPaid = $("#GuestAmountPaid").val();
    //console.log(Number(SumPrice) - Number(GuestAmountPaid));
    // Không cho số tiền trả ngay > số tiền còn lại
    if ((Number(SumPrice) - Number(GuestAmountPaid)) < 0) {
        $("#GuestAmountPaid").val(0);
        $("#RemainingAmount").val(Number(SumPrice) );
        $("#RemainingAmount2").html(Number(SumPrice));
    }
    else {
        $("#RemainingAmount").val(Number(SumPrice) - Number(GuestAmountPaid));
        $("#RemainingAmount2").html(Number(SumPrice) - Number(GuestAmountPaid));
    }
    
    $("#RemainingAmount2").number(true);
}
function formatNumberForGird() {
    // Định dạng hiển thị tiền : trọng lượng, giá, thành tiền
    $("#SumPrice").number(true);
    $(".detail-UnitShippingWeight").number(true);
    $(".detail-Price").number(true);
    $(".unit-price").number(true);
    $("#ExchangeRate").number(true);
    $("#Cash").number(true);
    $("#MoneyTransfer").number(true);
    $("#VATValue").number(true);
    $("#ManualDiscount").number(true);
    $(".detail-ShippingFee").number(true);
    $("#ManualDiscount").number(true);
    $("#GuestAmountPaid").number(true); 
    $("#TotalPrice").number(true); 
    $("#RemainingAmount2").number(true);
    $(".detail-ImportQty").number(true);
    $(".detail-InventoryQty").number(true);
    $(".detail-ReturnQty").number(true);
    $(".detail-ReturnedQty").number(true);
    $("#ExchangeRate").number(true);
    $(".detail-UnitShippingWeight").number(true,3);
}
(function ($) {
    // Thay đổi số lượng trả
    $(document).on("keyup", ".detail-ReturnQty", function () {
        TotalWeight();
    });
    // Thay đổi thành tiền =>  update TotalPrice
    $(document).on("keyup", ".detail-UnitPrice", function () {
        TotalWeight();
    });
    
})(jQuery);
