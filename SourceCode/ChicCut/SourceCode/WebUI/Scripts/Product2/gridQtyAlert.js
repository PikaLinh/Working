

$("select[name='Warehouse']").on("change", function (e) {
    var row = $(this).data("row");
    $("input[name='QtyAlertList[" + row + "].WarehouseId']").val($(this).val());

    var WarehouseName = $(this).find("option:selected").text();
    console.log(WarehouseName);
    $("input[name='QtyAlertList[" + row + "].WarehouseName']").val(WarehouseName); //gán tên hiển thị
});

$("select[name='Roles']").on("change", function (e) {
    var row = $(this).data("row");
    $("input[name='QtyAlertList[" + row + "].RolesId']").val($(this).val());
});
