$(document).ready(function () {
    ObjectVisible("KH");
    PeriodTypeVisible($("input[name='PeriodType']:checked").attr("id"));
    NDaysVisible();
    GetHrefEmailTemplate($("#EmailTemplateId").val());
    EmailTemplateVisible();
    SMSTemplateVisible();
    GetHrefSMSTemplate($("#SMSTemplateId").val());
    VisibleEmailOfEmployee();
    VisibleSMSOfEmployee();
    // Gán cho CCEmail mặc định là checked
    //("#isCCEmail").prop("checked", true);
    VisibleEmailPara();
    VisibleSMSPara();
    $("#divEmailOfEmployee input").first().attr("id", "EmployeeTag"); //Insert id cho input tag tempt tự sinh : Dùng để add tag 
});



$(document).on("click", "#btnSave", function () {
    AjaxSubmit("Save");
});