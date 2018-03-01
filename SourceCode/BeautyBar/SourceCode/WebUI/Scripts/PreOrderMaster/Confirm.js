$(document).on("click", "#btnConfirm", function () {
    var data = $("#frmList").serialize() + "&" + $("#frmHeader").serialize();
    $.ajax({
        type: "POST",
        url: "/PreOrderMaster/SaveConfirm",
        data: data,
        success: function (data) {
            if (data == "success") {
                window.location.assign("/PreOrderMaster/Index");
            }
            
        }
    });
});