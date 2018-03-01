$(function () {
    var loading = $("#divGlobalAjaxLoading");
    $(document).ajaxSend(function (event, jqxhr, settings) {
        console.log(settings.type.toLowerCase());
        if (settings.type.toLowerCase() == "post") {
            loading.show();
            console.log("loading");
        }
    });

    $(document).ajaxStop(function () {
        loading.hide();
    });
});