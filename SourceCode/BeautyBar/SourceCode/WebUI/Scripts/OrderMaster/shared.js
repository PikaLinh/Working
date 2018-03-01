$(document).ready(function () {
    //var id = $("select[name='CustomerId']").val();
    var id = $("#IdCustomer").val();
    //alert(id);
    if (id != "") {
        CustomerInfo(id);
    }
    TotalPrice();
});

// Thay đổi Mã Kh => Lấy được thông tin khách hàng tương ứng
$("select[name='CustomerId']").on("change", function (e) {
    //console.log(e.target.value);//id
    //console.log(e.target.textContent);//name
    var CustomerId = e.target.value;
    CustomerInfo(CustomerId);
   
});

function CustomerInfo(CustomerId) {
    $.ajax({
        type: "POST",
        url: "/Sell/GetProfileByCustomerId?CustomerID=" + CustomerId,
        success: function (data) {
            // alert("aaa");
            $("input[name='IdentityCard']").val(data.IdentityCard);
            $("input[name='FullName']").val(data.FullName);
            $("input[name='IdentityCard']").val(data.IdentityCard);
            $("input[name='Phone']").val(data.Phone);
            //if (data.Gender == true && $("#Nam").prop("checked") == false) {
            //    $("#Nu").removeAttr("checked");
            //    $("#Nam").attr('checked', 'checked');
            //    //alert(1);
            //}
            //else if (data.Gender == false && $("#Nu").prop("checked") == false)
            //{
            //    $("#Nam").removeAttr("checked");
            //    $("#Nu").attr('checked', 'checked');
            // // alert(0);
            //}
            $("input[name='Gender']").each(function () {
                $(this).removeAttr("checked");
            });
            if (data.Gender == true) {
                $("#Nam").prop("checked", true);
            }
            else {
                $("#Nu").prop("checked", true);
            }
            // $("input[name='Gender']").val(data.Gender);
            $("input[name='Address']").val(data.Address);
            $("input[name='Email']").val(data.Email);
            var CustomerLevelIdBefore = $("input[name='CustomerLevelId']").val();
            $("input[name='CustomerLevelId']").val(data.CustomerLevelId);
            $("#CustomerLevelName").html(data.CustomerLevelName);
            // Tỉnh
            Select2_Custom("/Sell/GetProvinceId", "ProvinceId", data.ProvinceId, data.ProvinceName, "divProvinceId");
            //// Quận Huyện
            var url = "/Sell/GetDistrictByProvinceId?ProvinceIdSelected=" + data.ProvinceId;
            Select2_Custom(url, "DistrictId", data.DistrictId, data.DistrictName, "divDistrictId");
            Select2_Custom("/Sell/GetSaleId", "SaleId", data.EmployeeId, data.FullNameEmployee, "divSaleId");
            if (CustomerLevelIdBefore != data.CustomerLevelId) {
                var data = $("#frmList").serializeArray();
                $.ajax({
                    type: "POST",
                    url: "/Sell/_DeleteAll",
                    data: data,
                    success: function (data) {
                        $("#tblImportDetail tbody").html(data);
                        TotalPrice();
                        //RemainingAmount();
                    }
                });
            }

        }
    });
}

// Thay đổi tỉnh thành => lấy đc quận - huyện tương ứng
$("select[name='ProvinceId']").on("change", function (e) {
        //console.log(e.target.value);//id
        //console.log(e.target.textContent);//name

        var Provinceid = e.target.value;
        var url = "/Sell/GetDistrictByProvinceId?ProvinceIdSelected=" + Provinceid;
    // Remove 
        Select2_Custom(url, "DistrictId",1, "", "divDistrictId");
        Select2_Custom(url, "DistrictId");


});
// Bước 1 xử lý thêm dòng mới trong CreateList
$("#btnAddNewRow").unbind("click").click(function () {
    var data = $("#frmList").serializeArray();
    $.ajax({
        type: "POST",
        url: "/Sell/_CreatelistInner",
        data: data,
        success: function (data) {
            $("#tblImportDetail tbody").html(data);
            TotalPrice();
        }
    });
});
// Bước 2 xử lý detail-btndelete
$(document).on("click", ".detail-btndelete", function () {
    //var $btn = $(this).button('loading');

    var data = $("#frmList").serializeArray();
    var removeId = $(this).data("row");
    $.ajax({
        type: "POST",
        url: "/Sell/_DeletelistInner?RemoveId=" + removeId,
        data: data,
        success: function (data) {
            //$btn.button('reset');
            $("#tblImportDetail tbody").html(data);
            //TotalWeight();
            TotalPrice();
        }
    });

    // $btn.button('reset');
    return false;
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
    $("#MoneyTransfer").number(true);
    $("#TotalPrice").number(true);
    $("#ViewBagTotalPrice").number(true);
    $("#SumPrice").number(true); 
    $("#BillDiscount").number(true);
    $("#Paid").number(true); 
    $("#GuestAmountPaid").number(true);
    $("#RemainingAmount2 span").number(true);
    $("#ProQty").number(true);
    $("#BillVAT").number(true);
}

// Tính tổng tiền
function TotalPrice() {
    var Sumprice = 0;
    $(".detail-UnitPrice").each(function () {
        var dataRow = $(this).data("row");
        var Price = $("input[name='detail[" + dataRow + "].UnitPrice']").val();
        Sumprice += Number(Price);
    });
    // Tổng Price
    //$("#TotalPrice strong").html(Totalprice);

    // Tổng tạm ViewBagSumPrice
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

$(document).on("keyup", ".detail-Discount", function () {
    var row = $(this).data("row");
    UnitPrice(row);
    TotalPrice();
});

$(document).on("keyup", "#BillDiscount", function () {
    CheckValidDiscount();
    TotalPrice();
    //RemainingAmount();
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
    //RemainingAmount();
});
$("select[name='PaymentMethodId']").on("change", function (e) {
    //alert("aa");
    var PaymentMethodId = $(this).val();
    if (PaymentMethodId == 4) {
        $("#divGuestAmountPaid").hide();
        $("#GuestAmountPaid").val(0);
    }
    else {
        $("#divGuestAmountPaid").show();
    }
    RemainingAmount();
});
function RemainingAmount() {
    $("#GuestAmountPaid").val($("#ViewBagTotalPrice").val());
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