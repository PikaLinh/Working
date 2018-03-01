$(document).ready(function () {
    $("#notificationLink").click(function () {
        $("#notificationContainer").fadeToggle("slow");
        //$("#notification_count").fadeOut("slow");
        return false;
    });
    LoadNewNotification();
});

$(document).on("click", "#notificationFooter", function () {
    //alert("123");
});

function LoadNewNotification() {
    $.ajax({
        type: 'Post',
        url: "/Home/_GetNewNotification",
        success: function (data) {
            $("#notificationsBody").html(data);
            var count = $("#Notifi_Count").val();
            if (Number(count) > 0) {
                $("#notification_count").html(count);
                $("#notification_count").css("display", "block");
            }
            else {
                $("#notification_count").css("display", "none");
            }
        }
    });
}
$(document).on("click", "body", function myfunction() {
    $("#notificationContainer").fadeOut("slow");
});
$(document).on("click", ".dropdown a", function myfunction() {
    $("#notificationContainer").fadeOut("slow");
});
