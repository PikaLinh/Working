

$(document).ready(function () {
    ////TAB RADIO MANAGEMENT BEGIN ***********************************************************************************
    $("ul.nav-tabs li:first").addClass("active").show(); //Activate first tab
    $(".tab-pane:first").addClass("active").show(); //Show first tab content

    //#region Lấy giá trị từ URL 
    var Tab = getUrlVars()["Tab"];
    //#endregion
    if (Tab == "2") {
        $("ul.nav-tabs li:first").removeClass("active"); // ẩn tab 1
        $(".tab-pane:first").removeClass("active");

        $("ul.nav-tabs li:last").addClass("active").show(); //hiện tab 2
        $(".tab-pane:last").addClass("active").show();
    }
});

//#region //Bước 1 xử lý thêm dòng mới trong CreateList
$(document).on("click", "#btnAddNewRow", function () {
    var data = $("#frmList").serializeArray();
    $.ajax({
        type: "POST",
        url: "/MasterChicCutService/_AddNewListQuantification",
        data: data,
        success: function (data) {
            $("#divPopupQuantification #contentQuantification").html(data);
            $("#divPopupQuantification").modal("show");
        }
    });
});
//#endregion

//#region Xử lý thêm dòng mới trong _AddNewListInnerQuantification
$(document).on("click", "#btnAddNewRowAddNewInner", function () {
    var data = $("#frmListAddNewInner").serializeArray();
    $.ajax({
        type: "POST",
        url: "/MasterChicCutService/_AddNewListInnerQuantification",
        data: data,
        success: function (data) {
            $("#tblQuantificationAddNewInner tbody").html(data);
        }
    });
});
//#endregion

//#region Xử lý detail-btndelete
$(document).on("click", ".detailInner-btndelete", function () {
    var removeId = $(this).data("row");
    var data = $("#frmListAddNewInner").serializeArray();
        $.ajax({
            type: "POST",
            url: "/MasterChicCutService/_DeleteAddNewlistInner?RemoveId=" + removeId,
            data: data,
            success: function (data) {
                $("#tblQuantificationAddNewInner tbody").html(data);
            }
        });
});
//#endregion

//#region Xử lý nút Lưu
$(document).on("click", "#btnSaveAddNewQuantification", function () {
    /* B1: Kiểm tra chi tiết định lượng
           + Hợp lệ: Thêm 1 record mới vào danh sách CreateList (các filed sản phẩm + số lượng để hidden)
           + Không hợp lệ: Hiển thị thông báo.
    */
    //$("select[name='ProductId']").each(function () {

    //});
    // alert("123");
    var QuantificationMasterId = $("#QuantificationMasterId").val();
    if (QuantificationMasterId != 0) { // Sửa
        //alert("Sửa");
        var ServiceId = $("#ServiceId").val();
        var QuantificationMasterId = QuantificationMasterId;
        var data = $("#frmListAddNewInner").serialize();
        $.ajax({
            type: "POST",
            url: "/MasterChicCutService/UpdateAddNewQuantification?QuantificationName=" + $("#QuantificationName").val() + "&ServiceId=" + ServiceId + "&QuantificationMasterId=" + QuantificationMasterId,
            data: data,
            success: function (data) {
                if (data == "success") {
                    window.location.assign("/MasterChicCutService/Edit/" + ServiceId + "?Tab=2");
                }
                else {
                    $("#divPopup #content").html(data);
                    $("#divPopup").modal("show");
                }
            }
        });
    }
    else { // Thêm mới
        var ServiceId = $("#ServiceId").val();
        var data = $("#frmListAddNewInner").serialize();
        $.ajax({
            type: "POST",
            url: "/MasterChicCutService/SaveAddNewQuantification?QuantificationName=" + $("#QuantificationName").val() + "&ServiceId=" + ServiceId,
            data: data,
            success: function (data) {
                if (data == "success") {
                    window.location.assign("/MasterChicCutService/Edit/" + ServiceId + "?Tab=2");
                }
                else {
                    $("#divPopup #content").html(data);
                    $("#divPopup").modal("show");
                }
            }
        });
    }
  
});
//#endregion

//#region Xử lý nút Xoá
$(document).on("click", ".QMaster-btndelete", function () {
    //alert("123");
    var ServiceId = $("#ServiceId").val();
    $.ajax({
        type: "POST",
        url: "/MasterChicCutService/_QuantificationDeleteInner?QuantificationMasterId=" + $(this).data("id"),
        success: function (data) {
            if (data == "success") {
                window.location.assign("/MasterChicCutService/Edit/" + ServiceId + "?Tab=2");
            }
            else {
                $("#divPopup #content").html(data);
                $("#divPopup").modal("show");
            }
        }
    });
});
//#endregion


//#region Xử lý nút Sửa
$(document).on("click", ".QMaster-btnupdate", function () {
    //alert("123");
    var ServiceId = $("#ServiceId").val();
    $.ajax({
        type: "POST",
        url: "/MasterChicCutService/_QuantificationUpdateInner?QuantificationMasterId=" + $(this).data("id"),
        success: function (data) {
            $("#divPopupQuantification #contentQuantification").html(data);
            $("#divPopupQuantification").modal("show");
        }
    });
});
//#endregion

//#region 

//#endregion

//Định dạng hiển thị số tiền
function formatNumberForGird() {
    // Định dạng hiển thị tiền : trọng lượng, giá, thành tiền
    $(".detailInner-Qty").number(true, 2);
}

// Bước 3 : Xử lý button Save
function SubmitForm(action) {
    loading2();
    var CheckQtyEmpty = false;
    $(".detail-Qty").each(function () {
        if ($(this).val() == "") {
            CheckQtyEmpty = true;
        }
    });
    if ($("#ServiceName").val() == "" || $("#Price").val() == "" || $("#QuantificationName").val() == "" || $("select[name='HairTypeId']").val() == "" || $("select[name='HairTypeId']").val() == null || CheckQtyEmpty == true) {
        $("#divPopup #content").html("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
        $("#divPopup").modal("show");
        $("body").removeClass("loading2");
    }
    else {
        var data = $("#frmList").serialize() + "&" + $("#frmHeader").serialize();
        $.ajax({
            type: "POST",
            url: "/MasterChicCutService/" + action,
            data: data,
            success: function (data) {
                if (data == "success") {
                    window.location.assign("/MasterChicCutService/Index");
                }
                else {
                    $("#divPopup #content").html(data);
                    $("#divPopup").modal("show");
                }
            }
        });
    }
}

$("#Price").number(true);
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