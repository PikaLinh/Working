productconfig = {
    PageIndex: 2,
    PageSize: 10
}
function LoadContent() {
    loading2();
    $.ajax({
        type: "POST",
        url: "/Salary/_SearchPartial?PageIndex=" + productconfig.PageIndex + "&PageSize=" + productconfig.PageSize,
        data: $("#formload").serialize(),
        success: function (data) {
            //Load lại kết quả
            $("#SalaryContent").html(data);
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