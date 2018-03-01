
// Khách hàng
Select2_Custom("/Employee/GetEmpoyeeId", "EmployeeId");
LoadContent();

function LoadContent() {
    loading2();
    $.ajax({
        type: "POST",
        url: "/Employee/_SearchEmployee",
        data: $("#formload").serialize(),
        success: function (data) {
            $("#contentEmployee").html(data);
        }
    });
}

$(document).on("click", "#btntim", function () {
    LoadContent();
});
