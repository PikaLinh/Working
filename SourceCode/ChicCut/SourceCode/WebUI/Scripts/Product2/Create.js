
GetProductStoreCode();
Select2_Custom("/Product/GetProductType", "ProductTypeId");
$("select[name='StoreId']").on("change", function (e) {
    GetProductStoreCode();
});
$("select[name='ProductTypeId']").on("change", function (e) {
    GetProductStoreCode();
});


$("select[name='CategoryId']").on("change", function (e) {
    GetProductStoreCode();
});

$(document).on("click", "#btnSave", function () {
    if (!$("#frmAdd").valid()) { // Not Valid
        return false;
    } else {
        if ($("select[name='ProductTypeId']").val() == null || $("#ProductName").val() == null) {
            $("#divPopup #content").html("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
            $("#divPopup").modal("show");
        }
        else {
            //var data = $("#frmAdd").serialize() + "&" + $("#frmListQtyAlert").serialize();
            //$.ajax({
            //    type: "POST",
            //    url: "/Product/CreateProduct",
            //    data: data,
            //    success: function (data) {
            //        if (data == "success") {
            //            window.location.assign("/product/Index");
            //        }
            //        else {
            //            $("#divPopup #content").html(data);
            //            $("#divPopup").modal("show");
            //        };
            //    }
            //});

            var form = $('#frmAdd');

            var row;
            var WarehouseId;
            var QtyAlert;
            var RolesId;

            $(".QtyAlertList-WarehouseName").each(function () {
                row = $(this).data("row");
                WarehouseId = $("input[name='QtyAlertList["+ row +"].WarehouseId']").val();
                QtyAlert = $("input[name='QtyAlertList["+ row +"].QtyAlert']").val();
                RolesId = $("input[name='QtyAlertList["+ row +"].RolesId']").val();

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
