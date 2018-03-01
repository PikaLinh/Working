function Alert_Popup(Centent_Alert)
{
    $.ajax({
        type: "POST",
        url: "/Home/Alert",
        data: {
            Content: Centent_Alert
        },
        datatype: "json",
        success: function (jsondata) {
            $("#divAlert").html(jsondata)
            $("#divPopup").modal("show");
        }
    });

    return false;
}

function Alert_Popup_Delete(Centent_Alert) {
    $.ajax({
        type: "POST",
        url: "/Home/Alert_Delete",
        data: {
            Content: Centent_Alert
        },
        datatype: "json",
        success: function (jsondata) {
            $("#divAlert").html(jsondata)
            $("#divPopup").modal("show");
        }
    });

    return false;
}