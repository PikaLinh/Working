
select2ForImportMaster();

function select2ForImportMaster() {
    $eventSelect = $("select[name='ImportMasterId']");
    $eventSelect.select2({
        allowClear: true,
        ajax: {
            url: '/ReturnMaster/GetImportMasterID',
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    q: params.term, // search term
                    page: params.page,
                    StoreId: $("#StoreId").val(),
                    WarehouseId: $("#WarehouseId").val(),
                    SupplierId: $("#SupplierId").val()
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
            url: "/ReturnMaster/GetListDetailByImportMasterID?ImportMasterID=" + $(this).val(),
            success: function (data) {
                $("#contentCreateListInner").html(data);
                formatNumberForGird();
                TotalWeight(); // Tính TotalShippingWeight,GuestAmountPaid(paid)
                //RemainingAmount();
            }
        });
        // Bước 2 : set value for StoreId, WarehouseId, SupplierId
        $.ajax({
            type: "POST",
            url: "/ReturnMaster/Get3FieldByImportMasterID?ImportMasterID=" + $(this).val(),
            success: function (data) {
                $("#StoreId").val(data.StoreId);
                $("#WarehouseId").val(data.WarehouseId);
                $("#SupplierId").val(data.SupplierId);
                // Tiền tệ
                $("#CurrencyId").val(data.CurencyId);
                $("#divCurrencyId label").html(data.CurencyName);

                // Tỷ giá
                $("#divExchangeRate2 label").html(data.ExchangeRate);
                $("#ExchangeRate").val(data.ExchangeRate)
                if (data.ExchangeRate != 1) {
                    $("#divExchangeRate").show();
                }
                else {
                    $("#divExchangeRate").hide();
                }

                // Nhân viên kinh doanh
                $("#SalemanName").val(data.SalemanName);
                $("#divSalemanName label").html(data.SalemanName);

                // SET Giảm giá và VAT
                $("#ManualDiscount").val(data.BillDiscount);
                $("#ManualDiscountType option").removeAttr("selected");
                $("#ManualDiscountType option[value='" + data.BillDiscountTypeId + "']").attr("selected", "selected");
                $("#VATValue").val(data.BillVAT);
            }
        });
    });
}
// Bắt sự kiện change StoreId
$(document).on("change", "#StoreId", function () {
    // Change ImportMasterId
    $("#ImportMasterId").empty().trigger('change')
    // Get value for WarehouseId
    $.ajax({
        type: "POST",
        url: "/ReturnMaster/GetWarehouseIdByStoreId?StoreId=" + $(this).val(),
        success: function (response) {
            //var LstItem = "";
            //$.each(response, function (i, item) {
            //    LstItem += "<option value=" + item.value + ">" + item.text + " </option>" ;
            //});
            //alert(LstItem);
            //console.log(LstItem);
            //$("#WarehouseId").html(LstItem);
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
    $("#ImportMasterId").empty().trigger('change')
    select2ForImportMaster();
});

// Bắt sự kiện change WarehouseId
$(document).on("change", "#SupplierId", function () {
    // Change ImportMasterId
    $("#ImportMasterId").empty().trigger('change')
    select2ForImportMaster();
});

//TotalWeight();

function invalid() {
    if ($("select[name='ImportMasterId']").val() == null) {
        return true;
    } else {
        return false;
    }
}
// Xử lý btnSave
$(document).on("click", "#btnSave", function () {
    if (invalid()) {
        $("#divPopup #content").html("Vui lòng nhập đầy đủ thông tin được đánh dấu sao \(<span class=\"color-red\">*</span>)\!");
        $("#divPopup").modal("show");
    }
    else {
        // Duyệt để tối thiểu phải trả 1 sản phẩm
        var TotalQty = 0;
        $(".detail-ReturnQty").each(function () {
            var dataRow = $(this).data("row");
            var Qty = $("input[name='detail[" + dataRow + "].ReturnQty']").val();
            TotalQty += Number(Qty);
        });
        if (TotalQty == 0) {
            $("#divPopup #content").html("Vui lòng nhập tối thiểu 1 số lượng trả !");
            $("#divPopup").modal("show");
        }
        else {
            //var $btn = $(this).button('loading');
            //TotalWeight();
            var data = $("#frmList").serialize() + "&" + $("#frmHeader").serialize();
            $.ajax({
                type: "POST",
                url: "/ReturnMaster/Save",
                data: data,
                success: function (data) {
                    // $btn.button('reset');
                    //$("#tblImportDetail tbody").html(data);
                    //$btn.button('reset');
                    if (data == "success") {
                        window.location.assign("/ReturnMaster/Index");
                    }
                    else {
                        //alert(data);
                        Alert_Popup(data);
                    }
                }
            });
        }
    }
});
$(document).ready(function () {
   
});