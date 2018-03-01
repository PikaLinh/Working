(function ($) {
    $(document).on("change", "#FromEmail", function () {
        var fromEmail = $(this).val();
        $.ajax({
            type: "GET",
            data: { fromEmail: fromEmail },
            url: "/UpdateEmail/GetEffect",
            success: function (data) {
                $("#effectTo").html(data);
            }
        });
    });
    $(document).on("change", "input[name='chkAll']", function () {
        $("input[name='CustomerId']").prop("checked", $(this).prop("checked"));
    });

    $(document).on("change", "input[name='CustomerId']", function () {
        var check = true;
        $("input[name='CustomerId']").each(function () {
            if ($(this).prop("checked") == false) {
                check = false;
            }
        });
        $("input[name='chkAll']").prop("checked", check);
    });
})(jQuery);