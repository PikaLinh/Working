$(document).on("change", "#isHot", function () {
    // alert($('#isHot').prop("checked"));
    if ($('#isHot').prop("checked") == true) {
        $('#divOrderIndex').show();
    }
    else {
        $('#divOrderIndex').hide();
        $('#OrderIndex').val("");
    }
});

