
//Mã Sản phẩm
$(document).ready(function () {
    Select2_CustomForList("/ImportMaster/GetProductId", "ProductId");
    formatNumberForGird();
    TotalPrice();
});

$("select[name='ProductId']").on("change", function (e) {
    // mostly used event, fired to the original element when the value changes
    //console.log(e.target.value);//id
    //console.log(e.target.textContent);//name
    //console.log($(this).data("row"));//row

    var Productid = e.target.value;
   // alert(ProductName.substring(ProductName.indexOf("|") + 2));
    //var ProductName = e.target.textContent;
    var ProductName = e.target.childNodes[e.target.childNodes.length - 1].textContent;
    var row = $(this).data("row");
    var QtyValue = $("input[name='detail[" + row + "].Quantity']").val();
    if (QtyValue == "") {
        $("input[name='detail[" + row + "].Quantity']").val(1);
    }
    $.ajax({
        type: "POST",
        url: "/Sell/GetValuePriceAndUnitShippingWeight",
        data: {
                SelectedProductid: Productid,
                CustomerLevelId: $("#CustomerLevelId").val()
              },
        success: function (data) {
            $("input[name='detail[" + row + "].Price']").val(data.Price); // giá bán

            //$("input[name='detail[" + row + "].Quantity']").val(data.Quantity);
            // Lưu ProductID lại
            $("input[name='detail[" + row + "].ProductId']").val(Productid);
            // Lưu ProducName lại
            $("input[name='detail[" + row + "].ProductName']").val(ProductName);
            $("input[name='detail[" + row + "].EndInventoryQty']").val(data.EndInventoryQty);

            $("#spanEndInventoryQty-" + row).html(data.EndInventoryQty); // trong vòng lặp => id phải khác nhau
            UnitPrice(row);
            TotalPrice();
            var SumPrice = $("#ViewBagSumPrice").val();
            var GuestAmountPaid = $("#GuestAmountPaid").val();
            $("#RemainingAmount").val(Number(SumPrice) - Number(GuestAmountPaid));
            $("#RemainingAmount2 span").html(Number(SumPrice) - Number(GuestAmountPaid));
        }
    });
});
// Thay đổi giá => update Thành tiền
$(document).on("change", ".detail-DiscountTypeId", function () {
    var row = $(this).data("row");
    UnitPrice(row);
    TotalPrice();
});

// Thay số lượng => update thành tiền
$(document).on("keyup", ".detail-Qty", function () {
    var row = $(this).data("row");
    // Tối thiểu bán 1 đơn vị sản phẩm
    // Chỉ được bán <= số lượng tồn kho
    //var EndInventoryQty = $("input[name='detail[" + row + "].EndInventoryQty']").val()*1;

    //var Value = $(this).val() * 1;
    //if (Value <= 0 || Value >  EndInventoryQty) {
    //    $(this).val(1);
    //}
    if ($(this).val() == 0) {
        $(this).val(1);
    }
    UnitPrice(row);
    TotalPrice();

    var SumPrice = $("#ViewBagSumPrice").val();
    var GuestAmountPaid = $("#GuestAmountPaid").val();
    $("#RemainingAmount").val(Number(SumPrice) - Number(GuestAmountPaid));
    $("#RemainingAmount2 span").html(Number(SumPrice) - Number(GuestAmountPaid));
});

// Thay giá => update thành tiền
//$(document).on("keyup", ".detail-Price", function () {
//    var row = $(this).data("row");
//    UnitPrice(row);
//    TotalPrice();
//});

// Thay Giá trị giảm giá => update thành tiền
$(document).on("keyup", ".detail-Discount", function () {
    var row = $(this).data("row");
    UnitPrice(row);
    TotalPrice();
});
// Thay Loại giảm giá => update thành tiền
$(document).on("keyup", ".detail-Discount", function () {
    var row = $(this).data("row");
    UnitPrice(row);
    TotalPrice();
});
// Tính thành tiền
function UnitPrice(row) {
    // Lấy giá trị số lượng, giá
    var Price = $(".detail-Price-"+row).val();
    var Quantity = $("input[name='detail[" + row + "].Quantity']").val();
    $("input[name='detail[" + row + "].UnitPrice']").val(Number((Price * Quantity)));
}

// Change scanBarcode
$(document).on("keyup", "#scanBarcode", function () {
    var leng = $("#scanBarcode").val().length;
    if (leng == 8) {
        //alert(leng);
        //Bước 1 : AddItem
        var data = $("#frmList").serializeArray();
        $.ajax({
            type: "POST",
            url: "/PreOrderMaster/_CreatelistInner?CustomerLevelId=" + $("#CustomerLevelId").val(),
            data: data,
            success: function (data) {
                $("#tblImportDetail tbody").html(data);
               // UnitPrice(row);
                TotalPrice();
            }
        });
        //Bước 2 :  $(this).val("");
        $(this).val("");
        $(this).focus();
        $("#ProQty").val(1);
    }
});
// ProQty
$(document).on("keyup", "#ProQty", function () {

    if ($(this).val() == 0) {
        $(this).val(1);
    }
});
