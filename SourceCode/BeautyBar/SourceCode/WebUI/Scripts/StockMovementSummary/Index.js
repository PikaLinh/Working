function MarkIsDropFillter() {
    $.ajax({
        type: "POST",
        url: "/StockMovementSummary/MarkIsDropFillter",
        success: function (data) {
            // $("#content").html(data);
        }
    });
}

function ChangedPasize() {
    $.ajax({
        type: "POST",
        url: "/StockMovementSummary/ChangedPasize?NumberRecordPerPage=" + $("select[name='PageSize']").val(),
        success: function (data) {
            // $("#content").html(data);
        }
    });
}



function GetPivotGrid(urlpara) {
    var data = $("#frmExport").serialize();
    $.ajax({
        type: "POST",
        url: urlpara,
        data: data,
        success: function (data) {
            $("#content").html(data);
        }
    });
}


function GetUrl(condition) {
    var Url = "";
    switch (condition) {
        case 'q':
            Url = "/StockMovementSummary/_PivotPartialViewQ";
            $("#frmExport").attr("action", "/StockMovementSummary/ExportQ"); // xét lại action cho form Export
            break;
        case 'm':
            Url = "/StockMovementSummary/_PivotPartialViewM";
            $("#frmExport").attr("action", "/StockMovementSummary/ExportM");
            break;
        default:
            Url = "/StockMovementSummary/_PivotPartialView";
            $("#frmExport").attr("action", "/StockMovementSummary/ExportD");
            break;
    }
    return Url;
}





$(document).on("click", "#btnReportView", function () {
    MarkIsDropFillter();
    // alert("123");
    var c = $("select[name='condition']").val();

    var Url = GetUrl(c);
    GetPivotGrid(Url);
});

$("select[name='condition']").change(function () {
    // alert("123");
    var c = $(this).val();
    switch (c)
    {
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

$("select[name='PageSize']").change(function () {
    //alert("123");
    ChangedPasize();
    var c = $("select[name='condition']").val();

    var Url = GetUrl(c);
    GetPivotGrid(Url);
});


$(document).ready(function () {
   // GetPivotGrid("/StockMovementSummary/_PivotPartialViewQ");
    $("#divDay").hide();
    $("#divMonth").hide();
});