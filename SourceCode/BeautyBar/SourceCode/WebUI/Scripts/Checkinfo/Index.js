//Sản phẩm
Select2_Custom("/Checkinfo/GetProductId", "ProductId");
$(document).ready(function () {
    ////TAB RADIO MANAGEMENT BEGIN ***********************************************************************************
    $("ul.nav-tabs li:first").addClass("active").show(); //Activate first tab
    $(".tab-pane:first").addClass("active").show(); //Show first tab content

    // Bắt sự kiện enter
    document.getElementById('txtdt').onkeypress = function (e) {
        if (!e) e = window.event;
        var keyCode = e.keyCode || e.which;
        if (keyCode == '13') {
            //alert("123");
            GetCusInfor();
            return false;
        }
    }
});

$(document).on("click", "#btnsearch", function () {
    if ($("#txtdt").val() != '') {
        GetCusInfor();
    }
});

function GetCusInfor() {
    var sdt = $("#txtdt").val();
    $.ajax({
        type: "POST",
        url: "/Checkinfo/_CustomerPartial",
        data: {
            dienthoai: sdt
        },
        success: function (data) {
            $("#content").html(data);
        },
        complete: function () {
            var CusId = $("#CustomerId").val();
            if (CusId != undefined || CusId > 1) {
                //alert(CusId);
                GetOrderDetail(CusId);
            }
        }
    });
}

function GetOrderDetail(id) {
    var sdt = $("#txtdt").val();
    $.ajax({
        type: "POST",
        url: "/Checkinfo/_OrderDetailPartial?CustomerId=" + id,
        success: function (data) {
            $("#contentDetailDebt").html(data);
        }
    });
}

$(document).on("click", "#btnsearchproduct", function () {
    loading2();
    $.ajax({
        type: "POST",
        url: "/Checkinfo/_ProductPartial",
        data: $("#frmtimproduct").serialize(),
        success: function (data) {
            $("#contentproduct").html(data);
            recreateDatatable();
        }
    });
});

