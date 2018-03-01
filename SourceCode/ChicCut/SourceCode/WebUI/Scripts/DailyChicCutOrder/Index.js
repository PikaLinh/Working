$(document).ready(function () {
    ReloadData();
    $("#divMessenger").hide();
});
$(document).on("click", ".OrderStatusId", function () {
    $(".OrderStatusId").removeClass("btn-danger");
    $(this).addClass("btn-danger");
    ReloadData();
});
function GetAllServicePartital(Orderstatusid) {
    $.ajax({
        type: "POST",
        url: "/DailyChicCutOrder/_GetAllServicePartital?Orderstatusid=" + Orderstatusid,
        success: function (data) {
            $("#contentAllService").html(data);
        }
    });
}
function ReloadData() {
    var Orderstatusid = $(".OrderStatusId").filter(".btn-danger").data("id");
    GetAllServicePartital(Orderstatusid);
}
$(document).on("click", ".OrderId", function () {
    var OrderId = $(this).data("id");
    //alert(OrderId);
    $.ajax({
        type: "POST",
        url: "/DailyChicCutOrder/_UpdateOrder?OrderId=" + OrderId,
        success: function (data) {
            $("#divPopupOrder #contentOrder").html(data);
            $("#divPopupOrder").modal("show");
            var OrderStatusIdNow = $("#OrderStatusIdNow").val();
            $(".action").css("display", "none");
            $("#Calcel-OrderId").css("display", "inline-block");
            switch (Number(OrderStatusIdNow)) {
                case 1:
                    {
                        $("#StatusNow").html("Đang chờ");
                        $("#serve-immediately").css("display", "inline-block");
                    }
                    break;
                case 2:
                    {
                        $("#StatusNow").html("Đang phục vụ");
                        $("#pay").css("display", "inline-block");
                    } break;
                case 3: $("#StatusNow").html("Đã tính tiền"); break;
                case 4: $("#StatusNow").html("Huỷ"); break;
            }
        }
    });
});

$(document).on("click", "#btnAddNewOrder", function () {
    $.ajax({
        type: "POST",
        url: "/DailyChicCutOrder/_UpdateOrder",
        success: function (data) {
            $(".action").css("display", "none");
            $("#StatusNow").html("Thêm mới");
            $("#Calcel-OrderId").css("display", "none");
            $("#divPopupOrder #contentOrder").html(data);
            $("#divPopupOrder").modal("show");
        }
    });
});


//#region Xử lý chọn dịch vụ
$(document).on("click", ".ServiceId", function () {
    //alert("123");
    var ServiceId = $(this).data("id");
    var ServiceName = $(this).text();
    var Price = $("input[name='Price_Of_ServiceId_" + ServiceId + "']").val();
    var data = $("#frmList").serialize();
    // alert(ServiceName + " " + ServiceId + " " + Price);
    //Duyệt xem đã tồn tại dịch vụ đó chưa.
    var ServiceExist = false;
    $(".details-ServiceId").each(function () {
        var row = $(this).data("row");
        if ($(this).val() == ServiceId) {
            var Quantity = $("input[name='details[" + row + "].Qty']").val();
            $("input[name='details[" + row + "].Qty']").val(Number(Quantity) + 1);
            UnitPrice(row);
            TotalPrice();
            ServiceExist = true;
            return false; // nếu return true nó tương tự lệnh continue trong vòng lòng thông thường.
        }
    });
    var OrderId = $("#OrderId").val();
    if (!ServiceExist || Number(OrderId) > 0) {
        $.ajax({
            type: "POST",
            url: "/DailyChicCutOrder/_DailyChicCutOrderDetailInnerInfo?ServiceId=" + ServiceId + "&OrderId=" + OrderId,
            data: data,
            success: function (data) {
                $("#tblOrderDetail tbody").html(data);
                TotalPrice();
            }
        });
    }
});
//#endregion

