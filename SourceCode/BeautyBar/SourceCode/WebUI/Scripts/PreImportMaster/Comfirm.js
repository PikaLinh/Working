$(document).ready(function () {
    formatNumberForGird();
});


$(document).on("keyup", ".detail-ConfirmQty", function () {
    var row = $(this).data("row");
    var ConfrimQty = $("input[name='detail[" + row + "].ConfirmQty']").val() * 1;
    var BookQty = $("input[name='detail[" + row + "].Qty']").val() * 1;
    if (ConfrimQty == '' || ConfrimQty == null || ConfrimQty > BookQty) {
        $("#divPopup #content").html("'Số lượng xác nhận' phải nhỏ hơn hoặc bằng 'Số lượng' đã đặt ");
        $("#divPopup").modal("show");
        $("input[name='detail[" + row + "].ConfirmQty']").val(0)
    }
    UnitPrice(row);
    TotalWeight();

    //alert(Value);
});

$(document).on("keyup", ".detail-ShippingFee", function () {
    var row = $(this).data("row");
    var value = $("input[name='detail[" + row + "].ShippingFee']").val();
    if (value == '' || value == null) {
        $("input[name='detail[" + row + "].ShippingFee']").val(0);
    }
    UnitPrice(row);
    TotalWeight();
});

$(document).on("keyup", "#ManualDiscount", function () {
    if ($("select[name='ManualDiscountType']").val() == "PERCENT") {
        if ($(this).val() * 1 > 100) {
            $(this).val(0);
        }
    }
    else {
        if ($(this).val() * 1 > $("#ViewBagSumPrice").val() * 1) {
            $(this).val(0);
        }
    }
    TotalWeight();
});

$("select[name='ManualDiscountType']").on("change", function (e) {
    TotalWeight();
});

$(document).on("keyup", "#VATValue", function () {
    if ($(this).val() > 100) {
        $(this).val(1);
    }
    TotalWeight();
});

$(document).on("keyup", "#GuestAmountPaid", function () {
    RemainingAmount();
});

function formatNumberForGird() {
    // Định dạng hiển thị tiền : trọng lượng, giá, thành tiền
    $("#TotalWeight strong").number(true);
    $("#SumPrice").number(true);
    $(".detail-Qty").number(true);
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
}

function UnitPrice(row) {
    var QtyValue = $("input[name='detail[" + row + "].ConfirmQty']").val() * 1;
    var Price = $("input[name='detail[" + row + "].Price']").val() * 1;
    var ShippingFee = $("input[name='detail[" + row + "].ShippingFee']").val() * 1;
    // Update Thành tiền
    $("input[name='detail[" + row + "].UnitPrice']").val(QtyValue * (Price + ShippingFee));
    // Update Giá vốn = Giá + Phí vận chuyển
    $("input[name='detail[" + row + "].UnitCOGS']").val(Price + ShippingFee);
    // Update trọng lượng
    var ShippingWeight = $("input[name='detail[" + row + "].ShippingWeight']").val() * 1;
    $("input[name='detail[" + row + "].UnitShippingWeight']").val(ShippingWeight * QtyValue);
}

function TotalWeight() {
    var TotalWeight = 0;
    var Sumprice = 0;
    var TotalQty = 0;
    $(".detail-UnitShippingWeight").each(function () {
        var dataRow = $(this).data("row");
        //console.log(dataRow);
        var Weight = $("input[name='detail[" + dataRow + "].UnitShippingWeight']").val();
        var ConfrimQty = $("input[name='detail[" + dataRow + "].ConfirmQty']").val() * 1;
        TotalWeight += (Number(Weight) * ConfrimQty );
    });
    $(".detail-UnitPrice").each(function () {
        var dataRow = $(this).data("row");
        //console.log(dataRow);
        var Price = $("input[name='detail[" + dataRow + "].UnitPrice']").val();
        Sumprice += Number(Price);
    });
    $(".detail-ConfirmQty").each(function () {
        var dataRow = $(this).data("row");
        //console.log(dataRow);
        var Qty = $("input[name='detail[" + dataRow + "].ConfirmQty']").val();
        TotalQty += Number(Qty);
    });
    //Tổng Qty
    //console.log(TotalWeight);
    $("#TotalQty strong").html(TotalQty);
    $("#ViewBagTotalQty").val(TotalQty);

    //Tổng Weight
    //console.log(TotalWeight);
    $("#TotalWeight strong").html(TotalWeight);
    $("#ViewBagTotalShippingWeight").val(TotalWeight);

    // Tổng cộng
    $("#SumPrice").html(Sumprice);
    $("#ViewBagSumPrice").val(Sumprice);

    // Tính tổng cộng thành tiền
    var Totalprice = 0;
    var Discount = 0;
    var BillDiscount = $("input[name='ManualDiscount']").val();
    var BillDiscountValue = $("select[name='ManualDiscountType']").val();
    if (BillDiscountValue == "CASH") {
        Discount = BillDiscount;
    }
    else {
        Discount = (BillDiscount / 100) * Sumprice;
    }
    var BillVAT = $("input[name='VATValue']").val();
    var BillVATValue = (BillVAT / 100) * (Sumprice - Discount);
    Totalprice = Sumprice - Discount + BillVATValue;

    $("#TotalPrice").html(Totalprice);
    $("#ViewBagTotalPrice").val(Totalprice);
    formatNumberForGird();
    RemainingAmount();
}

function RemainingAmount() {
    var SumPrice = $("#ViewBagTotalPrice").val();
    var GuestAmountPaid = $("#GuestAmountPaid").val();
    //console.log(Number(SumPrice) - Number(GuestAmountPaid));
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

$(document).on("click", "#btnConfirm", function () {
    var SumQty = 0;
    $(".detail-ConfirmQty").each(function () {
        SumQty += ($(this).val() * 1);
    });
    //alert(SumQty);
    if (SumQty <= 0) {
        $("#divPopup #content").html("Bạn chưa xác nhận số lượng sản phẩm nào");
        $("#divPopup").modal("show");
    }
    else {
        var data = $("#frmList").serialize() + "&" + $("#frmHeader").serialize();
        $.ajax({
            type: "POST",
            url: "/PreImportMaster/SaveConfirm",
            data: data,
            success: function (data) {
                if (data == "success") {
                    window.location.assign("/PreImportMaster/Index");
                }
                else {
                    $("#divPopup #content").html(data);
                    $("#divPopup").modal("show");
                }
            }
        });
    }
});