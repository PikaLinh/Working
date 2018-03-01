$("#btnAddNewRow").unbind("click").click(function () {
    var $btn = $(this).button('loading');
    var data = $("#frmList").serializeArray();
    $.ajax({
        type: "POST",
        url: "/ImportMaster/_CreatelistInner",
        data: data,
        success: function (data) {
            $btn.button('reset');
            $("#tblImportDetail tbody").html(data);
        }
    });

    // $btn.button('reset');
});
// bước 2 : xử lý button Xoá
// Bước 2 xử lý tetail-btndelete
$(document).on("click", ".detail-btndelete", function () {
    var $btn = $(this).button('loading');
    var data = $("#frmList").serializeArray();
    var removeId = $(this).data("row");
    $.ajax({
        type: "POST",
        url: "/ImportMaster/_DeletelistInner?RemoveId=" + removeId,
        data: data,
        success: function (data) {
            $btn.button('reset');
            $("#tblImportDetail tbody").html(data);
        }
    });

    // $btn.button('reset');
    return false;
});