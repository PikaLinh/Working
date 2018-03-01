(function ($) {
    function resetTextBox() {
        $("#txtDays").val("");
        $("#txtQty").val("");
        $("#txtDiscount").val("");
    }
    $(document).on("click", "#btnAddDisscount", function () {
        var $btn = $(this).button('loading');
        var data = $("#frm").serializeArray();
        $.ajax({
            type: "POST",
            data: data,
            url: "/Calendar/_AddDiscount",
            success: function (data) {
                $btn.button('reset');
                $("#discountList").html(data);
                resetTextBox();
                $("#txtDays").focus();
            }
        });
    })
    $(document).on("click", ".remove-discount", function () {
        var $btn = $(this).button('loading');
        var data = $("#frm").serializeArray();
        $.ajax({
            type: "POST",
            data: data,
            url: "/Calendar/_RemoveDiscount?STT=" + $(this).data("stt"),
            success: function (data) {
                $btn.button('reset');
                $("#discountList").html(data);
                $("#txtDays").focus();
            }
        });        
    });
})(jQuery);