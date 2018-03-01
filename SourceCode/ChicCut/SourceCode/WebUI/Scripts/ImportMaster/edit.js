formatNumberForGird();
// Nhà cung cấp
Select2_Custom("/ImportMaster/GetSuplierID", "SupplierId");

//tong tien
TotalWeight();

// Load Sản phẩm từ Select2
Select2_CustomForList("/ImportMaster/GetProductId", "ProductId");

// Bước  : Xử lý btnUpdate
$(document).on("click", "#btnUpdate", function () {
    //var $btn = $(this).button('loading');
    var data = $("#frmList").serialize() + "&" + $("#frmHeader").serialize();
    $.ajax({
        type: "POST",
        url: "/ImportMaster/Update",
        data: data,
        success: function (data) {
           // $btn.button('reset');
            //$("#tblImportDetail tbody").html(data);
            if (data == "success") {
                window.location.assign("/ImportMaster/Index");
            }
            else {
                Alert_Popup(data);
            }
        }
    });

});
