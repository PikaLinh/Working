function GetData() {
    var data = $("#frmList").serializeArray();
    for (var i = 0; i < data.length; i++) {
        if (data[i].name.match(/^.*.TONQty/)) {
            data[i].value = data[i].value;
        }
    }
    return data;
}

(function ($) {
    function sumTotal() {
        var Total = 0;
        $(".detail-tonqty").each(function (index, value) {
            var thisValue = $(this).val();
            if (thisValue != "") {
                var number = parseFloat(thisValue.replace(/\,/g, ''));
                if (!isNaN(number)) {
                    Total += parseFloat(number);
                }
            }
        });
        $("#sumTotal").html(Total.format(3, 3, ',', '.'));
    }
  
    $("#btnAddNewRow").unbind("click").click(function () {
        var $btn = $(this).button('loading');
        //$(".detail-tonqty").each(function () {
        //    $(this).val($(this).val());
        //});
        var data = GetData();
        $.ajax({
            type: "POST",
            url: "/Order/_CreatelistInner",
            data: data,
            success: function (data) {
                $btn.button('reset');
                $("#tblOrderDetails tbody").html(data);
            }
        });
    });

    $(document).on("click", ".detail-btndelete", function () {
        var $btn = $(this).button('loading');
        var row = $(this).data("row");
        $.ajax({
            type: "POST",
            url: "_Delete?id=" + row,
            data: $("#frmList").serialize(),
            success: function (data) {
                $btn.button('reset');
                $("#tblOrderDetails tbody").html(data);
                return false;
            }
        });
    });

    function resetTextBox(row) {
        $("#detail-rollqty-" + row).val("");
        $("#detail-tonqty-" + row).val("");
    }
    $(document).on("blur", ".detail-steelmarkid", function () {
        //Steel Mark
        var steelmarkId = $(this).val();
        var row = $(this).data("row");
        //Get Steel FI 
        $.ajax({
            type: "GET",
            url: "/SteelFI/GetByMarkId/" + steelmarkId,
            success: function (json) {
                $("#detail-steelfiid-" + row).empty();
                $.each(json, function (i, value) {
                    //alert(value.SteelFIId + ' ' + value.SteelFICode);
                    $("#detail-steelfiid-" + row).append($('<option>').text(value.SteelFICode).attr('value', value.SteelFIId));
                    resetTextBox(row);
                });
            }
        });

    });

    $(document).on("blur", ".detail-steelfiid", function () {
        var row = $(this).data("row");
        resetTextBox(row);
    });

    //detail-rollqty change => update detail-tonqty
   
    $(document).on("blur", ".detail-rollqty", function () {
        var row = $(this).data("row");
        var rollValue = $(this).val();
        var SteelMarkId = $("#detail-steelmarkid-" + row).val();
        var SteelFIId = $("#detail-steelfiid-" + row).val();
        if (rollValue != "" && rollValue != 0) {
            $.ajax({
                type: "POST",
                url: "/Exchange/RollToTON?value=" + rollValue + "&SteelMarkId=" + SteelMarkId + "&SteelFIId=" + SteelFIId,
                success: function (data) {
                    if (data != "") {
                        $("#detail-tonqty-" + row).val(parseFloat(data.replace(/\,/g, '')).format(3, 3, ',', '.'));
                        sumTotal();
                    }
                }
            });
        } else {
            $("#detail-tonqty-" + row).val("");
        }
    });

    //detail-tonqty change => update detail-rollqty
    $(document).on("blur", ".detail-tonqty", function () {
        var row = $(this).data("row");
        var tonqty = $(this).val();
        if (tonqty != "") {
            $(this).val(parseFloat(tonqty.replace(/\,/g, '')).format(3, 3, ',', '.'))
        }
        var SteelMarkId = $("#detail-steelmarkid-" + row).val();
        var SteelFIId = $("#detail-steelfiid-" + row).val();
        
        if (tonqty != "" && tonqty != 0) {
            $.ajax({
                type: "POST",
                url: "/Exchange/TONToRoll?value=" + tonqty + "&SteelMarkId=" + SteelMarkId + "&SteelFIId=" + SteelFIId,
                success: function (data) {
                    if (data != "") {
                        $("#detail-rollqty-" + row).val(data);
                        sumTotal();
                    }
                }
            });
        } else {
            $("#detail-rollqty-" + row).val("");
        }
    });

    function cancel() {

    }
    $(document).on("click", ".btnCancelOrder", function () {
        var $btn = $(this).button('loading');
        var r = confirm("Bạn có thật sự muốn hủy đơn hàng này không!");
        if (r == true) {
            var id = $(this).data("id");
            $.ajax({
                type: "POST",
                url: "/Order/CancelOrder/" + id,
                success: function (data) {
                    $btn.button('reset');
                    if (data == "success") {
                        window.location.href = "/";
                    }
                    else
                    {
                        alert(data);
                    }
                }
            });
        } else {
            $btn.button('reset');
            return false;
        }
    });

    $(document).on("click", "#btnSend", function () {
        var $btn = $(this).button('loading');
        var r = confirm("Bạn có thật sự muốn gửi đơn hàng này không!");
        if (r == true) {
            var mode = $(this).data("mode");

            var data;
            var url = "";
            if (mode == 'add') {
                url = "/Order/Save?isSend=true";
                data = GetData();
                var data2 = $("#frmHeader").serializeArray();
                for (var i = 0; i < data2.length; i++) {
                    data.push(data2[i]);
                }
            } else if(mode == 'edit') {
                url = "/Order/Update?isSend=true";
                data = GetData();
                var data2 = $("#frmHeader").serializeArray();
                for (var i = 0; i < data2.length; i++) {
                    data.push(data2[i]);
                }
            }
            else
            {
                url = "/Order/Send";
                data = {id:$(this).data("id")};
            }

            $.ajax({
                type: "POST",
                data: data,
                url: url,
                success: function (data) {
                    $btn.button('reset');
                    if (data == "success") {
                        alert("Đơn hàng đã được gửi thành công!");
                        window.location = "/";
                    } else {
                        alert(data);
                    }
                }
            });
        } else {
            $btn.button('reset');
            return false;
        }
    });
})(jQuery);