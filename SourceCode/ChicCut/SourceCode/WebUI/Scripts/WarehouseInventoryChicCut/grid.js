
//Sản phẩm
$(document).ready(function () {
    Select2_CustomForList("/ImportMaster/GetProductId", "ProductId");
});

function Total() {
    //TotalEndInventoryQty
    var TotalEndInventoryQty = 0;
    $(".detail-EndInventoryQty").each(function () {
        var dataRow = $(this).data("row");
        var Qty = $("input[name='detail[" + dataRow + "].EndInventoryQty']").val();
        TotalEndInventoryQty += Number(Qty);
    });
    $("#SumEndInventoryQty").html(parseFloat(TotalEndInventoryQty).toFixed(2));


    var TotalActualInventory = 0;
    $(".detail-ActualInventory").each(function () {
        var dataRow = $(this).data("row");
        var Qty = $("input[name='detail[" + dataRow + "].ActualInventory']").val();
        TotalActualInventory += Number(Qty);
    });
    $("#SumActualInventory").html(parseFloat(TotalActualInventory).toFixed(2));

    var TotalAmountDifference = 0;
    $(".detail-AmountDifference").each(function () {
        var dataRow = $(this).data("row");
        var Qty = $("input[name='detail[" + dataRow + "].AmountDifference']").val();
        TotalAmountDifference += Number(Qty);
    });
    $("#SumAmountDifference").html(parseFloat(TotalAmountDifference).toFixed(2));
}


//format Number
//$(".detail-EndInventoryQty").number(true);
//$(".detail-ActualInventory").number(true);
//$(".detail-AmountDifference").number(true);
$("select[name='ProductId']").on("change", function (e) {
    var Productid = e.target.value;
    //var ProductName = e.target.textContent;
    var ProductName = e.target.childNodes[e.target.childNodes.length - 1].textContent;
    var row = $(this).data("row");
    $.ajax({
        type: "POST",
        url: "/WarehouseInventoryChicCut/GetValueSpecifications",
        data: {
                SelectedProductid: Productid
              },
        success: function (data) {
            $("input[name='detail[" + row + "].Specifications']").val(data.Specifications); // giá bán

            // Lưu ProductID lại
            $("input[name='detail[" + row + "].ProductId']").val(Productid);
            $("input[name='detail[" + row + "].EndInventoryQty']").val(data.EndInventoryQty);
            $("input[name='detail[" + row + "].ActualInventory']").val(data.EndInventoryQty);
            $("input[name='detail[" + row + "].ProductName']").val(ProductName);
            $(".detail-ActualInventory").trigger("change");
            Total();
        }
    });
});

$(document).on("change", ".detail-ActualInventory", function () {
    var row = $(this).data("row");
    var AmountDifference = $("input[name='detail[" + row + "].ActualInventory']").val() - $("input[name='detail[" + row + "].EndInventoryQty']").val();
    $("input[name='detail[" + row + "].AmountDifference']").val(AmountDifference);
    Total();
});


