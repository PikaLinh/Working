
// Load Sản phẩm từ Select2
Select2_Custom("/Product/GetProductId", "ProductId");

$("select[name='ProductId']").on("change", function (e) {
    // console.log(e.target.textContent);//name
    var Name = e.target.textContent;
    var Product = Name.substring(Name.indexOf(" | ") + 3);
    $("input[name='ProductName']").val(Product);
});

Select2_Custom("/InventoryMaster/GetInventoryTypeId", "InventoryTypeId");

$("select[name='InventoryTypeId']").on("change", function (e) {
    // console.log(e.target.textContent);//name
    var Name = e.target.textContent;
    var InventoryType = Name.substring(Name.indexOf(" | ") + 3);
    $("input[name='InventoryTypeName']").val(InventoryType);
});

Select2_Custom("/InventoryMaster/GetInventoryMasterId", "InventoryMasterId");

$("select[name='InventoryMasterId']").on("change", function (e) {
    // console.log(e.target.textContent);//name
    var Name = e.target.textContent;
    var InventoryCode = Name.substring(Name.indexOf(" | ") + 3);
    $("input[name='InventoryCode']").val(InventoryCode);
});