
// Thay đổi số lượng => update Trọng lượng , Thành tiền
$(document).on("keyup", ".detail-ReturnQuantity ", function () {
    var row = $(this).data("row");
    UnitPrice(row);
    if ($(this).val() == "" || $(this).val() == null) {
        $(this).val(0);
    }

    // Xét số lượng trả : < Số lượng nhập || < Số lượng tồn kho
    var ReturnQty = $(this).val()*1;
    var SellQuantity = $("input[name='detail[" + row + "].SellQuantity']").val() * 1;
    var ReturnedQuantity = $("input[name='detail[" + row + "].ReturnedQuantity']").val() * 1;

    if (ReturnQty > (SellQuantity - ReturnedQuantity) ||  ReturnQty < 0) {
        //alert("Số lượng trả phải nhỏ hơn số lượng nhập hoặc nhỏ hơn số lượng tồn kho !");
        $("#divPopup #content").html("'Số lượng trả' phải nhỏ hơn 'số lượng đã bán' trừ số 'lượng đã trả'!");
        $("#divPopup").modal("show");
        $(this).val((SellQuantity - ReturnedQuantity));
    }
    TotalPrice();
    //RemainingAmount();
});

function UnitPrice(row)
{
    var QtyValue = $("input[name='detail[" + row + "].ReturnQuantity']").val() * 1;
    var Price = $("input[name='detail[" + row + "].Price']").val() * 1;
    // Update Thành tiền
    $("input[name='detail[" + row + "].UnitPrice']").val(QtyValue * Price );
}

// Tính tổng trọng lượng
function TotalPrice() {
    var Sumprice = 0;
    var TotalQty = 0;
    $(".detail-UnitPrice").each(function () {
        var dataRow = $(this).data("row");
        //console.log(dataRow);
        var Price = $("input[name='detail[" + dataRow + "].UnitPrice']").val();
        Sumprice += Number(Price);
    });
    $(".detail-ReturnQuantity ").each(function () {
        var dataRow = $(this).data("row");
        //console.log(dataRow);
        var Qty = $("input[name='detail[" + dataRow + "].ReturnQuantity']").val();
        TotalQty += Number(Qty);
    });
    //Tổng Qty
    //console.log(TotalWeight);
    $("#TotalQty strong").html(TotalQty);
    $("#ViewBagTotalQty").val(TotalQty);

    // Tổng cộng
    $("#SumPrice").html(Sumprice);
    $("#ViewBagSumPrice").val(Sumprice);

    // Tính tổng thành tiền
    var Totalprice = 0;
    var Discount = 0;
    var BillDiscount = $("input[name='BillDiscount']").val();
    var BillDiscountValue = $("select[name='BillDiscountTypeId']").val();
    if (BillDiscountValue == 1) {
        Discount = BillDiscount;
    }
    else {
        Discount = (BillDiscount / 100) * Sumprice;
    }
    var BillVAT = $("input[name='BillVAT']").val();
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