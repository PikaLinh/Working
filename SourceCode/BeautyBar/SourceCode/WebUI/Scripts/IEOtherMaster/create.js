
$(document).ready(function () {
    // Khách hàng
    var CustomerId = $("#IdCustomer").val();
    var CustomerName = $("#FullName").val();
    Select2_Custom("/Sell/GetCustomerId", "CustomerName", CustomerId, CustomerName, "divCustomerId");
    formatNumberForGird();
});
// Bước 3 : Xử lý btnSave
$(document).on("click", "#btnSave", function () {
    //var $btn = $(this).button('loading');
    var data = $("#frmList").serialize() + "&" + $("#frmHeader").serialize();
    //var data = $("#frmHeader").serialize();
    $.ajax({
        type: "POST",
        url: "/IEOtherMaster/Save",
        data: data,
        success: function (data) {
            // $btn.button('reset');
            //$("#tblImportDetail tbody").html(data);
            //$btn.button('reset');
            if (data == "success") {
                window.location.assign("/IEOtherMaster/Index");
            }
            else {
                //alert(data);
                Alert_Popup(data);
            }
        }
    });

});

$(document).ready(function () {
    //ProQty
    $("#ProQty").keyup(function () {
        if ($(this).val() == 0) {
            $(this).val(1);
        }
    });
    
    $("#scanBarcode").keypress(function (e) {
        if (e.which == 13) {
            //Loại bỏ sự kiện mặc định của enter
            e.preventDefault();

            var data = $("#frmList").serializeArray();
            
            $.ajax({
                type: "POST",
                url: "/IEOtherMaster/_CreatelistInner",
                data: data,
                success: function (result) {
                    if (result != "Mã vạch này chưa tồn tại!") {
                        $("#btnAddNewRow").trigger("click");
                    }
                    else {
                        Alert_Popup(result);
                    }
                    //Để textbox trở lại trạng thái rỗng
                    $("#scanBarcode").val("");
                    $("#scanBarcode").focus();
                    $("#ProQty").val(1);
                },
            });
        }
    });
});
