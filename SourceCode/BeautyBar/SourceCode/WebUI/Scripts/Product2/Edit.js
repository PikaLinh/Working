if ($('#isHot').prop("checked") == true) {
    $('#divOrderBy').show();
}

Select2_Custom("/Product/GetProductType", "ProductTypeId", $("#ProductTypeIdMark").val(), $("#ProductTypeNameMark").val(), "divProductType");
$("select[name='StoreId']").on("change", function (e) {
    if ($("#StoreIdMark").val() != $("select[name='StoreId']").val()) {
        GetProductStoreCode();
    }
    else {// Nếu = với StoreId cũ thì xét ProductTypeId có = cũ không
        if ($("#ProductTypeIdMark").val() != $("select[name='ProductTypeId']").val()) {
            GetProductStoreCode();
        }
        else {
            $("#ProductStoreCode").val($("#ProductStoreCodeMark").val());
            $("#divOrderCode span").html($("#ProductStoreCodeMark").val());
            //alert("Return");
        }

    }
});
$("select[name='ProductTypeId']").on("change", function (e) {
    console.log(e.target.value);
    console.log($("#ProductTypeIdMark").val());
    if ($("#ProductTypeIdMark").val() != e.target.value) {
        GetProductStoreCode();
    }
    else {
        if ($("#StoreIdMark").val() != $("select[name='StoreId']").val()) {
            GetProductStoreCode();
        }
        else {
            $("#ProductStoreCode").val($("#ProductStoreCodeMark").val());
            $("#divOrderCode span").html($("#ProductStoreCodeMark").val());
            //alert("Return");
        }
    }
});
$("select[name='CategoryId']").on("change", function (e) {
    GetProductStoreCode();
});

$(document).on("click", "#btnSave", function () {
    if (!$("#frmEdit").valid()) { // Not Valid
        return false;
    } else {
        if ($("select[name='ProductTypeId']").val() == "" || $("#ProductName").val() == "") {
            $("#divPopup #content").html("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
            $("#divPopup").modal("show");
        }
        else {
            var form = $('#frmEdit');

            var row;
            var WarehouseId;
            var QtyAlert;
            var RolesId;

            $(".QtyAlertList-WarehouseName").each(function () {
                row = $(this).data("row");
                WarehouseId = $("input[name='QtyAlertList[" + row + "].WarehouseId']").val();
                QtyAlert = $("input[name='QtyAlertList[" + row + "].QtyAlert']").val();
                RolesId = $("input[name='QtyAlertList[" + row + "].RolesId']").val();

                var inputWarehouseId = $("<input>")
                .attr("type", "hidden")
                .attr("name", "QtyAlertList[" + row + "].WarehouseId").val(WarehouseId);
                form.append($(inputWarehouseId));

                var inputQtyAlert = $("<input>")
                .attr("type", "hidden")
                .attr("name", "QtyAlertList[" + row + "].QtyAlert").val(QtyAlert);
                form.append($(inputQtyAlert));

                var inputRolesId = $("<input>")
                .attr("type", "hidden")
                .attr("name", "QtyAlertList[" + row + "].RolesId").val(RolesId);
                form.append($(inputRolesId));
            });

            form.ajaxSubmit({
                type: "POST",
                success: function (data) {
                    //can sua lai cho dung
                    if (data == "success") {
                        window.location.assign("/product/Index");
                    }
                    else {
                        $("#divPopup #content").html(data);
                        $("#divPopup").modal("show");
                    };
                }
            });

            return false;
        }
    }
});