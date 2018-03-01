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

//Khi click vào <li> và 'Đã xem' thì sẽ set Actived = false
$(document).on("click", ".ulNotifi li", function () {
    var id = $(this).data("id");
    $("#NotifiIdSeen").val(id);
    // $("#notificationContainer").hide();
    // alert(id);
    $.ajax({
        type: 'post',
        url: '/Home/_NotifiDetail?id=' + id,
        success: function (data) {
            window.location.href = "/DailyChicCutPreOrder/Index?SearchPreOrderCode=" + data.SearchPreOrderCode
        }
    });
});

//$(document).on("click", "#all", function () {
//    var all = $(this).data("id");
//    window.location.href = '/DailyChicCutPreOrder/Index?SearchAll=' + all
//});

$(document).on("click", "#btnSeen", function () {
    var id = $("#NotifiIdSeen").val();
    $.ajax({
        type: 'post',
        url: '/Home/NotifiSeen?id=' + id,
        success: function (data) {
            if (data == "success") {
                //$("#notificationContainer").hide();
                //LoadNewNotification();
                window.location.assign("/Home/Index");
            }
        }
    })
});