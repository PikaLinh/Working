//Mã Sản phẩm
$(document).ready(function () {
    Select2_CustomForListDetailInner("/ImportMaster/GetProductId", "ProductId");
    formatNumberForGird();
});

$("select[name='ProductId']").on("change", function (e) {
    var Productid = e.target.value;
    //var ProductName = e.target.textContent;
    var ProductName = e.target.childNodes[e.target.childNodes.length - 1].textContent;
    var row = $(this).data("row");
    // Lưu ProductID lại
    $("input[name='detailInner[" + row + "].ProductId']").val(Productid);
    // Lưu ProducName lại
    $("input[name='detailInner[" + row + "].ProductName']").val(ProductName);
});