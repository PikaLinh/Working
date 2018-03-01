(function ($) {
    $(document).on("click", "#btnSave", function () {
        var $btn = $(this).button('loading');
        var data = GetData();
        var data2 = $("#frmHeader").serializeArray();
        for (var i = 0; i < data2.length; i++) {
            data.push(data2[i]);
        }

        $.ajax({
            type: "POST",
            data: data,
            url: "/Order/Update",
            success: function (data) {
                $btn.button('reset');
                if (data == "success") {
                    alert("Đơn hàng đã được cập nhật thành công!");
                    window.location = "/";
                } else {
                    alert(data);
                }
            }
        });
    });

})(jQuery);