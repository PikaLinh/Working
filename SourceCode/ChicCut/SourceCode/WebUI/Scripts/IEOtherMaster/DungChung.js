// Thay đổi Mã Kh => Lấy được thông tin khách hàng tương ứng
$("select[name='CustomerName']").on("change", function (e) {
    //console.log(e.target.value);//id
    //console.log(e.target.textContent);//name
    var CustomerId = e.target.value;
    $.ajax({
        type: "POST",
        url: "/IEOtherMaster/GetProfileByCustomerId?CustomerID=" + CustomerId,
        success: function (data) {
            // alert("aaa");
            $("input[name='Phone']").val(data.Phone);
        }
    });

});
// Bước 1 xử lý thêm dòng mới trong CreateList abc
$("#btnAddNewRow").unbind("click").click(function () {
    var data = $("#frmList").serializeArray();
    $.ajax({
        type: "POST",
        url: "/IEOtherMaster/_CreatelistInner",
        data: data,
        success: function (data) {
            $("#tblImportDetail tbody").html(data);
            TotalWeight();
        }
    });
});

//Định dạng hiển thị số tiền
function formatNumberForGird() {
    // Định dạng hiển thị tiền : trọng lượng, giá, thành tiền
    //$("#TotalWeight strong").number(true);
    //$("#TotalPrice strong").number(true);
    $(".detail-Qty").number(true);
    $(".detail-Price").number(true);
    $(".unit-price").number(true);
    $(".detail-Discount").number(true);
    $("#Paid").number(true);
    // $("#MoneyTransfer").number(true);
    $("#TotalPrice").number(true);
    $("#ViewBagTotalPrice").number(true);
    $("#SumPrice").number(true);
    $("#BillDiscount").number(true);
    $("#Money").number(true);
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
        //var QtyValue = $("input[name='detail[" + row + "].Qty']").val();
        //var Price = $("input[name='detail[" + row + "].Price']").val();
        //// Update Thành tiền
        //$("input[name='detail[" + row + "].UnitPrice']").val(QtyValue * Price);
        UnitPrice(row);

    });
   /* $("select[name='ProductId']").on("change", function (e) {
        // mostly used event, fired to the original element when the value changes
        //console.log(e.target.value);//id
        //console.log(e.target.textContent);//name
        //console.log($(this).data("row"));//row

        var Productid = e.target.value;
        //var ProductName = e.target.textContent;
        var ProductName = e.target.childNodes[e.target.childNodes.length - 1].textContent;
        var row = $(this).data("row");
        var QtyValue = $("input[name='detail[" + row + "].Qty']").val();
        var PriceTotal
        $.ajax({
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
                //$("input[name='detail[" + row + "].UnitPrice']").val(data.Price * QtyValue);
                TotalWeight();
            }
        });
    });*/



    // Bước 1 xử lý thêm dòng mới trong CreateList
    $("#btnAddNewRow").unbind("click").click(function () {

        var data = $("#frmList").serializeArray();
        $.ajax({
            type: "POST",
            url: "/IEOtherMaster/_CreatelistInner",
            data: data,
            success: function (data) {
                $("#tblImportDetail tbody").html(data);
                TotalWeight();
                TotalQty();
                $("select[name='ProductId']").trigger("change");
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
            url: "/IEOtherMaster/_DeletelistInner?RemoveId=" + removeId,
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
// Tính tổng trọng lượng
function TotalWeight() {
    var TotalPrice = 0;
    $(".detail-UnitPrice").each(function () {
        var dataRow = $(this).data("row");
        //console.log(dataRow);
        var Price = $("input[name='detail[" + dataRow + "].UnitPrice']").val();
        TotalPrice += Number(Price);
    });
    // Tổng Price
    //console.log(TotalWeight);
    $("#SumPrice").html(TotalPrice);
    $("#ViewBagTotalPrice").val(TotalPrice);

    formatNumberForGird();
}

//Tính tổng số lượng sản phẩm xuất kho
function TotalQty() {
    var TotalQuantity = 0;
    $(".detail-Qty").each(function () {
        var dataRow = $(this).data("row");
        var Qty = $("input[name='detail[" + dataRow + "].Qty']").val();
        TotalQuantity += Number(Qty);
    });
    $("#SumQty").html(TotalQuantity);
}