

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
    //GetProductStoreCode();
});

$(document).on("click", "#btnSave", function () {
    if (!$("#frmEdit").valid()) { // Not Valid
        return false;
    } else {
        if ($("Select[name='CategoryId']").val() == null || $("Select[name='CategoryId']").val() == "") {
            $("#divPopup #content").html("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
            $("#divPopup").modal("show");
        }
        else {
            for (instance in CKEDITOR.instances) {
                CKEDITOR.instances[instance].updateElement();
            }

            $("#frmEdit").ajaxSubmit({
                type: "POST",
                url: "/DynamicProduct/SaveEditProduct",
                success: function (data) {
                    if (data == "success") {
                        window.location.assign("/DynamicProduct/Index");
                    }
                    else {
                        $("#divPopup #content").html(data);
                        $("#divPopup").modal("show");
                    }
                }
            });
            return false;
        }
    }
});

//Định dạng hiển thị số 
$(".pricelist-Price").number(true);
