
select2ForImportMaster();

function select2ForImportMaster() {
    $eventSelect = $("select[name='OrderId']");
    $eventSelect.select2({
        allowClear: true,
        ajax: {
            url: '/OrderReturnMaster/GetOrderId',
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    q: params.term, // search term
                    page: params.page,
                    StoreId: $("#StoreId").val(),
                    WarehouseId: $("#WarehouseId").val()
                };
            },
            processResults: function (data) {
                return {
                    results: $.map(data, function (obj) {
                        return { id: obj.value, text: obj.text };
                    })
                };
                
            },
            minimumInputLength: 0 // Tối thiếu 1 kí tự thì mới search
        }
    });

    $eventSelect.on("change", function (e) {
        // Bước 1:  set list value for _CreateListInner
        $.ajax({
            type: "POST",
            url: "/OrderReturnMaster/GetListDetailByOrderID?OrderID=" + $(this).val(),
            success: function (data) {
                $("#contentCreateListInner").html(data);
                formatNumberForGird();
                TotalPrice(); // Tính TotalShippingWeight,GuestAmountPaid(paid)
                RemainingAmount();
            }
        });
        // Bước 2 : set value for StoreId, WarehouseId, SupplierId
        $.ajax({
            type: "POST",
            url: "/OrderReturnMaster/Get3FieldByOrderID?OrderID=" + $(this).val(),
            success: function (data) {
                $("#StoreId").val(data.StoreId);
                $("#WarehouseId").val(data.WarehouseId);
                $("#CustomerId").html(data.CustomerId);
                $("#CustomerLevelId ").html(data.CustomerLevelId);
                $("#IdentityCard ").html(data.IdentityCard);
                $("#Phone ").html(data.Phone);
                $("#Gender ").html(data.Gender);
                $("#Email ").html(data.Email);
                $("#ProvinceId ").html(data.ProvinceId);
                $("#DistrictId ").html(data.ProvinceId);
                $("#Address ").html(data.Address);
                $("#SaleId ").html(data.SaleId);
                $("#CompanyName ").html(data.CompanyName);
                $("#TaxBillCode ").html(data.TaxBillCode);
                $("#ContractNumber ").html(data.ContractNumber);
                $("#TaxBillDate ").html(data.TaxBillDate);
                $("#ContractNumber ").html(data.ContractNumber);
               // $("#Note ").html(data.Note);
                $("#DebtDueDate ").html(data.DebtDueDate);
                // SET Giảm giá và VAT
                $("#BillDiscount").val(data.BillDiscount);
                $("#BillDiscountTypeId option").removeAttr("selected");
                $("#BillDiscountTypeId option[value='" + data.BillDiscountTypeId + "']").attr("selected", "selected");
                $("#BillVAT").val(data.BillVAT);
            }
        });
    });
}
// Bắt sự kiện change StoreId
$(document).on("change", "#StoreId", function () {
    // Change ImportMasterId
    $("#OrderId").empty().trigger('change')
    // Get value for WarehouseId
    $.ajax({
        type: "POST",
        url: "/ReturnMaster/GetWarehouseIdByStoreId?StoreId=" + $(this).val(),
        success: function (response) {
            var listItems = "";

            $.each(response, function (i, item) {
                listItems += "<option value='" + item.Id + "'>" + item.Name + "</option>";
            });

            $("#WarehouseId").html(listItems);
           
        }
    });
});

// Bắt sự kiện change WarehouseId
$(document).on("change", "#WarehouseId", function () {
    // Change ImportMasterId
    $("#OrderId").empty().trigger('change')
    select2ForImportMaster();
});
function invalid() {
    if ($("select[name='OrderId']").val() == null) {
        return true;
    } else {
        return false;
    }
}
// Xử lý btnSave
$(document).on("click", "#btnSave", function () {
    loading2();
    if ($("select[name='PaymentMethodId']").val() == 0) {
        $("#divPopup #content").html("Vui lòng chọn 'phương thức thanh toán'");
        $("#divPopup").modal("show");
        $("body").removeClass("loading2");
    }
    else {
        if (invalid()) {
            $("#divPopup #content").html("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
            $("#divPopup").modal("show");
            $("body").removeClass("loading2");
        }
        else {
            // Duyệt để tối thiểu phải trả 1 sản phẩm
            var TotalQty = 0;
            $(".detail-ReturnQuantity ").each(function () {
                var dataRow = $(this).data("row");
                var Qty = $("input[name='detail[" + dataRow + "].ReturnQuantity']").val();
                TotalQty += Number(Qty);
            });
            if (TotalQty == 0) {
                $("#divPopup #content").html("Vui lòng nhập tối thiểu 1 số lượng trả !");
                $("#divPopup").modal("show");
                $("body").removeClass("loading2");
            }
            else {
                //var $btn = $(this).button('loading');
                //TotalWeight();
                var data = $("#frmList").serialize() + "&" + $("#frmHeader").serialize();
                $.ajax({
                    type: "POST",
                    url: "/OrderReturnMaster/Save",
                    data: data,
                    success: function (data) {
                        // $btn.button('reset');
                        //$("#tblImportDetail tbody").html(data);
                        //$btn.button('reset');
                        if (data == "success") {
                            window.location.assign("/OrderReturnMaster/Index");
                        }
                        else {
                            //alert(data);
                            Alert_Popup(data);
                        }
                    }
                });
            }
        }
    }
});
$(document).ready(function () {
   
});