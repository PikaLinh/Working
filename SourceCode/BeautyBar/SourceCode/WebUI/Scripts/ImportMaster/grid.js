
//Sản phẩm
$(document).ready(function () {
    Select2_CustomForList("/ImportMaster/GetProductId", "ProductId");
    formatNumberForGird();
});

$("select[name='ProductId']").on("change", function (e) {
    // mostly used event, fired to the original element when the value changes
    //console.log(e.target.value);//id
    //console.log(e.target.textContent);//name
    //console.log($(this).data("row"));//row
    var Productid = e.target.value;
    var ProductName = e.target.childNodes[e.target.childNodes.length - 1].textContent;
    //var ProductName = e.target.textContent;
    var row = $(this).data("row");
    var QtyValue = $("input[name='detail[" + row + "].Qty']").val();
    if (QtyValue == 0) {
        $("input[name='detail[" + row + "].Qty']").val(1);
    }

    //var ShippingFeeValue = $("input[name='detail[" + row + "].ShippingFee']").val();
    //if (ShippingFeeValue == "") {
    //    $("input[name='detail[" + row + "].ShippingFee']").val(0);
    //}
    $.ajax({
        type: "POST",
        url: "/ImportMaster/GetValuePriceAndUnitShippingWeight",
        data: {
            SelectedProductid: Productid
        },
        success: function (data) {
            $("select[name='detail[" + row + "].Note']").val('--');
            $("input[name='detail[" + row + "].Price']").val(data.Price);
            $("input[name='detail[" + row + "].detailPrice']").val(data.Price);

            $("input[name='detail[" + row + "].ShippingFee']").val(data.ShippingFee);
            UnitPrice(row);
            // Lưu ProductID lại
            $("input[name='detail[" + row + "].ProductId']").val(Productid);
            // Lưu ProducName lại
            $("input[name='detail[" + row + "].ProductName']").val(ProductName);

            // Lưu trọng lượng lại
            $("input[name='detail[" + row + "].ShippingWeight']").val(data.ShippingWeight);
            // Get value now
            $("input[name='detail[" + row + "].UnitShippingWeight']").val(data.ShippingWeight * QtyValue);
            TotalWeight();
            //RemainingAmount();
        }
    });
});
// Thay đổi số lượng => update Trọng lượng , Thành tiền
$(document).on("keyup", ".detail-Qty", function () {
    var row = $(this).data("row");
    var Value = $("input[name='detail[" + row + "].Qty']").val() * 1;
    if (Value <= 0) {
        $("input[name='detail[" + row + "].Qty']").val(1);
    }

    UnitPrice(row);
    TotalWeight();
    //RemainingAmount();
});

$(document).on("keyup", ".detail-ShippingFee", function () {
    var row = $(this).data("row");
    UnitPrice(row);
    TotalWeight();
    //RemainingAmount();
});

function UnitPrice(row) {
    var QtyValue = $("input[name='detail[" + row + "].Qty']").val() * 1;
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

