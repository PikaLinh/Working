// Khách hàng
Select2_Custom("/CashReceiptVoucher/GetCustomerID", "CustomerId");

// Nhà cung cấp
Select2_Custom("/ImportMaster/GetSuplierID", "SupplierId");

// Nhân viên
Select2_Custom("/CashReceiptVoucher/GetEmployeeId?StoreId=" + $("#StoreId").val(), "EmployeeId");

$(document).ready(function () {
    LoaiDoiTuong();
    $("#Amount").number(true);
});

// Thay đổi Đối tượng
$("select[name='ContactItemTypeCode']").on("change", function (e) {
    LoaiDoiTuong();
    $("#ViewBagDebtOld").val(0);
    CalDebt(0);
});

// Thay đổi Của hàng => load select2 Nhân Viên
$("select[name='StoreId']").on("change", function (e) {
    //alert("123");
    //$(this).empty();
    $("select[name='EmployeeId']").empty().trigger('change');
    Select2_Custom("/CashReceiptVoucher/GetEmployeeId?StoreId=" + $("#StoreId").val(), "EmployeeId");
});

// Thay đổi Nhà cung cấp => Xét lại 'Số dư nợ cuối kỳ'
$("select[name='SupplierId']").on("change", function (e) {
    // alert("123");
    $.ajax({
        type: "POST",
        url: "/CashReceiptVoucher/GetDebOldSupplier",
        data: { SupplierId: $(this).val() },
        success: function (data) {
            CalDebt(data);
        }
    })
});

// Thay đổi Khách hàng => Xét lại 'Số dư nợ cuối kỳ'
$("select[name='CustomerId']").on("change", function (e) {
    // alert("123");
    $.ajax({
        type: "POST",
        url: "/CashReceiptVoucher/GetDebOld",
        data: { CustomerId: $(this).val() },
        success: function (data) {
            CalDebt(data);
        }
    })
});

$(document).on("keyup", "#Amount", function () {
    CalDebt($("#ViewBagDebtOld").val());
});

$("select[name='TransactionTypeCode']").on("change", function (e) {
    CalDebt($("#ViewBagDebtOld").val());
});

function CalDebt(NumberDebtOld) {
    $("#DebtOld").html(NumberDebtOld);
    $("#DebtOld").number(true);
    $("#ViewBagDebtOld").val(NumberDebtOld);
    var dau = $("select[name='TransactionTypeCode']").val() == 'NHRUT' ? 1 : -1; // nếu là Báo nợ (rút tiền) => Làm nợ khách hàng tăng
    var Amount = $("#Amount").val() == '' ? 0 : $("#Amount").val();
    $("#DebtNew").html(Number(NumberDebtOld) + (Amount * dau));
    $("#DebtNew").number(true);
}

function LoaiDoiTuong() {
    $(".visible").each(function () {
        $(this).hide();
    });
    $("select[name='CustomerId']").empty().trigger('change');
    $("select[name='SupplierId']").empty().trigger('change');
    $("select[name='EmployeeId']").empty().trigger('change');
    var DoiTuong = $("#ContactItemTypeCode").val();
    switch (DoiTuong) {
        case "KH": $("#divCustomerId2").show(); break;
        case "NCC": $("#divSupplierId2").show(); break;
        case "NV": $("#divEmployeeId2").show(); break;
        default: break;
    }
}

$(document).on("click", "#btnSave", function () {
    if ($("select[name='CustomerId']").val() == null && $("select[name='SupplierId']").val() == null && $("select[name='EmployeeId']").val() == null && $("#ContactItemTypeCode").val() != "KHAC" || $("#Amount").val() == "") {
        $("#divPopup #content").html("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
        $("#divPopup").modal("show");
    }
    else {
        var data = $("#frmHeader").serialize();
        $.ajax({
            type: "POST",
            url: "/BankPaymentVoucher/Save",
            data: data,
            success: function (data) {
                if (data == "success") {
                    window.location.assign("/BankPaymentVoucher/Index");
                }
                else {
                    $("#divPopup #content").html(data);
                    $("#divPopup").modal("show");
                }
            }
        });
    }
});
