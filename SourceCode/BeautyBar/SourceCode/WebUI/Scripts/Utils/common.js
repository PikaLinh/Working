(function ($) {

    $.fn.serializeAnything = function () {

        var toReturn = {};
        var els = $(this).find(':input').get();

        $.each(els, function () {
            if (this.name){
                if (/select|textarea/i.test(this.nodeName) || /text|date|hidden|password/i.test(this.type)) {
                    var val = $(this).val();
                    toReturn[this.name] = val;
                } else if (/checkbox/i.test(this.type)){
                    var val = $(this).is(':checked');
                    toReturn[this.name] = val;
                }
                else if (/radio/i.test(this.type)) {
                    if ($(this).is(':checked')) {
                        toReturn[this.name] = $(this).val();
                    }
                }
            } 
            
        });

        return toReturn;
    }

    $.fn.serializeAnythingWithoutRegion = function (withoutParent) {

        var toReturn = {};
        var els = $(this).find(':input').filter(function (index) {
            return $(this).parents(withoutParent).length == 0;
        }).get();

        $.each(els, function () {
            if (this.name) {
                if (/select|textarea/i.test(this.nodeName) || /text|date|hidden|password/i.test(this.type)) {
                    var val = $(this).val();
                    toReturn[this.name] = val;
                }
                else if (/checkbox/i.test(this.type)) {
                    var val = $(this).is(':checked');
                    toReturn[this.name] = val;
                }
                else if (/radio/i.test(this.type)) {
                    if ($(this).is(':checked')) {
                        toReturn[this.name] = $(this).val();
                    }
                }
            }

        });

        return toReturn;
    }

    $.fn.populate = function (data, name) {
        var self = this;
        $.each(data, function (key, value) {
            var $ctrl = null;
            if (typeof name !== "undefined") {
                $ctrl = $("[name='" + name + "." + key + "']", $(self));
            }
            else {
                $ctrl = $('[name=' + key + ']', $(self));
            }
            
            switch ($ctrl.attr("type")) {
                case "text":
                case "date":
                case "hidden":
                    $ctrl.val(value);
                    break;
                case "radio": case "checkbox":
                    $ctrl.each(function () {
                        if ($(self).attr('value') == value) { $(self).attr("checked", value); }
                    });
                    break;
                default:
                    $ctrl.val(value);
            }
        });
    };

    $.fn.preventNonNumberic = function () {
        $(this).keydown(function (e) {

            // Allow: backspace, delete, tab, escape, enter and .
            if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                // Allow: Ctrl+A
                (e.keyCode == 65 && e.ctrlKey === true) || 
                // Allow: home, end, left, right, down, up
                (e.keyCode >= 35 && e.keyCode <= 40)) {
                // let it happen, don't do anything
                return;
            }


            // Ensure that it is a number and stop the keypress
            if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                e.preventDefault();
            }
        });
    };

    /**
 * Number.prototype.format(n, x, s, c)
 * 
 * @param integer n: length of decimal
 * @param integer x: length of whole part
 * @param mixed   s: sections delimiter
 * @param mixed   c: decimal delimiter
 */
    Number.prototype.format = function (n, x, s, c) {
        var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\D' : '$') + ')',
            num = this.toFixed(Math.max(0, ~~n));

        return (c ? num.replace('.', c) : num).replace(new RegExp(re, 'g'), '$&' + (s || ','));
    };

    //var numbers = [1, 12, 123, 1234, 12345, 123456, 1234567, 12345.67, 123456.789];

    //document.write('<p>Classic Format:</p>');
    //for (var i = 0, len = numbers.length; i < len; i++) {
    //    document.write(numbers[i].format(2, 3, '.', ',') + '<br />');
    //}



})(jQuery);


function aspJsonDateToJsDate(str) {
    return new Date(parseInt(str.substr(6)));
}

function floatToString(flt) {
    if (flt != '' && flt != null) {
        return flt.toFixed(0).replace(/./g, function (c, i, a) {
            return i > 0 && c !== "." && (a.length - i) % 3 === 0 ? "," + c : c;
        });
    } else {
        return '0';
    }
}

function stringToFloat(str) {
    return str.replace(/,/g, "");
}

function openPrint(tmpl, obj) {
    return openPrintHtml(kendo.template(tmpl)(obj));
}

function openPrintHtml(html) {
    var dummyContent = "<html><body style='width:270px;height:auto'>" + html + "</body></html>";
    var printWindow = window.open('', '', 'height=0,width=0');
    printWindow.document.write(dummyContent);
    printWindow.print();
    printWindow.close();
    return dummyContent;
}

function previewPrint(tmpl, obj) {
    var content = kendo.template(tmpl)(obj);
    $("#dlgPreviewPrintReserve").modal();
    $("#divPreviewPrintContent").html(content);
}

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

function app_handle_listing_horisontal_scroll(listing_obj) {
    //get table object   
    table_obj = $('.table', listing_obj);

    //get count fixed collumns params
    count_fixed_collumns = table_obj.attr('data-count-fixed-columns')
    console.log(count_fixed_collumns);
    if (count_fixed_collumns > 0) {
        //get wrapper object
        wrapper_obj = $('.table-scrollable', listing_obj);

        wrapper_left_margin = 0;

        table_collumns_width = new Array();
        table_collumns_margin = new Array();

        //calculate wrapper margin and fixed column width
        $('th', table_obj).each(function (index) {
            if (index < count_fixed_collumns) {
                wrapper_left_margin += $(this).outerWidth();
                table_collumns_width[index] = $(this).outerWidth();
            }
        })

        //calcualte margin for each column  
        $.each(table_collumns_width, function (key, value) {
            if (key == 0) {
                table_collumns_margin[key] = wrapper_left_margin;
            }
            else {
                next_margin = 0;
                $.each(table_collumns_width, function (key_next, value_next) {
                    if (key_next < key) {
                        next_margin += value_next;
                    }
                });

                table_collumns_margin[key] = wrapper_left_margin - next_margin;
            }
        });

        //set wrapper margin               
        if (wrapper_left_margin > 0) {
            wrapper_obj.css('cssText', 'margin-left:' + wrapper_left_margin + 'px !important; width: auto')
        }

        //set position for fixed columns
        $('tr', table_obj).each(function () {

            //get current row height
            current_row_height = $(this).outerHeight();

            $('th,td', $(this)).each(function (index) {

                //set row height for all cells
                //$(this).css('height', current_row_height)
                $(this).css('height', "41px")

                //set position 
                if (index < count_fixed_collumns) {
                    $(this).css('position', 'absolute')
                           .css('margin-left', '-' + table_collumns_margin[index] + 'px')
                           .css('width', table_collumns_width[index])

                    $(this).addClass('table-fixed-cell')
                }
            })
        })
    }
}