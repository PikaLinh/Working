
// Khách hàng
Select2_Custom("/Sell/GetCustomerId", "CustomerId");

// Load Sản phẩm từ Select2
Select2_Custom("/Product/GetProductId", "ProductId");

$("select[name='CustomerId']").on("change", function (e) {
    // console.log(e.target.textContent);//name
    var Name = e.target.textContent;
    var fullName = Name.trim(Name.substring(Name.indexOf("\n") + 2));
    $("input[name='FullName']").val(fullName);
});
productconfig = {
    PageIndex: 2,
    PageSize: 10
}

Select2_Custom("/PreOrderMaster/GetPreOrderId", "OrderId");

$("select[name='OrderId']").on("change", function (e) {
    // console.log(e.target.textContent);//name
    var Name = e.target.textContent;
    var Product = Name.substring(Name.indexOf(" | ") + 3);
    $("input[name='OrderCode']").val(Product);
});

function LoadContent() {
    loading2();
    $.ajax({
        type: "POST",
        url: "/PreOrderMaster/_Search?PageIndex=" + productconfig.PageIndex + "&PageSize=" + productconfig.PageSize,
        data: $("#formload").serialize(),
        success: function (data) {
            $("#SearchContent").html(data);
            var TotalRow = $("#TotalRow").val();
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
        last: ">>",
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

$(document).on("click", ".btn-xoa", function (e) {
    var id = $(this).data("id");
    var row = $(this).data("row");
    var Code = $("#PreOrderMasterCode_" + row).val();
    $("#idDelete").val(id);
    $(".modal-body strong").html(Code);
});
$(document).on('click', '.btn-ok', function (e) {
    //  var $modalDiv = $(e.delegateTarget);
    var id = $("#idDelete").val();
    $.ajax({
        url: '/PreOrderMaster/Cancel?id=' + id,
        success: function (data) {
            $("#confirm-delete").modal("hide");
            if (data == "success") {
                window.location.assign("/PreOrderMaster/Index");
            }
            else {
                $("#divPopup #content").html(data);
                $("#divPopup").modal("show");
            }
        }
    })
});