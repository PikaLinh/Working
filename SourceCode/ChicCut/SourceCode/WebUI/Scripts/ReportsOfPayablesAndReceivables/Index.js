
// Khách hàng
Select2_Custom("/Sell/GetCustomerId", "CustomerId");

// Nhà cung cấp
Select2_Custom("/ImportMaster/GetSuplierID", "SupplierId");

$(document).ready(function () {
    ////TAB RADIO MANAGEMENT BEGIN ***********************************************************************************
    $("ul.nav-tabs li:first").addClass("active").show(); //Activate first tab
    $(".tab-pane:first").addClass("active").show(); //Show first tab content

    //#region Lấy giá trị từ URL 
    var Fromdate = getUrlVars()["Fromdate"];
    var Todate = getUrlVars()["Todate"];
    var Type = getUrlVars()["Type"];
    var CustomerId = getUrlVars()["CustomerId"];
    var SupplierId = getUrlVars()["SupplierId"];
    var FullName = getUrlVars()["FullName"];
    //#endregion
   
    if (Type == "KH") {
        //Gán giá trị ngày tháng
        Select2_Custom("/Sell/GetCustomerId", "CustomerId", CustomerId, decodeURIComponent(FullName), "divCustomId");
        $("#FromDate").val(Fromdate);
        $("#ToDate").val(Todate);
        ChiTietCongNoKH();
    }
    else if (Type == "NCC")
    {
        //Gán giá trị ngày tháng
        Select2_Custom("/ImportMaster/GetSuplierID", "SupplierId", SupplierId, decodeURIComponent(FullName), "divSupplierId");
        $("#FromDateSup").val(Fromdate);
        $("#ToDateSup").val(Todate);
        ChiTietCongNoNCC();
        //Tab NCC hiển thị

        $("ul.nav-tabs li:first").removeClass("active"); // ẩn tab 1
        $(".tab-pane:first").removeClass("active"); 

        $("ul.nav-tabs li:last").addClass("active").show(); //hiện tab 2
        $(".tab-pane:last").addClass("active").show(); 
    }
});

$(document).on("click", "#btnSearchCustomer", function () {
    if ($("select[name='CustomerId']").val() == null) {
        $("#divPopup #content").html("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
        $("#divPopup").modal("show");
    }
    else {
        ChiTietCongNoKH();
    }
});
function ChiTietCongNoKH() {
    var data = $("#frmCustomerSearch").serialize();
    $.ajax({
        type: 'POST',
        url: '/ReportsOfPayablesAndReceivables/_CustomerInforPartial',
        data: data,
        success: function (data) {
            $("#CustomerInforContent").html(data);
        }
    })
    $.ajax({
        type: 'POST',
        url: '/ReportsOfPayablesAndReceivables/_CustomerInforTransaction',
        data: data,
        success: function (data) {
            $("#CustomerInforTransaction").html(data);
        }
    })
}
$(document).on("click", "#btnSearchSupplier", function () {
    if ($("select[name='SupplierId']").val() == null) {
        $("#divPopup #content").html("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
        $("#divPopup").modal("show");
    }
    else {
        ChiTietCongNoNCC();
    }
});

function ChiTietCongNoNCC() {
    var data = $("#frmSupplierSearch").serialize();
    $.ajax({
        type: 'POST',
        url: '/ReportsOfPayablesAndReceivables/_SupplierInforPartial',
        data: data,
        success: function (data) {
            $("#SupplierInforContent").html(data);
        }
    })
    $.ajax({
        type: 'POST',
        url: '/ReportsOfPayablesAndReceivables/_SupplierInforTransaction',
        data: data,
        success: function (data) {
            $("#SupplierInforTransaction").html(data);
        }
    })
}
function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}