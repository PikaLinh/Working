Select2_Custom("/Employee/GetEmployeeId", "EmployeeId");
$("select[name='EmployeeId']").on("change", function (e) {
    // console.log(e.target.textContent);//name
    var Name = e.target.textContent;
    var Employee = Name.substring(Name.indexOf(" | ") + 3);
    $("input[name='EmployeeName']").val(Employee);
});


// Load Sản phẩm từ Select2
Select2_Custom("/Product/GetProductId", "ProductId");

$("select[name='ProductId']").on("change", function (e) {
    // console.log(e.target.textContent);//name
    var Name = e.target.textContent;
    var Product = Name.substring(Name.indexOf(" | ") + 3);
    $("input[name='ProductName']").val(Product);
});

function Select2_Custom(url, ListProductId, selectedId, selectedName, divList) {
    $("select[name='" + ListProductId + "']").select2({
        ajax: {
            url: url,
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    q: params.term, // search term
                    page: params.page
                };
            },
            processResults: function (data) {
                return {
                    results: $.map(data, function (obj) {
                        return { id: obj.value, text: obj.text };
                    })
                };
            }
        , minimumInputLength: 0// Tối thiếu 1 kí tự thì mới search

        }, allowClear: true
    });
    // $("#divProductId .select2-selection__rendered").html("Hiển thị giá trị mặc định");
    if (selectedId != undefined && selectedName != undefined) {
        if (divList != undefined) {
            $("#" + divList + " select[name='" + ListProductId + "']").html("<option value='" + selectedId + "'>" + selectedName + "</option>");
            $("#" + divList + " .select2-selection__rendered").html("<option value='" + selectedId + "'>" + selectedName + "</option>");
        } else {
            $("select[name='" + ListProductId + "']").html("<option value='" + selectedId + "'>" + selectedName + "</option>");
            $(".select2-selection__rendered").html("<option value='" + selectedId + "'>" + selectedName + "</option>");
        }

    }
}

Select2_Custom("/InventoryMaster/GetInventoryMasterId", "InventoryMasterId");

$("select[name='InventoryMasterId']").on("change", function (e) {
    // console.log(e.target.textContent);//name
    var Name = e.target.textContent;
    var InventoryCode = Name.substring(Name.indexOf(" | ") + 3);
    $("input[name='InventoryCode']").val(InventoryCode);
});

$(document).on("click", "#btnSearch", function () {
    var data = $("#formload").serialize();
    $.ajax({
        url: "/Inventory/_SearchInventory",
        data: data,
        success: function (htmlData) {
            console.log("success");
            $("#content").html(htmlData);

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

            $(".dataTables_filter").prepend("<span class='search-icon'></span>");
            $(".dataTables_filter").append($(".has-btn-add-new").html());
            $(".has-btn-add-new").html("");
        }
    });
});