//#region Xử lý chọn Sản phẩm
$(document).on("click", ".ProductId", function () {
    //alert("123");
    var ProductId = $(this).data("id");
    var ProductName = $(this).text();
    var Price = $("input[name='Price_Of_ProductId_" + ProductId + "']").val();
    var data = $("#frmList").serialize();
    // alert(ProductName + " " + ProductId + " " + Price);
    //Duyệt xem đã tồn tại dịch vụ đó chưa.
    var ServiceExist = false;
    $(".details-ProductId").each(function () {
        var row = $(this).data("row");
        if ($(this).val() == ProductId) {
            var Quantity = $("input[name='details[" + row + "].Qty']").val();
            $("input[name='details[" + row + "].Qty']").val(Number(Quantity) + 1);
            UnitPrice(row);
            TotalPrice();
            ServiceExist = true;
            return false; // nếu return true nó tương tự lệnh continue trong vòng lòng thông thường.
        }
    });
    var OrderId = $("#OrderId").val();
    if (!ServiceExist || Number(OrderId) > 0) {
        $.ajax({
            type: "POST",
            url: "/DailyChicCutOrder/_DailyChicCutOrderDetailInnerInfo?ProductId=" + ProductId + "&OrderId=" + OrderId,
            data: data,
            success: function (data) {
                $("#tblOrderDetail tbody").html(data);
                TotalPrice();
            }
        });
    }
});
//#endregion

