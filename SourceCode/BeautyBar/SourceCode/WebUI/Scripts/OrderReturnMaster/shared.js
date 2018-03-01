$("#divExchangeRate").hide();
$("#ExchangeRate").val(1);
$("#ExchangeDate").val("2016-05-01");


function CalTotalPrice() {// Tính tổng cộng thành tiền khi thay đổi Giảm giá và VAT
    var TongTien = $("#ViewBagSumPrice").val();

}

function CheckValidDiscount() {
    var SumPrice = $("#ViewBagSumPrice").val() * 1;
    var BillDiscount = $("#BillDiscount").val() * 1;
    var BillDiscountTypeId = $("select[name='BillDiscountTypeId']").val() * 1;
    var Discount = 0;
    if (BillDiscountTypeId == 1) {
        Discount = BillDiscount;
    }
    else {
        Discount = (BillDiscount / 100) * SumPrice;
    }
    //alert(SumPrice + " " + BillDiscount + " " + BillDiscountTypeId);
    if (Discount > SumPrice) {
        $("#BillDiscount").val("");
    }
}



$(document).on("keyup", "#BillDiscount", function () {
    CheckValidDiscount();
    TotalPrice();
   // RemainingAmount();
});
$(document).on("keyup", "#BillVAT", function () {
    if ($(this).val() > 100) {
        $(this).val(1);
    }
    TotalPrice();
    //RemainingAmount();
});

$("select[name='BillDiscountTypeId']").on("change", function (e) {
    CheckValidDiscount();
    TotalPrice();
   // RemainingAmount();
});
$("select[name='PaymentMethodId']").on("change", function (e) {
    //alert("aa");
    var PaymentMethodId = $(this).val();
    if (PaymentMethodId == 4) {
        $("#divGuestAmountPaid").hide();
        $("#GuestAmountPaid").val(0);
        TotalPrice();
    }
    else {
        $("#divGuestAmountPaid").show();
    }
});
function RemainingAmount() {
    var SumPrice = $("#ViewBagTotalPrice").val();
    var GuestAmountPaid = $("#GuestAmountPaid").val();
    // Không cho số tiền trả ngay > số tiền còn lại
    if ((Number(SumPrice) - Number(GuestAmountPaid)) < 0) {
        $("#GuestAmountPaid").val(0);
        $("#RemainingAmount").val(Number(SumPrice));
        $("#RemainingAmount2").html(Number(SumPrice));
    }
    else {
        $("#RemainingAmount").val(Number(SumPrice) - Number(GuestAmountPaid));
        $("#RemainingAmount2").html(Number(SumPrice) - Number(GuestAmountPaid));
    }

    $("#RemainingAmount2").number(true);
}
$(document).on("keyup", "#GuestAmountPaid", function () {
    RemainingAmount();
});

//Định dạng hiển thị số tiền
function formatNumberForGird() {
    // Định dạng hiển thị tiền : trọng lượng, giá, thành tiền
    //$("#TotalWeight strong").number(true);
    //$("#TotalPrice strong").number(true);
    $(".detail-SellQuantity").number(true);
    $(".detail-ReturnedQty").number(true);
    $(".detail-ReturnQuantity").number(true);
    $(".detail-Price").number(true);
    $(".unit-price").number(true);
    $("#Paid").number(true);
    $("#TotalPrice").number(true);
    $("#ViewBagTotalPrice").number(true);
    $("#SumPrice").number(true);
    $("#BillDiscount").number(true);
    $("#GuestAmountPaid").number(true);
    $("#RemainingAmount2").number(true);
}


















//$(document).on("keyup", "#ManualDiscount", function () {
//    TotalWeight();
//    RemainingAmount();
//});


//$("select[name='ManualDiscountType']").on("change", function (e) {
//    TotalWeight();
//    RemainingAmount();
//});
//$(document).on("keyup", "#VATValue", function () {
//    TotalWeight();
//    RemainingAmount();
//});
//$(document).on("keyup", "#GuestAmountPaid", function () {
//    RemainingAmount();
//});
//function RemainingAmount() {
//    var SumPrice = $("#ViewBagTotalPrice").val();
//    var GuestAmountPaid = $("#GuestAmountPaid").val();
//    //console.log(Number(SumPrice) - Number(GuestAmountPaid));
//    // Không cho số tiền trả ngay > số tiền còn lại
//    if ((Number(SumPrice) - Number(GuestAmountPaid)) < 0) {
//        $("#GuestAmountPaid").val(0);
//        $("#RemainingAmount").val(Number(SumPrice) );
//        $("#RemainingAmount2").html(Number(SumPrice));
//    }
//    else {
//        $("#RemainingAmount").val(Number(SumPrice) - Number(GuestAmountPaid));
//        $("#RemainingAmount2").html(Number(SumPrice) - Number(GuestAmountPaid));
//    }
    
//    formatNumberForGird();
//}
//function formatNumberForGird() {
//    // Định dạng hiển thị tiền : trọng lượng, giá, thành tiền
//    $("#SumPrice").number(true);
//    $(".detail-UnitShippingWeight").number(true);
//    $(".detail-Price").number(true);
//    $(".unit-price").number(true);
//    $("#ExchangeRate").number(true);
//    $("#Cash").number(true);
//    $("#MoneyTransfer").number(true);
//    $("#VATValue").number(true);
//    $("#ManualDiscount").number(true);
//    $(".detail-ShippingFee").number(true);
//    $("#ManualDiscount").number(true);
//    $("#GuestAmountPaid").number(true); 
//    $("#TotalPrice").number(true); 
//    $("#RemainingAmount2").number(true);
//    $(".detail-ImportQty").number(true);
//    $(".detail-InventoryQty").number(true);
//    $(".detail-ReturnQty").number(true);
//    $(".detail-ReturnedQty").number(true);
//    $("#ExchangeRate").number(true);
//    $(".detail-UnitShippingWeight").number(true,3);
//}
//(function ($) {
//    // Thay đổi số lượng trả
//    $(document).on("keyup", ".detail-ReturnQty", function () {
//        TotalWeight();
//    });
//    // Thay đổi thành tiền =>  update TotalPrice
//    $(document).on("keyup", ".detail-UnitPrice", function () {
//        TotalWeight();
//    });
    
//})(jQuery);
