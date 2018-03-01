
GetProductStoreCode();
//Select2_Custom("/Product/GetProductType", "ProductTypeId");
$("select[name='StoreId']").on("change", function (e) {
    GetProductStoreCode();
});
//$("select[name='ProductTypeId']").on("change", function (e) {
//    GetProductStoreCode();
//});

$("select[name='CategoryId']").on("change", function (e) {
    //GetProductStoreCode();
});

$(document).on("click", "#btnSave", function () {
    if (!$("#frmAdd").valid()) { // Not Valid
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

            $("#frmAdd").ajaxSubmit({
                    type: "POST",
                    url: "/DynamicProduct/SaveCreateProduct",
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