    $(document).on("click", "#btnsearchproduct", function () {
        loading2();
        $.ajax({
            type: "POST",
            url: "/ProductUpdates/_ProductPartial",
            data: $("#frmtimproduct").serialize(),
            success: function (data) {
                $("#contentproduct").html(data);
                //recreateDatatable();
                $("body").removeClass("loading2");
                Reinitialize('#dataTable1');
            }
        });
    });
    function Reinitialize(datatableName) {
        $(datatableName).DataTable({
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
$(document).ready(function () {
    ////TAB RADIO MANAGEMENT BEGIN ***********************************************************************************
    $("ul.nav-tabs li:first").addClass("active").show(); //Activate first tab
    $(".tab-pane:first").addClass("active").show(); //Show first tab content
});

//Sản phẩm
Select2_Custom("/Checkinfo/GetProductId", "ProductId");

// Export
$(document).on("click", "#btnExport", function () {
    $.ajax({
        type: "POST",
        url: "/ProductUpdates/Export",
        data: $("#frmtimproduct").serialize(),
        success: function (data) {
            var response = data;
            window.location = '/ProductUpdates/Download?fileGuid=' + response.FileGuid
                              + '&filename=' + response.FileName;
        }
    });
});

//Import
// Xử lý btn Tải File
$(document).on("click", "#btnImport", function () {
    loading2();//addClass(loading2) --   $(document).ajaxStop(function () {  $("body").removeClass("loading2");  });
    if (document.getElementById("FileUpload").files[0] == undefined) {
        //alert("Chưa upfile");
        $("body").removeClass("loading2");
        return false;
    } else {
        $("#Messenger").html("Đang Import ...");
        $("#fileupload").ajaxSubmit({
            type: "POST",
            success: function (data) {
                if (data == "success") {
                    $("#Messenger").html("Import thành công !");
                    $.ajax({
                        type: "POST",
                        url: "/ProductUpdates/_ProductUpdateList",
                        success: function (data) {
                            $("#contentProductUpdateList").html(data);
                            $('input[type=file]').val('');
                            $("body").removeClass("loading2");
                            Reinitialize('#datatableProductUpdateList');
                        }
                    });
                }
                else {
                    $("#Messenger").html(data);
                    $("body").removeClass("loading2");
                }
            }
        });

        return false;
    }
});

// Huỷ phiếu cập nhật 
$(document).on("click", ".btn-xoa", function (e) {
    var id = $(this).data("id");
    var code = $(this).data("code");
    $("#idDelete").val(id);
    $(".modal-body strong").html(code);
    //alert(id);
});
$('#confirm-delete').on('click', '.btn-ok', function (e) {
    var $modalDiv = $(e.delegateTarget);
    var id = $("#idDelete").val();
    $.ajax({
        url: '/ProductUpdates/Cancel?id=' + id,
        success: function (data) {
            if (data == "success") {
                $.ajax({
                    type: "POST",
                    url: "/ProductUpdates/_ProductUpdateList",
                    success: function (data) {
                        $("#contentProductUpdateList").html(data);
                        $('input[type=file]').val('');
                        Reinitialize('#datatableProductUpdateList');
                    }
                });
                $.ajax({
                    type: "POST",
                    url: '/ProductUpdates/ProductUpdateDetails',
                    success: function (data) {
                        $("#contentProductUpdatesDetails").html(data);
                        Reinitialize('#datatableproduct');
                    }
                });
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


// Cập nhật sản phẩm
$(document).on("click", ".bt-autoimex", function (e) {
    var id = $(this).data("id");
    var code = $(this).data("code");
    $("#idDelete").val(id);
    $(".modal-body strong").html(code);
    //alert(id);
});
$('#confirm-autoimex').on('click', '.btn-ok', function (e) {
    var $modalDiv = $(e.delegateTarget);
    var id = $("#idDelete").val();
    $.ajax({
        url: '/ProductUpdates/Autoimex?id=' + id,
        success: function (data) {
            if (data == "success") {
                $.ajax({
                    type: "POST",
                    url: "/ProductUpdates/_ProductUpdateList",
                    success: function (data) {
                        $("#contentProductUpdateList").html(data);
                        $('input[type=file]').val('');
                        Reinitialize('#datatableProductUpdateList');
                    }
                });
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

// Xem
$(document).on("click", ".btn-details", function (e) {
    loading2();
    var id = $(this).data("id");
    $.ajax({
        type: "POST",
        url: '/ProductUpdates/ProductUpdateDetails?id=' + id,
        success: function (data) {
            $("#contentProductUpdatesDetails").html(data);
            $("body").removeClass("loading2");
            Reinitialize('#datatableproduct');
        }
    });
    // Chuyển tab hiển thị
    $("ul.nav-tabs li").each(function () {
        if ($(this).hasClass("active")) {
            $(this).removeClass("active");
        }
    });
    $(".tab-pane").each(function () {
        if ($(this).hasClass("active")) {
            $(this).removeClass("active");
        }
    });
    $("ul.nav-tabs li:last").addClass("active").show(); //Activate last tab
    $(".tab-pane:last").addClass("active").show(); //Show first tab contentư
});


// Xoá sản phẩm
$(document).on("click", ".btn-xoa", function (e) {
    var id = $(this).data("id");
    var name = $(this).data("name");
    var masterid = $(this).data("row");
    console.log(masterid);
    $("#idDeleteProduct").val(id);
    $("#masterId").val(masterid);
    $(".modal-body strong").html(name);
    //alert(id);
});

$('#confirm-delete-product').on('click', '.btn-ok', function (e) {
    var $modalDiv = $(e.delegateTarget);
    var id = $("#idDeleteProduct").val();
    var masterid = $("#masterId").val();
    $.ajax({
        url: '/ProductUpdates/DeleteProduct?id=' + id,
        success: function (data) {
            if (data == "success") {
                $.ajax({
                    type: "POST",
                    url: "/ProductUpdates/ProductUpdateDetails?id=" + masterid,
                    success: function (data) {
                        $("#contentProductUpdatesDetails").html(data);
                        Reinitialize('#datatableproduct');
                    }
                });
                // Cập nhật lại danh sách cập nhật
                $.ajax({
                    type: "POST",
                    url: "/ProductUpdates/_ProductUpdateList",
                    success: function (data) {
                        $("#contentProductUpdateList").html(data);
                        Reinitialize('#datatableProductUpdateList');
                    }
                });
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
