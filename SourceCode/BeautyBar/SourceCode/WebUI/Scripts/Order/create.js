(function ($) {
   
    $(document).on("click", "#btnCancel", function () {
        var $btn = $(this).button('loading');
        window.location = "/";
    });

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
            url: "/Order/Save",
            success: function (data) {
                $btn.button('reset');
                if (data == "success") {
                    alert("Đơn hàng đã được lưu thành công!");
                    window.location = "/";
                }else
                {
                    alert(data);
                }
            }
        });
    });
  
    //$(document).on("change", ".detail-tonqty", function () {
    //    sumTotal();
    //});
})(jQuery);