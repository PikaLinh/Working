
//Sản phẩm
$(document).ready(function () {
    Select2_CustomForList("/IEOtherMaster/GetProductId", "ProductId");
    formatNumberForGird();
});

$("select[name='ProductId']").on("change", function (e) {
    // mostly used event, fired to the original element when the value changes
    //console.log(e.target.value);//id
    //console.log(e.target.textContent);//name
    //console.log($(this).data("row"));//row

    var Productid = e.target.value;
    //var ProductName = e.target.textContent;
    var ProductName = e.target.childNodes[e.target.childNodes.length - 1].textContent;
    var row = $(this).data("row");
    var QtyValue = $("input[name='detail[" + row + "].Qty']").val();
    if (QtyValue == 0) {
        $("input[name='detail[" + row + "].Qty']").val(1);
    }
    var PriceTotal

    // Lưu ProductID lại
    $("input[name='detail[" + row + "].ProductId']").val(Productid);
    // Lưu ProducName lại
    $("input[name='detail[" + row + "].ProductName']").val(ProductName);
   /* $.ajax({
        type: "POST",
        url: "/ImportMaster/GetValuePriceAndUnitShippingWeight",
        data: {
                 SelectedProductid: Productid
              },
        success: function (data) {
            //$("input[name='detail[" + row + "].Price']").val(data.Price);
            //UnitPrice(row);
            // Lưu ProductID lại
            $("input[name='detail[" + row + "].ProductId']").val(Productid);
            // Lưu ProducName lại
            $("input[name='detail[" + row + "].ProductName']").val(ProductName);
            // Lưu trọng lượng lại
            $("input[name='detail[" + row + "].ShippingWeight']").val(data.ShippingWeight);
            // Get value now
            $("input[name='detail[" + row + "].UnitShippingWeight']").val(data.ShippingWeight * QtyValue);
            TotalWeight();

        }
    });*/
});
// Thay đổi số lượng => update Trọng lượng , Thành tiền
$(document).on("keyup", ".detail-Qty", function () {
    var row = $(this).data("row");
    UnitPrice(row);
    TotalWeight();
});

// Thay đổi số lượng => update Trọng lượng , Thành tiền
$(document).on("keyup", ".detail-Price", function () {
    var row = $(this).data("row");
    UnitPrice(row);
    TotalWeight();
});

function UnitPrice(row)
{
    var QtyValue = $("input[name='detail[" + row + "].Qty']").val();
    var Price = $("input[name='detail[" + row + "].Price']").val();
    // Update Thành tiền
    $("input[name='detail[" + row + "].UnitPrice']").val(QtyValue * Price);
    // Update trọng lượng
    var ShippingWeight = $("input[name='detail[" + row + "].ShippingWeight']").val();
    $("input[name='detail[" + row + "].UnitShippingWeight']").val(ShippingWeight * QtyValue);
}