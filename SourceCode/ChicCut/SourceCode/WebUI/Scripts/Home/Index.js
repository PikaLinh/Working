(function ($) {
    $(document).on("click", "#btnSearchOrder", function () {
        var $btn = $(this).button('loading');
        var data = $("#frmSearch").serializeArray();
        $.ajax({
            type: "POST",
            data: data,
            url: "/Order/_List",
            success: function (data) {
                $btn.button('reset');
                $("#OrderList").html(data);
            }
        });
        return false;
    });
    $(document).on("click", ".data-row-view", function () {
        var orderId = $(this).data("orderid");
        window.location.href = "/Order/View/" + orderId;
    });
})(jQuery);