//#region Xử lý nút Lưu
$(document).on("click", "#btnSaveOrder", function () {
    var OrderId = $("#OrderId").val();
    //alert(OrderId);
    if ($("select[name='CustomerId']").val() == "" || $("select[name='CustomerId']").val() == null) {
        //$("#divPopupInner #contentInner").html("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
        //$("#divPopupInner").modal("show");
        $("#divMessenger").removeAttr("class");
        $("#divMessenger").addClass("alert alert-danger");
        $("#Messenger").html("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
        $("#divMessenger").fadeToggle();
        // alert("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
    }
    else {
        loading2();
        var data = $("#frmList").serializeAnything();
        $.extend(data, $("#frmHeader").serializeAnything());
        var HairTypeId = $(".HairTypeId").filter(".btn-danger").data("hairtypeid");
        if (HairTypeId != undefined) {
            $.extend(data, { HairTypeId: HairTypeId });
        }

        if (OrderId > 0) { // Sửa đơn hàng (Thêm dịch vụ)
            //alert("sửa đơn hàng: thông tin khách hàng, giảm giá ?");
            var Note = $("#Note").val();
            var BillDiscount = $("#BillDiscount").val();
            var BillDiscountTypeId = $("#BillDiscountTypeId").val();
            var StaffId = $("#StaffId").val();
            var PaymentMethodId = $("select[name='PaymentMethodId']").val();
            var holiday = $("input[name='isHoliday']:checked").val();
            $.ajax({
                type: "POST",
                url: "/DailyChicCutOrder/SaveEditOrder",
                data: {
                    Note: Note,
                    BillDiscount: BillDiscount,
                    BillDiscountTypeId: BillDiscountTypeId,
                    StaffId: StaffId,
                    OrderId: OrderId,
                    PaymentMethodId: PaymentMethodId,
                    isHoliday: holiday
                },
                success: function (data) {
                    if (data == "success") {
                        window.location.assign("/DailyChicCutOrder/Index");
                    }
                    else {
                        $("#divMessenger").removeAttr("class");
                        $("#divMessenger").addClass("alert alert-danger");
                        $("#Messenger").html(data);
                        $("#divMessenger").fadeToggle();
                    }
                }
            });
        }
        else { // Thêm mới đơn hàng
            //alert("Thêm mới đơn hàng");
            $.ajax({
                type: "POST",
                url: "/DailyChicCutOrder/SaveAddNewOrder",
                data: data,
                success: function (data) {
                    if (data == "success") {
                        window.location.assign("/DailyChicCutOrder/Index");
                    }
                    else {
                        $("#divMessenger").removeAttr("class");
                        $("#divMessenger").addClass("alert alert-danger");
                        $("#Messenger").html(data);
                        $("#divMessenger").fadeToggle();
                    }
                }
            });
        }
    }
});
//#endregion
// Tính tổng tiền
function TotalPrice() {
    var Sumprice = 0;
    var TotalUnitCOGS = 0;
    var TotalAdditionalPrice = 0;
    $(".details-UnitPrice").each(function () {
        var dataRow = $(this).data("row");
        //console.log(dataRow);
        var Price = $("input[name='details[" + dataRow + "].UnitPrice']").val();
        Sumprice += (parseInt(Price.replace(/,/g, ''), 10));

        ////Lấy ServiceCategoryId và isHoliday để tính phụ thu nếu là ngày lễ
        //var ServiceCategoryId = $("input[name='details[" + dataRow + "].ServiceCategoryId']").val();
        //var holiday = $("input[name='isHoliday']:checked").val();
        ////Tính tổng phụ thu theo từng dịch vụ
        ////1. Không phải là sản phẩm => ServiceCategoryId != ""
        ////2. Trừ dịch vụ "Cắt" => ServiceCategoryId != "10"
        //if ((ServiceCategoryId != "10" && ServiceCategoryId != "") && holiday == 2) {
        //    //Phụ thu = Thành tiền * 15%
        //    TotalAdditionalPrice += (parseInt(Price.replace(/,/g, ''), 10)) * 15 / 100;
        //}
    });
    //Phụ thu
    $("#SumAdditionalPrice").html(TotalAdditionalPrice);
    $("#ViewBagAdditionalPrice").val(TotalAdditionalPrice);

    $(".details-UnitCOGS").each(function () {
        var dataRow = $(this).data("row");
        //console.log(dataRow);
        var UnitCOGS = $("input[name='details[" + dataRow + "].UnitCOGS']").val();
        TotalUnitCOGS += (parseInt(UnitCOGS.replace(/,/g, ''), 10));
    });

    //Tổng vốn
    //console.log(TotalWeight);
    $("#ViewBagCOGSOfOrder").val(TotalUnitCOGS);

    // Tổng cộng
    $("#SumPrice").html(Sumprice);
    $("#ViewBagSumPrice").val(Sumprice);
    console.log("Sumprice:" + Sumprice);

    // Tính tổng cộng thành tiền

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
    //Ngày thường => không có phụ thu
    //Totalprice = Sumprice - Discount;
    
    //Ngày lễ => có phụ thu
    Totalprice = Sumprice + TotalAdditionalPrice - Discount;
    

    // INSERT giá trị quy ra tiền đưa vào database : TotalBillDiscount 
    $("#ViewBagTotalBillDiscount").val(Discount);

    $("#TotalPrice").html(Totalprice);
    //var a = $("#ResultSumPrice").val();
    //$("#TotalPrice").html(a);

    $("#ViewBagTotal").val(Totalprice);
    formatNumberForGird();
}
$(document).on("keyup", "#BillDiscount", function () {
    CheckValidDiscount();
    TotalPrice();
});

$(document).on("change", "#BillDiscountTypeId", function () {
    CheckValidDiscount();
    TotalPrice();
    //RemainingAmount();
});


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


function formatNumberForGird() {
    $("#SumAdditionalPrice").number(true);
    $("#SumPrice").number(true);
    $("#TotalPrice").number(true);
    $(".detail-Qty").number(true);
}

$(document).on("click", "#closedivMessenger", function () {
    $("#divMessenger").fadeOut();
});

//Nút phục vụ ngay
$(document).on("click", "#serve-immediately", function () {
    var OrderId = $("#OrderId").val();
    $.ajax({
        type: "POST",
        url: "/DailyChicCutOrder/UpdateOrderServeImmediately?OrderId=" + OrderId,
        success: function (data) {
            if (data == "success") {
                window.location.assign("/DailyChicCutOrder/Index");
            }
            else {
                $("#divMessenger").removeAttr("class");
                $("#divMessenger").addClass("alert alert-danger");
                $("#Messenger").html(data);
                $("#divMessenger").fadeToggle();
            }
        }
    });
});

//Nút thanh toán
$(document).on("click", "#pay", function () {
    //gom dữ liệu bỏ vào object OrderMaster
    var Master = $("#frmHeader").serializeAnything();
    var Details = $("#frmList").serializeAnything();
    var OrderMaster = $.extend(Master, Details);
    OrderMaster = $.extend(OrderMaster, { OrderId: $("#OrderId").val() });
    //Post lên server
    $.ajax({
        type: "POST",
        url: "/DailyChicCutOrder/UpdateOrderPay",
        dataType: "json",
        data: OrderMaster,
        success: function (result, stt, jqXHR) {
            if (result.Success == true) {
                //Nếu thành công bật khung in
                console.log(result.Data)
                //In
                openPrint($("#tmplInvoicePrint").html(), {
                    Order: result.Data
                });
                //Đóng popup
                $("#divPopupOrder").modal('hide')
                //Load lại dữ liệu
                ReloadData();
            }
            else {
                $("#divMessenger").removeAttr("class");
                $("#divMessenger").addClass("alert alert-danger");
                $("#Messenger").html(result.Data);
                $("#divMessenger").fadeToggle();
            }
        }
    });
});


//#region Nút huỷ dịch vụ này

$(document).on("click", ".btn-xoa", function (e) {
    var CustomerName = $("#FullName").val() + " - " + $("#Phone").val();
    $("#confirm-delete .modal-body strong").html(CustomerName);
    $("#divPopupOrder").modal("hide");
});

$('#confirm-delete').on('click', '.btn-ok', function (e) {
    var $modalDiv = $(e.delegateTarget);
    var OrderId = $("#OrderId").val();
    console.log(OrderId);
    $.ajax({
        type: "POST",
        url: "/DailyChicCutOrder/UpdateOrderCalcelOrderId?OrderId=" + OrderId,
        success: function (data) {
            if (data == "success") {
                window.location.assign("/DailyChicCutOrder/Index");
            }
            else {
                $("#divMessenger").removeAttr("class");
                $("#divMessenger").addClass("alert alert-danger");
                $("#Messenger").html(data);
                $("#divMessenger").fadeToggle();
            }
        }
    });
});

$(document).on("click", ".btn-show-popup-order", function () {
    $("#divPopupOrder").modal("show");
});

//#endregion
function numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}


