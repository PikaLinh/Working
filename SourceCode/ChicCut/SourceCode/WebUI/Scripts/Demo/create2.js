function customAlert(message) {
    alert(message);
}

$(document).on("click", "#btnSave", function () {

    $.ajax({
        type: "POST",
        data: $("#frmMaster").serialize() + '&' + $("#frmDetail").serialize(),
        url: "/Demo/Save2",
        success: function (aaavvvzzz) {
            if (aaavvvzzz == "success") {
                if ($("input[name='isRI']:checked").val() == true) {
                    window.location = "/";
                } else {
                    customAlert("thành cồng");
                }
            } else {
                customAlert(aaavvvzzz);
            }
        }
    });

    return false;
});