Select2_Custom("/ImportMaster/GetImportMasterId", "ImportMasterId");

$("select[name='ImportMasterId']").on("change", function (e) {
    // console.log(e.target.textContent);//name
    var Name = e.target.textContent;
    var ImportMaster = Name.substring(Name.indexOf(" | ") + 3);
    $("input[name='ImportMasterCode']").val(ImportMaster);
});


// Load Sản phẩm từ Select2
Select2_Custom("/Product/GetProductId", "ProductId");

$("select[name='ProductId']").on("change", function (e) {
    // console.log(e.target.textContent);//name
    var Name = e.target.textContent;
    var Product = Name.substring(Name.indexOf(" | ") + 3);
    $("input[name='ProductName']").val(Product);
});