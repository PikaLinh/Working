//Select2_Custom("/Sell/GetSaleId", "SaleId");
$("#divQuater").hide();
$("#divMonth").hide();

$("select[name='condition']").change(function () {
    var c = $(this).val();
    switch (c) {
        case 'q':
            $(".DivToHide").hide();
            $("#divQuater").show();
            break;
        case 'm':
            $(".DivToHide").hide();
            $("#divMonth").show();
            break;
        default:
            $(".DivToHide").hide();
            $("#divDay").show();
    }
});
// Xem chi tiết đơn hàng bán được trong khoảng thời gian đó
$(document).on("click", ".ViewDetail", function () {
    var Date = $(this).data("id");
    var StoreId = $("#StoreId option:selected").val();
    var CashierUserId = $("#CashierUserId option:selected").val();
    var StaffId = $("#StaffId option:selected").val();
    //alert(Date);
    $.ajax({
        type: "POST",
        url: "/GrossRevenueReport/_GetDatailOrder",
        data: {
            Date: Date,
            StoreId: StoreId,
            CashierUserId: CashierUserId,
            StaffId: StaffId
        },
        success: function (data) {
            $("#divPopup2 #content").html(data);
            $("#divPopup2 .modal-header h1").html($("#Date").val());
            $("#divPopup2").modal("show");
        }        
    });
});

$(document).on("click", "#btntim", function () {
    var c = $("select[name='condition']").val();
    var url = GetUrl(c);
    GetReportGrid(url);
   

});

function GetReportGrid(urlpara) {
    var data = $("#frmExport").serialize();
    $.ajax({
        type: "POST",
        url: urlpara,
        data: data,
        success: function (data) {
            $("#contentGrossRevenueReport").html(data);
        },
        complete : function () {
            LineChart();
        }
    });
}

function GetUrl(condition) {
    var Url = "";
    switch (condition) {
        case 'q':
            Url = "/GrossRevenueReport/_ReportPartialViewQ";
            break;
        case 'm':
            Url = "/GrossRevenueReport/_ReportPartialViewM";
            break;
        default:
            Url = "/GrossRevenueReport/_ReportPartialViewD";
            break;
    }
    return Url;
}

// Vẽ biểu đồ Line Chart with Flot Js
function LineChart() {
    var CheckHasList = $("input[name='CheckHasList']").val();
    //alert(CheckHasList);
    if (CheckHasList == "true") {
        var Revenue = [],
         Profit = [];

        $(".ViewTime").each(function () {
            var row = $(this).data("row");
            var ViewTimeValue = $("input[name='ViewTime_" + row + "']").val();
            var RevenueValue = $("input[name='Revenue_" + row + "']").val();
            var ProfitValue = $("input[name='Profit_" + row + "']").val();
            //console.log(RevenueValue);
            Revenue.push([ViewTimeValue, RevenueValue]);
            Profit.push([ViewTimeValue, ProfitValue]);
        });

        var dataset = [
           { data: Revenue, label: "Doanh thu", color: "#005CDE" }, //points: { symbol: "triangle"}
           { data: Profit, label: "Lợi nhuận" }
        ];

        var ThietLapHienThi = {
            series: {
                lines: {
                    show: true
                },
                points: {
                    show: true
                }
            },
            grid: {
                hoverable: true,
                clickable: true
            },
            xaxis: {
                mode: "categories",
                tickLength: 5

            },
            yaxis: {tickFormatter: function numberWithCommas(x) {
                return x.toString().replace(/\B(?=(?:\d{3})+(?!\d))/g, ",");
                                                                }
            }
        };
        var plot = $.plot("#placeholder", dataset, ThietLapHienThi);

      


        // hover chuột sẽ hiển thị giá trị tại point đó
        $("#placeholder").bind("plothover", function (event, pos, item) {
            if (item) {
                var x = item.datapoint[0],
                    y = item.datapoint[1];

                //$("#tooltip").html(item.series.label + " of " + x + " = " + y)
                //	.css({ top: item.pageY + 5, left: item.pageX + 5 })
                //	.fadeIn(200);
                showTooltip(item.pageX, item.pageY, item.series.color,
                        "<strong>" + item.series.label + " = </strong> <strong class=\"color-red\" " + item.series.label +
                        "</strong>" +  y.toString().replace(/\B(?=(?:\d{3})+(?!\d))/g, ",") );
            } else {
                $("#tooltipflot").remove();
            }
        });
    }
    else {
        $("#placeholder").html("");
    }
};
function showTooltip(x, y, color, contents) {
    $('<div id="tooltipflot"  class=\"text-left\">' + contents + '</div>').css({
        position: 'absolute',
        display: 'none',
        top: y - 40,
        left: x,
        border: '2px solid ' + color,
        padding: '3px',
        'font-size': '9px',
        'border-radius': '5px',
        'background-color': '#fff',
        'font-family': 'Verdana, Arial, Helvetica, Tahoma, sans-serif',
        opacity: 0.9
        ,width : "150px"
    }).appendTo("body").fadeIn(200);
}