productconfig = {
    PageIndex: 2,
    PageSize: 10
}

Select2_Custom("/PreImportMaster/GetPreImportMasterId", "PreImportMasterId");

$("select[name='PreImportMasterId']").on("change", function (e) {
    // console.log(e.target.textContent);//name
    var Name = e.target.textContent;
    var ImportMaster = Name.substring(Name.indexOf(" | ") + 3);
    $("input[name='PreImportMasterCode']").val(ImportMaster);
});
// Load Sản phẩm từ Select2
Select2_Custom("/Product/GetProductId", "ProductId");

$("select[name='ProductId']").on("change", function (e) {
    // console.log(e.target.textContent);//name
    var Name = e.target.textContent;
    var Product = Name.substring(Name.indexOf(" | ") + 3);
    $("input[name='ProductName']").val(Product);
});


function LoadContent() {
    loading2();
    $.ajax({
        type: "POST",
        url: "/PreImportMaster/_SearchPreImportMaster?PageIndex=" + productconfig.PageIndex + "&PageSize=" + productconfig.PageSize,
        data: $("#formload").serialize(),
        success: function (data) {
            $("#PreImportcontent").html(data);
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

$(document).on("click", "#btnSearch", function () {
    $('#paging').twbsPagination('destroy');
    LoadContent();
});
//==========javascrip btn-delete ====================
$(document).on("click", ".btn-xoa", function (e) {
    var id = $(this).data("id");
    var row = $(this).data("row");
    var Code = $("#PreImportMasterCode_" + row).val();
    $("#idDelete").val(id);
    $(".modal-body strong").html(Code);
});
//  $('#confirm-delete').on('click', '.btn-ok', function (e)
$(document).on('click', '.btn-ok', function (e) {
    var $modalDiv = $(e.delegateTarget);
    var id = $("#idDelete").val();
    $.ajax({
        url: '/PreImportMaster/Cancel?id=' + id,
        success: function (data) {
            if (data == "success") {
                window.location.assign("/PreImportMaster/Index");
            }
            else {
                $("#divPopup #content").html(data);
                $("#divPopup").modal("show");
            }
        }
    })
    // $.post('/api/record/' + id).then()
    $modalDiv.addClass('loading');
    setTimeout(function () {
        $modalDiv.modal('hide').removeClass('loading');
    }, 1000)
});
