// Nhân viên hiện tại
Select2_Custom("/TransferManagementEmployee/GetEmployeeId", "EmployeeCurrentId", $("#EmployeeCurrentId").val(), $("#EmployeeCurrentName").val(), "divEmployeeCurrentId");
LoadListCusByEmp($("#EmployeeCurrentId").val());
// Nhân viên mới
Select2_Custom("/TransferManagementEmployee/GetEmployeeId", "EmployeeNewId");

$("select[name='EmployeeCurrentId']").on("change", function () {
    if ($(this).val() == $("select[name='EmployeeNewId']").val()) {
        $("#divPopup #content").html("<span class='color-red'>2 nhân viên</span> không thể trùng nhau !");
        $("#divPopup").modal("show");
        $(this).empty().trigger('change');
    }
    LoadListCusByEmp($(this).val());
});

$("select[name='EmployeeNewId']").on("change", function () {
    //alert("123");
    if ($(this).val() == $("select[name='EmployeeCurrentId']").val()) {
        $("#divPopup #content").html("<span class='color-red'>2 nhân viên</span> không thể trùng nhau !");
        $("#divPopup").modal("show");
        $(this).empty().trigger('change');
    }
});

function LoadListCusByEmp(EmployeeId) {
    loading2();
    $.ajax({
        type: "POST",
        url: "/TransferManagementEmployee/_CustomerList?EmployeeId=" + EmployeeId,
        success: function (data) {
            $("#divCustomerId").html(data);
        }
    });
}

$(document).on("click", "#chkAll", function () {
    //alert("123");
    if ($(this).prop("checked")) {
        $("input[name='CustomerId']").prop("checked", true);
    }
    else {
        $("input[name='CustomerId']").prop("checked", false);
    }
});

//#region Xử lý Save
$(document).on("click", "#btnSave", function () {
    if (IsValid() === "OK") {
        var ListCus = [];
        $("input[name='CustomerId'] ").each(function () {
            if ($(this).prop("checked")) {
                //console.log($(this).val());
                ListCus.push($(this).val());
            }
        });
        // console.log(ListCus);
        $.ajax({
            type: "POST",
            url: "/TransferManagementEmployee/Save",
            traditional: true,
            data: {
                ListCustomerId: ListCus
                , EmployeeCurrentId: $("select[name='EmployeeCurrentId']").val()
                , EmployeeNewId: $("select[name='EmployeeNewId']").val()
            },
            success: function (data) {
                if (data == "success") {
                    window.location.assign("/TransferManagementEmployee/Index?EmployeeCurrentId=" + $("select[name='EmployeeCurrentId']").val());
                }
                else {
                    $("#divPopup #content").html(data);
                    $("#divPopup").modal("show");
                }
            }
        });
    }
    else {
        $("#divPopup #content").html(IsValid());
        $("#divPopup").modal("show");
    }
   

    
});

function IsValid() {
    var Messg = "OK";
    var EmployeeCurrentId = $("select[name='EmployeeCurrentId']").val();
    var EmployeeNewId = $("select[name='EmployeeNewId']").val();
    if (EmployeeCurrentId == null || EmployeeCurrentId == "") {
        Messg = "Bạn hãy chọn <span class='color-red'>nhân viên hiện tại </span> !";
        return Messg;
    }
    if (EmployeeNewId == null || EmployeeNewId == "") {
        Messg = "Bạn hãy chọn <span class='color-red'>nhân viên mới </span> !";
        return Messg;
    }
    if (EmployeeNewId == EmployeeCurrentId) {
        Messg = "<span class='color-red'>2 nhân viên</span> không thể trùng nhau !";
        return Messg;
    }
    var ListCus = [];
    $("input[name='CustomerId'] ").each(function () {
        if ($(this).prop("checked")) {
            //console.log($(this).val());
            ListCus.push($(this).val());
        }
    });
    if (ListCus.length === 0) {
        Messg = "Không có <span class='color-red'>khách hàng</span> nào được áp dụng !";
        return Messg;
    }
    return Messg;
}
//#endregion