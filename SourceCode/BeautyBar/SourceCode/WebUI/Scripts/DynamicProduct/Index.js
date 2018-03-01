
productconfig = {
    PageIndex: 2,
    PageSize: 10
}

// Load Sản phẩm từ Select2
Select2_Custom("/Product/GetProductId", "ProductId");

$("select[name='ProductId']").on("change", function (e) {
    // console.log(e.target.textContent);//name
    var Name = e.target.textContent;
    var Product = Name.substring(Name.indexOf(" | ") + 3 );
    $("input[name='ProductName']").val(Product);
});

function LoadContent() {
    loading2();
    $.ajax({
        type: "POST",
        url: "/DynamicProduct/_SearchPartial?PageIndex=" + productconfig.PageIndex + "&PageSize=" + productconfig.PageSize,
        data: $("#formload").serialize(),
        success: function (data) {
            //Load lại kết quả
            $("#Productcontent").html(data);
            //Tính tổng dòng
            var TotalRow = $("#TotalRow").val();
            //2 giây sau => thực hiện phân trang 
            setTimeout(Paging(TotalRow, function () {
                LoadContent();
            }), 20000);
        }
    });
}

function Paging(TotalRow, Callback) {
    if (TotalRow == 0) {
        TotalRow = 1;
    }
    $('#paging').twbsPagination({
        totalPages: Math.ceil(TotalRow / productconfig.PageSize),
        visiblePages: 3,
        first: '<<',
        prev: '<',
        next: ">",
        last : ">>",
        onPageClick: function (event, page) {
            productconfig.PageIndex = page;
            setTimeout(Callback, 200);
        }
    });
}

LoadContent();

$(document).on("click", "#btntim", function () {
    $('#paging').twbsPagination('destroy');
    LoadContent();
});

function recreateDatatable() {
    $('.dataTable').DataTable({
        language: {
            sProcessing: "Đang xử lý...",
            sLengthMenu: "Xem _MENU_ mục",
            sZeroRecords: "Không tìm thấy dữ liệu",
            sInfo: "Đang xem _START_ đến _END_ trong tổng số _TOTAL_ mục",
            sInfoEmpty: "Đang xem 0 đến 0 trong tổng số 0 mục",
            sInfoFiltered: "(được lọc từ _MAX_ mục)",
            sInfoPostFix: "",
            sSearch: "Tìm nội dung:",
            sUrl: "",
            oPaginate: {
                sFirst: "Đầu",
                sPrevious: "&laquo;",
                sNext: "&raquo;",
                sLast: "Cuối"
            },
            columnDefs: [
            { targets: [0, 1], visible: true },
            { targets: 'no-sort', visible: false }
            ]
        },
        "bLengthChange": false,
        "bInfo": false,
        //"bPaginate" : false,
        "sDom": '<"top"flp>rt<"bottom"i><"clear">',
    });
}

// In mã vạch
$(document).on("click", ".btn-in", function () {
    var code = $(this).data("code");
    if (code == "") {
        $("#divPopup #content").html("Sản phẩm này chưa có 'Mã code' !");
        $("#divPopup").modal("show");
    }
    else {
        //alert(code);
        var id = $(this).data("id");
        var storecode = $(this).data("storecode");
        $.ajax({
            type: "POST",
            url: "/DynamicProduct/PrintBarCode?ProductId=" + id,
            success: function (data) {
                //$("#divPopupBarcode #contentBarcode").html(data);
                //$("#divPopupBarcode").modal("show");
                $("#resuiltGetBarcodeToPrint").html(data);
                var docprint = window.open("ISDCorp", "_blank");
                var oTable = document.getElementById("resuiltGetBarcodeToPrint");
                docprint.document.open();
                docprint.document.write('<html><head><title>' + storecode + ' </title>');
                docprint.document.write('</head><body style=\'max-width: 180px;max-height:80px;\'><center>');
                docprint.document.write(oTable.parentNode.innerHTML);
                docprint.document.write('</center></body></html>');
                docprint.document.close();
                //docprint.print();
                //docprint.close();
            }
        });
    }
});
$(document).on("click", "#btnconfirmPrint", function () {
    var docprint = window.open("ISDCorp", "_blank");
    var oTable = document.getElementById("contentBarcode");
    docprint.document.open();
    docprint.document.write('<html><head><title>In barcode</title>');
    docprint.document.write('</head><body><center>');
    docprint.document.write(oTable.parentNode.innerHTML);
    docprint.document.write('</center></body></html>');
    docprint.document.close();
    //docprint.print();
    //docprint.close();
});

//#region Export
$(document).on("click", "#btnExport", function () {
    loading2();
    $.ajax({
        type: "POST",
        url: "/DynamicProduct/Export",
        //data: $("#frmSearch").serialize(),
        success: function (data) {
            var response = data;
            window.location = '/DynamicProduct/Download?fileGuid=' + response.FileGuid
                              + '&filename=' + response.FileName;
        }
    });
});
//#endregion
