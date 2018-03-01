
//Mã Sản phẩm
$(document).ready(function () {
    Select2_CustomForList("/DynamicProduct/GetCustomerLevelId", "CustomerLevelId");
    formatNumberForGird();
});

//Định dạng hiển thị số 
function formatNumberForGird() {
    $(".pricelist-Price").number(true);
}

//$("select[name='CustomerLevelId']").on("change", function (e) {
//    var CustomerLevelId = e.target.value;
//    var CustomerLevelName = e.target.textContent;
//    var row = $(this).data("row");
//    $.ajax({
//        type: "POST",
//        url: "/DynamicProduct/GetValueCustomerLevel",
//        data: {
//            SelectedCustomerLevelId: CustomerLevelId,
//              },
//        success: function (data) {
//            $("input[name='pricelist[" + row + "].MinimumPurchase']").val(data.MinimumPurchase); // giá bán
//        }
//    });
//});
