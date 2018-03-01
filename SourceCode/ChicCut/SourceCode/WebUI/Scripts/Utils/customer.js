$(function () {
    $(document).on("click", "#btnSearchCustomer", function () {
        searchCustomer();
    });


    $(document).on("change", "#txtPhone", function () {
        searchCustomer();
    });

    function searchCustomer() {
        var strPhone = $("#txtPhone").val();
        if (strPhone.length > 0) {
            var url = "/customer/" + strPhone;
            $.ajax({
                url: url,
                type: "GET",
                success: function (data, stt, jqXHR) {
                    if (data != null) {
                        $("#frmCustomer").populate(data); //ko dùng đc vì thiết kế db không trùng field giữa submit và get
                        $("#lblCustomerNotExistMessage").hide();
                        //$("input[name=FullName]", "#frmCustomer").val(data.FullName);
                        //$("input[name=Address]", "#frmCustomer").val(data.Address);

                    } else {
                        $("#lblCustomerNotExistMessage").show();

                    }
                },
                error: function (jqXHR, stt, err) {
                    console.log(stt);
                }
            });;
        }
        
    }
});