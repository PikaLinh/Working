$(document).ready(function () {
    // ReloadData();
    ReservationLst();
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
        url: "/DailyChicCutPreOrder/_GetAllServicePartital?Orderstatusid=" + Orderstatusid ,
        success: function (data) {
            $("#contentAllService").html(data);
        }
    });
}
function ReloadData() {
    var Orderstatusid = $(".OrderStatusId").filter(".btn-danger").data("id");
    GetAllServicePartital(Orderstatusid);
}
$(document).on("click", ".PreOrderId", function () {
    var PreOrderId = $(this).data("id");
    //alert(OrderId);
    $.ajax({
        type: "POST",
        url: "/DailyChicCutPreOrder/_UpdateOrder?PreOrderId=" + PreOrderId,
        success: function (data) {
           
            $("#divPopupOrder #contentOrder").html(data);
            $("#divPopupOrder").modal("show");
            var OrderStatusIdNow = $("#OrderStatusIdNow").val();
            $(".action").css("display", "none");
            $("#Calcel-OrderId").css("display", "inline-block");
            switch (Number(OrderStatusIdNow)) {
                case 5:
                    {
                        $("#StatusNow").html("Đặt trước");
                        $("#serve-immediately").css("display", "inline-block");
                        $("#wait").css("display", "inline-block");
                    }
                    break;
                case 2:
                    {
                        $("#StatusNow").html("Đang phục vụ");
                    }  break;
                case 3: $("#StatusNow").html("Đã tính tiền"); break;
                case 4: $("#StatusNow").html("Huỷ"); break;
            }
        }
    });
});

