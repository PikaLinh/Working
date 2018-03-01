$(document).ready(function () {
    ObjectVisible($("#ObjectNameTmp").val());
    PeriodTypeVisible($("input[name='PeriodType']:checked").attr("id"));
    NDaysVisible();
    GetHrefEmailTemplate($("#EmailTemplateId").val());
    EmailTemplateVisible();
    SMSTemplateVisible();
    GetHrefSMSTemplate($("#SMSTemplateId").val());
    VisibleEmailOfEmployee();
    //Khách hàng
    Select2_Custom("/CashReceiptVoucher/GetCustomerID", "CustomerId", $("#IdCustomer").val(), $("#FullNameCustomer").val(), "divCustomerId");
    // Nhà cung cấp
    Select2_Custom("/ImportMaster/GetSuplierID", "SupplierId", $("#IdSupplier").val(), $("#FullNameSupplier").val(), "divSupplierId");
    GetNextDateReminder();
    VisibleEmailPara();
    VisibleSMSPara();
    $("#divEmailOfEmployee input").first().attr("id", "EmployeeTag"); //Insert id cho input tag tempt tự sinh : Dùng để add tag 
});



$(document).on("click", "#btnSave", function () {
        AjaxSubmit("Update");
});