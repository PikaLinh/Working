$("#divExchangeRate").hide();
$("#ExchangeRate").val(1);
$("#ExchangeDate").val("2016-05-01");
// Tính tổng trọng lượng
function TotalWeight() {
    var TotalWeight = 0;
    var Sumprice = 0;
    var TotalQty = 0;
    $(".detail-UnitShippingWeight").each(function () {
        var dataRow = $(this).data("row");
        //console.log(dataRow);
        var Weight = $("input[name='detail[" + dataRow + "].UnitShippingWeight']").val();
        TotalWeight += Number(Weight);
    });
    $(".detail-UnitPrice").each(function () {
        var dataRow = $(this).data("row");
        //console.log(dataRow);
        var Price = $("input[name='detail[" + dataRow + "].UnitPrice']").val();
        Sumprice += Number(Price);
    });
    $(".detail-Qty").each(function () {
        var dataRow = $(this).data("row");
        //console.log(dataRow);
        var Qty = $("input[name='detail[" + dataRow + "].Qty']").val();
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
    // INSERT giá trị quy ra tiền đưa vào database : TotalBillDiscount và TotalVAT
    $("#ViewBagTotalBillDiscount").val(Discount);
    $("#ViewBagTotalVAT").val(BillVATValue);

    $("#TotalPrice").html(Totalprice);
    $("#ViewBagTotalPrice").val(Totalprice);
    formatNumberForGird();
    RemainingAmount();
}

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
    //RemainingAmount();
});
$("select[name='ManualDiscountType']").on("change", function (e) {
    TotalWeight();
    // RemainingAmount();
});
$(document).on("keyup", "#VATValue", function () {
    if ($(this).val() > 100) {
        $(this).val(1);
    }
    TotalWeight();
    // RemainingAmount();
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
        $("#RemainingAmount").val(Number(SumPrice));
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
    $("#ExchangeRate").number(true, 3);
}
(function ($) {
    // Thay đổi giá
    $(document).on("keyup", ".detail-Price", function () {
        TotalWeight();
    });
    // Thay đổi trọng lượng =>  update TotalWeight
    $(document).on("keyup", ".detail-UnitShippingWeight", function () {
        TotalWeight();
    });
    // Thay đổi thành tiền =>  update TotalPrice
    $(document).on("keyup", ".detail-UnitPrice", function () {
        TotalWeight();
    });

    //// Thay đổi giá => Update lại Thành tiền = số lượng * giá
    $(document).on("keyup", ".detail-Price", function () {
        var row = $(this).data("row");
        UnitPrice(row);
        TotalWeight();
        // RemainingAmount();
    });

    ////Thay đổi Note => Update Price
    $(document).on("change", ".detail-Note", function () {
        var row = $(this).data("row");
        var Note = $("select[name='detail[" + row + "].Note']").val();
        var Price = $("input[name='detail[" + row + "].detailPrice']").val();
        if (Note == "KM") {
            $("input[name='detail[" + row + "].Price']").val(0);
        }
        else {
            $("input[name='detail[" + row + "].Price']").val(Price);
        }
        UnitPrice(row);
        TotalWeight();
    });

    //$("select[name='ProductId']").on("change", function (e) {
    //    // mostly used event, fired to the original element when the value changes
    //    //console.log(e.target.value);//id
    //    //console.log(e.target.textContent);//name
    //    //console.log($(this).data("row"));//row
    //    var Productid = e.target.value;
    //    var ProductName = e.target.textContent;
    //    var row = $(this).data("row");
    //    var QtyValue = $("input[name='detail[" + row + "].Qty']").val();
    //    var Note = $("select[name='detail[" + row + "].Note']").val();

    //    var PriceTotal
    //    $.ajax({
    //        type: "POST",
    //        url: "/ImportMaster/GetValuePriceAndUnitShippingWeight",
    //        data: {
    //            SelectedProductid: Productid
    //        },
    //        success: function (data) {
    //            //if (Note == "KM") {
    //            //    $("input[name='detail[" + row + "].Price']").val(0);
    //            //}
    //            //else
    //            //{
    //            //    $("input[name='detail[" + row + "].Price']").val(0);
    //            //}
    //            //UnitPrice(row);
    //            //// Lưu ProductID lại
    //            //$("input[name='detail[" + row + "].ProductId']").val(Productid);
    //            //// Lưu ProducName lại
    //            //$("input[name='detail[" + row + "].ProductName']").val(ProductName);

    //            //// Lưu trọng lượng lại
    //            //$("input[name='detail[" + row + "].ShippingWeight']").val(data.ShippingWeight);
    //            //// Get value now
    //            //$("input[name='detail[" + row + "].UnitShippingWeight']").val(data.ShippingWeight * QtyValue);
    //            //$("input[name='detail[" + row + "].UnitPrice']").val(data.Price * QtyValue);
    //            //TotalWeight();
    //           // RemainingAmount();
    //        }
    //    });
    //});


    $(document).on("change", "#CurrencyId", function () {
        var CurrencyId = $(this).val();
        if (CurrencyId == 1) {
            $("#divExchangeRate").hide();
            $("#ExchangeRate").val(1);
            $("#ExchangeDate").val("2016-05-01");
        }
        else {
            $("#divExchangeRate").show();
            $.ajax({
                type: "POST",
                url: "/ImportMaster/GetExchangeRateBy",
                data: {
                    CurrencyIdSelect: CurrencyId
                },
                dataType: "json",
                success: function (jsonData) {
                    $("#ExchangeRate").val(jsonData.ExchangeRate);
                    $("#ExchangeDate").val(jsonData.ExchangeDate);
                    //console.log(jsonData.ExchangeRate);
                    //console.log(jsonData.ExchangeDate);
                }
            });
            return false;

        }

    });

    // Bước 1 xử lý thêm dòng mới trong CreateList
    $("#btnAddNewRow").unbind("click").click(function () {
        var data = $("#frmList").serializeArray();
        $.ajax({
            type: "POST",
            url: "/ImportMaster/_CreatelistInner",
            data: data,
            success: function (data) {
                $("#tblImportDetail tbody").html(data);
                $("#tblImportDetail tbody tr").each(function (index, e) {
                    UnitPrice(index);
                });
                TotalWeight();
            }
        });

        // $btn.button('reset');
    });
    // Bước 2 xử lý tetail-btndelete
    $(document).on("click", ".detail-btndelete", function () {
        //var $btn = $(this).button('loading');

        var data = $("#frmList").serializeArray();
        var removeId = $(this).data("row");
        $.ajax({
            type: "POST",
            url: "/ImportMaster/_DeletelistInner?RemoveId=" + removeId,
            data: data,
            success: function (data) {
                //$btn.button('reset');
                $("#tblImportDetail tbody").html(data);
                TotalWeight();
            }
        });
        // $btn.button('reset');
        return false;
    });
})(jQuery);