$(document).on("click", "#btnAddNewOrder", function () {
    $.ajax({
        type: "POST",
        url: "/DailyChicCutPreOrder/_UpdateOrder",
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
$(document).on("click", ".ServiceCategoryId", function () {
    //alert("123");
    var ServiceCategoryId = $(this).data("id");
    var ServiceName = $(this).text();
    var MinPrice = $("input[name='Min_Price_Of_ServiceCategoryId_" + ServiceCategoryId + "']").val();
    var MaxPrice = $("input[name='Max_Price_Of_ServiceCategoryId_" + ServiceCategoryId + "']").val();
    var data = $("#frmList").serialize();
    // alert(ServiceName + " " + ServiceCategoryId + " " + Price);
    //Duyệt xem đã tồn tại dịch vụ đó chưa.
    var ServiceExist = false;
    $(".details-ServiceCategoryId").each(function () {
        var row = $(this).data("row");
        if ($(this).val() == ServiceCategoryId) {
            var Quantity = $("input[name='details[" + row + "].Qty']").val(); 
            $("input[name='details[" + row + "].Qty']").val(Number(Quantity) + 1);
            UnitPrice(row);
            TotalPrice();
            ServiceExist = true;
            return false; // nếu return true nó tương tự lệnh continue trong vòng lòng thông thường.
        }
    });
    var PreOrderId = $("#PreOrderId").val();
    if (!ServiceExist || Number(PreOrderId) > 0) {
        $.ajax({
            type: "POST",
            url: "/DailyChicCutPreOrder/_DailyChicCutOrderDetailInnerInfo?ServiceCategoryId=" + ServiceCategoryId + "&PreOrderId=" + PreOrderId,
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
    var PreOrderId = $("#PreOrderId").val();
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
        var data = $("#frmList").serialize() + "&" + $("#frmHeader").serialize();
        var Gender = $(".Gender").filter(".btn-danger").data("gender");
       // var HairTypeId = $(".HairTypeId").filter(".btn-danger").data("hairtypeid");

        if (PreOrderId > 0) { // Sửa đơn hàng (Thêm dịch vụ)
            //alert("sửa đơn hàng: thông tin khách hàng, giảm giá ?");
            var Note = $("#Note").val();
            var AppointmentTime = $("#AppointmentTime").val();
          //  var BillDiscount = $("#BillDiscount").val();
          //  var BillDiscountTypeId = $("#BillDiscountTypeId").val();

            $.ajax({
                type: "POST",
                url: "/DailyChicCutPreOrder/SaveEditOrder?Note=" + Note + "&PreOrderId=" + PreOrderId + "&AppointmentTime=" + AppointmentTime,
                success: function (data) {
                    if (data == "success") {
                        window.location.assign("/DailyChicCutPreOrder/Index");
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
                url: "/DailyChicCutPreOrder/SaveAddNewOrder?Gender=" + Gender,
                data: data,
                success: function (data) {
                    if (data == "success") {
                        window.location.assign("/DailyChicCutPreOrder/Index");
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
    var MinSumprice = 0;
    var MaxSumprice = 0;
    var TotalUnitCOGS = 0;
    $(".details-MinUnitPrice").each(function () {
        var dataRow = $(this).data("row");
        //console.log(dataRow);
        var MinPrice = $("input[name='details[" + dataRow + "].MinUnitPrice']").val();
        var MaxPrice = $("input[name='details[" + dataRow + "].MaxUnitPrice']").val();
        MinSumprice += Number(MinPrice);
        MaxSumprice += Number(MaxPrice);
    });
    // Tổng cộng
    $("#MinViewBagSumPrice").val(MinSumprice);
    $("#MaxViewBagSumPrice").val(MaxSumprice);

    //Tạm thời không có giảm giá, sau này muốn cho giảm giá thì chỉnh lại field này
    $("#MinViewBagTotal").val(MinSumprice);
    $("#MinViewBagTotal").val(MaxSumprice);

    $("#SumPrice").html(numberWithCommas(MinSumprice) + " - " + numberWithCommas(MaxSumprice));

    console.log("MinSumprice:" + MinSumprice);
    console.log("MaxSumprice:" + MaxSumprice);

    //// Tính tổng cộng thành tiền
    //var Totalprice = 0;
    //var Discount = 0;
    //var BillDiscount = $("input[name='BillDiscount']").val();
    //var BillDiscountValue = $("select[name='BillDiscountTypeId']").val();
    //if (BillDiscountValue == 1) {
    //    Discount = BillDiscount;
    //}
    //else {
    //    Discount = (BillDiscount / 100) * Sumprice;
    //}
    //Totalprice = Sumprice - Discount;
    //// INSERT giá trị quy ra tiền đưa vào database : TotalBillDiscount 
    //$("#ViewBagTotalBillDiscount").val(Discount);

    //$("#TotalPrice").html(Totalprice);
    //$("#ViewBagTotal").val(Totalprice);
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
   // $("#SumPrice").number(true);
    $("#TotalPrice").number(true);
    $(".detail-Qty").number(true);
}

$(document).on("click", "#closedivMessenger", function () {
    $("#divMessenger").fadeOut();
});

//Nút phục vụ ngay
$(document).on("click", "#serve-immediately", function () {
    var PreOrderId = $("#PreOrderId").val();
    alert("Phục vụ ngay");
    //$.ajax({
    //    type: "POST",
    //    url: "/DailyChicCutPreOrder/UpdateOrderServeImmediately?OrderId=" + OrderId ,
    //    success: function (data) {
    //        if (data == "success") {
    //            window.location.assign("/DailyChicCutPreOrder/Index");
    //        }
    //        else {
    //            $("#divMessenger").removeAttr("class");
    //            $("#divMessenger").addClass("alert alert-danger");
    //            $("#Messenger").html(data);
    //            $("#divMessenger").fadeToggle();
    //        }
    //    }
    //});
});

//Nút phục vụ ngay
$(document).on("click", "#wait", function () {
    var PreOrderId = $("#PreOrderId").val();
    alert("Chờ");
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
        url: "/DailyChicCutPreOrder/UpdateOrderPay",
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

$(document).on('click', '.btn-ok', function (e) {
    var $modalDiv = $(e.delegateTarget);
    var PreOrderId = $("#PreOrderId").val();
    console.log(PreOrderId);
    $.ajax({
        type: "POST",
        url: "/DailyChicCutPreOrder/UpdateOrderCalcelOrderId?PreOrderId=" + PreOrderId,
        success: function (data) {
            if (data == "success") {
                window.location.assign("/DailyChicCutPreOrder/Index");
            }
            else {
                alert(data);
            }
        }
    });
});

//$(document).on("click", ".btn-show-popup-order", function () {
//    $("#divPopupOrder").modal("show");
//});

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
    var ServiceCategoryId = $("input[name='details[" + row + "].ServiceCategoryId']").val();
    if (Number(OrderId) > 0) { // sửa: Cập nahatj 
        $.ajax({
            type: "POST",
            url: "/DailyChicCutPreOrder/_DailyChicCutOrderDetailInnerInfoUpdateQty?ServiceCategoryId=" + ServiceCategoryId + "&OrderId=" + OrderId + "&Qty=" + Qty,
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
    var MinPrice = $("input[name='details[" + row + "].MinPrice']").val();
    var MaxPrice = $("input[name='details[" + row + "].MaxPrice']").val();
    var Quantity = $("input[name='details[" + row + "].Qty']").val();
   // var COGS = $("input[name='details[" + row + "].COGS]").val();
    $("input[name='details[" + row + "].MinUnitPrice']").val(Number(MinPrice) * Number(Quantity));
    $("input[name='details[" + row + "].MaxUnitPrice']").val(Number(MaxPrice) * Number(Quantity));
    $("input[name='details[" + row + "].UnitPrice']").val(numberWithCommas(Number(MinPrice) * Number(Quantity)) + " - " + numberWithCommas(Number(MaxPrice) * Number(Quantity)));
    //$("input[name='details[" + row + "].UnitCOGS]").val(Number((COGS * Quantity)));
}

//#region Xem danh sách đơn hàng đặt trước
$(document).on("click", "#btn-search", function () {
    //alert("123");
    ReservationLst();
});

function ReservationLst() {
    var data = $("#frmSearch").serialize();
    $.ajax({
        type: "POST",
        url: "/DailyChicCutPreOrder/_ReservationLst",
        data: data,
        success: function (data) {
            $("#divKhachDatTruoc").html(data);
        }
    });
}
//#region Xem 
$(document).on("click", ".btn-view", function () {
    var PreOrderId = $(this).data("id");
    //alert(PreOrderId);
    //Post lên server
    $.ajax({
        type: "POST",
        url: "/DailyChicCutPreOrder/GetPreOrderModelFromPreOrderId?PreOrderId=" + PreOrderId,
        dataType: "json",
        success: function (result, stt, jqXHR) {
            if (result.Success == true) {
                //Nếu thành công bật khung in
                console.log(result.Data)
                //In
                //openPrint($("#tmplInvoicePrint").html(), {
                //    Order: result.Data
                //});
                var content = kendo.template($("#tmplInvoicePrint").html())({
                    Order: result.Data
                });
                $("#divPopup-view-PreOder #contentPreOrder").html(content);
                $("#divPopup-view-PreOder").modal("show");
            }
            else {
                alert(result.data);
            }
        }
    });
});
//#endregion

//#region xoá 
$(document).on("click", ".btn-cancel", function () {
    var PreOrderId = $(this).data("id");
    var FullName = $("#PreOrderId_" + PreOrderId).val();
    $("#confirm-delete .modal-body strong").html(FullName);
    $("#PreOrderId").val(PreOrderId);
});
//#endregion

//#region Chờ 
$(document).on("click", ".btn-wait", function () {
    var PreOrderId = $(this).data("id");
    var FullName = $(this).data("info");
    $("#PreOrderId-to-confirm").val(PreOrderId); 
    $("#OrderStatusId-to-confirm").val(1); // đang chờ
    $("#full-name").html(FullName);
    //Xoá chọn mặc định loại tóc
    $(".HairTypeId").removeClass("btn-danger");
    $("#divPopup-Confirm-PreOder").modal("show");

});
//#endregion

//#region Phục vụ ngay 
$(document).on("click", ".btn-serve-immediately", function () {
    var PreOrderId = $(this).data("id");
    var FullName = $(this).data("info");
    $("#PreOrderId-to-confirm").val(PreOrderId);
    $("#OrderStatusId-to-confirm").val(2); // đang phục vụ
    $("#full-name").html(FullName);
    //Xoá chọn mặc định loại tóc
    $(".HairTypeId").removeClass("btn-danger");
    $("#divPopup-Confirm-PreOder").modal("show");

});
//#endregion

//#region Xác nhận 
$(document).on("click", "#btnSave-comfirm-customer", function () {
    var HairTypeId = $(".HairTypeId").filter(".btn-danger").data("hairtypeid");
    if (HairTypeId == undefined || HairTypeId == "") {
        $("#Mes-error-confirm-customer").html("Bạn chưa chọn loại tóc!");
    }
    else
    {
        $("#Mes-error-confirm-customer").html("");
        $.ajax({
            type: "POST",
            url: "/DailyChicCutPreOrder/ConfirmToPreOrder",
            data:{
                PreOrderId: $("#PreOrderId-to-confirm").val(),
                HairTypeId: HairTypeId,
                OrderStatusId: $("#OrderStatusId-to-confirm").val()
            },
            success: function (data) {
                if (data != "success") {
                    $("#Mes-error-confirm-customer").html(data);
                }
                else {
                    $("#divPopup-Confirm-PreOder").modal("hide");
                    $("#btn-search").trigger( "click" );
                }
            }
        });
    }
});
//#endregion

//#region Loại tóc
$(document).on("click", ".HairTypeId", function () {
    $(".HairTypeId").removeClass("btn-danger");
    $(this).addClass("btn-danger");
    var HairTypeId = $(this).data("hairtypeid");
});
//#endregion

//#endregion