$(".Price").number(true);
$(".unit-price").number(true);
formatNumberForGird();
// Thay đổi giá => update Thành tiền
$(document).on("change", ".detail-DiscountTypeId", function () {
    var row = $(this).data("row");
    UnitPrice(row);
    TotalPrice();
});

// Thay số lượng => update thành tiền
$(document).on("change", ".details-Qty", function () {
    var row = $(this).data("row");
    // Tối thiểu bán 1 đơn vị sản phẩm
    // Chỉ được bán <= số lượng tồn kho
    //var EndInventoryQty = $("input[name='detail[" + row + "].EndInventoryQty']").val()*1;

    if ($.isNumeric($(this).val()) == false || $(this).val() <= 0) {
        $(this).val(1);
    }
    var Qty = $(this).val();
    var OrderId = $("#OrderId").val();
    var ServiceId = $("input[name='details[" + row + "].ServiceId']").val();
    if (Number(OrderId) > 0) { // sửa: Cập nahatj 
        $.ajax({
            type: "POST",
            url: "/DailyChicCutOrder/_DailyChicCutOrderDetailInnerInfoUpdateQty?ServiceId=" + ServiceId + "&OrderId=" + OrderId + "&Qty=" + Qty,
            success: function (data) {
                if (data != "success") {
                    $("#divMessenger").removeAttr("class");
                    $("#divMessenger").addClass("alert alert-danger");
                    $("#Messenger").html(data);
                    $("#divMessenger").fadeToggle();
                }
            }
        });
    }
    UnitPrice(row);
    TotalPrice();
    $(this).number(true);

});

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
    var Price = $("input[name='details[" + row + "].Price']").val();
    var Quantity = $("input[name='details[" + row + "].Qty']").val();
    var COGS = $("input[name='details[" + row + "].COGS]").val();
    $("input[name='details[" + row + "].UnitPrice']").val(Number(Price) * Number(Quantity));
    $("input[name='details[" + row + "].UnitCOGS]").val(Number((COGS * Quantity)));
}