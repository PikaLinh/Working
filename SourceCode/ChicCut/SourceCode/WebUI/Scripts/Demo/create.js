$(document).on("click", "#btnSave", function () {

    $.ajax({
        type: "POST",
        data: $("#frmHeader").serializeArray(),
        url: "/Demo/Save",
        success: function (returnData) {
            alert(returnData);
        }
    });

    return false;
});