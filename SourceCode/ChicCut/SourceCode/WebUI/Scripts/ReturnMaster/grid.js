
// Thay đổi số lượng => update Trọng lượng , Thành tiền
$(document).on("keyup", ".detail-ReturnQty", function () {
    var row = $(this).data("row");
    UnitPrice(row);
    if ($(this).val() == "" || $(this).val() == null) {
        $(this).val(0);
    }
    // Xét số lượng trả : < Số lượng nhập || < Số lượng tồn kho
    var ReturnQty = $(this).val()*1;
    var ImportQty =  $("input[name='detail[" + row + "].ImportQty']").val() * 1;
    var InventoryQty = $("input[name='detail[" + row + "].InventoryQty']").val() * 1;
    var ReturnedQty = $("input[name='detail[" + row + "].ReturnedQty']").val() * 1;

    if (ReturnQty > (ImportQty - ReturnedQty) || ReturnQty > InventoryQty || ReturnQty < 0) {
        //alert("Số lượng trả phải nhỏ hơn số lượng nhập hoặc nhỏ hơn số lượng tồn kho !");
        $("#divPopup #content").html("'Số lượng trả' phải nhỏ hơn 'số lượng nhập trừ số lượng đã trả' hoặc nhỏ hơn 'số lượng tồn kho' !");
        $("#divPopup").modal("show");
        $(this).val((ImportQty - ReturnedQty) < InventoryQty ? (ImportQty - ReturnedQty) : InventoryQty);
    }
    TotalWeight();
    //RemainingAmount();
});

function UnitPrice(row)
{
    var QtyValue = $("input[name='detail[" + row + "].ReturnQty']").val() * 1;
    var Price = $("input[name='detail[" + row + "].Price']").val() * 1;
    var ShippingFee = $("input[name='detail[" + row + "].ShippingFee']").val() * 1;
    // Update Thành tiền
    $("input[name='detail[" + row + "].UnitPrice']").val(QtyValue * (Price + ShippingFee));
    // Update Giá vốn = Giá + Phí vận chuyển
    $("input[name='detail[" + row + "].UnitCOGS']").val(Price + ShippingFee);
    //// Update trọng lượng
    var ShippingWeight = $("input[name='detail[" + row + "].ShippingWeight']").val()*1;
    $("input[name='detail[" + row + "].UnitShippingWeight']").val(ShippingWeight * QtyValue);
}

// Tính tổng trọng lượng
function TotalWeight() {
    var TotalWeight = 0;
    var Sumprice = 0;
    var TotalQty = 0;
    $(".detail-UnitShippingWeight").each(function () {
        var dataRow = $(this).data("row");
       // console.log(dataRow);
        var Weight = $("input[name='detail[" + dataRow + "].UnitShippingWeight']").val() * 1;
       // console.log(Weight);
        TotalWeight += Number(Weight);
    });
    $(".detail-UnitPrice").each(function () {
        var dataRow = $(this).data("row");
        //console.log(dataRow);
        var Price = $("input[name='detail[" + dataRow + "].UnitPrice']").val();
        Sumprice += Number(Price);
    });
    $(".detail-ReturnQty").each(function () {
        var dataRow = $(this).data("row");
        //console.log(dataRow);
        var Qty = $("input[name='detail[" + dataRow + "].ReturnQty']").val();
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