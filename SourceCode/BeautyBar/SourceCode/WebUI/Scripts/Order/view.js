(function ($) {
    $(document).on("click", "#btnEdit", function () {
        var id = $(this).data("id");
        var url = "/Order/Edit/" + id;
        window.location.href = url;
    });
    $(document).on("click", "#btnBack", function () {
        var url = "/Home/Index";
        window.location.href = url;
    });

    
})(jQuery);