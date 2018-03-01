$(document).on("click", "#btnView", function () {
    var data = $("#frmExport").serialize();
    $.ajax({
        type: "POST",
        url: "/DailyServicesReport/_ReportPartial",
        data: data,
        success: function (data) {
            $("#ReportDetail").html(data);
        }
        //,
        //complete: function () {
        //    LineChart();
        //}
    });
});

$(document).on("click", "#btnPrint", function () {
    var data = $("#frmExport").serialize();
    $.ajax({
        type: "POST",
        url: "/DailyServicesReport/Print",
        data: data,
        success: function (result, stt, jqXHR) {
            //$("#divPopup2").modal("hide");
            //openPrint($("#tmplInvoicePrint").html(), {
            //    Order: result.Data
            //});
            console.log(result);
            console.log(result.Data);

            openPrint($("#tmplDailyServicesReportPrint").html(), {
                Order: result.Data
            });
        }
    });
});

$(document).on("click", "#btnSendSMS", function () {
    $(this).remove();
    var fromDate = $("input[name=FromDate]").val();
    var toDate = $("input[name=ToDate]").val();
    var total = $("input[name=Total]").val();
    $.ajax({
        type: "POST",
        url: "/DailyServicesReport/SendSMS",
        data: {
            FromDate: fromDate,
            ToDate: toDate,
            Total: total
        },
        success: function (result, stt, jqXHR) {
            if (result.Success == true) {
                $("#content").html("Đã gửi tin nhắn thành công!");
                $("#divPopup").modal("show");
            }
            else
            {
                $("#content").html(result.ErrorMessage);
                $("#divPopup").modal("show");
            }
        }
    });
    
});