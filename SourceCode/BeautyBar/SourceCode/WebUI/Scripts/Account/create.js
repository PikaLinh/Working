(function ($) {
    $(document).on("change", "#CustomerNumber", function () {
        var customerNumber = $(this).val();
        if (!isNaN(customerNumber)) {
            $.ajax({
                type: "POST",
                data: { CustomerNumber: customerNumber },
                url: "/Account/GetMoreInfo",
                success: function (data) {
                    if (data != "") {
                        //m.CustomerNumber = Convert.ToInt32(row["KUNNR"]); // ma customer 10 ky tu
                        //m.Name = row["NAME"].ToString();
                        $("#EnterpriseName").val(data.Name);
                        //m.ReceivedAddress = row["ADDRESS"].ToString();
                        $("#ReceivedAddress").val(data.ReceivedAddress);
                        //m.Tax = row["TAX"].ToString();
                        //m.Phone = row["TEL_NUMBER"].ToString();
                        $("#Phone").val(data.Phone);
                        //m.Fax = row["FAX_NUMBER"].ToString();
                        $("#Fax").val(data.Fax);
                        //m.Email = row["EMAIL"].ToString();
                        $("#UserName").val(data.Email);
                    }
                }
            });
        }
    });
})(jQuery);