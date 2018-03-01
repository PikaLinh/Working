var phone;
var errorMessage = "";
var defaultStartTime;
var defaultEndTime;
// init setting
var setting;

// waiting find customer
var currentMobile = "";

// init day in week
var weekday = new Array(7);
weekday[1] = "Thứ 2";
weekday[2] = "Thứ 3";
weekday[3] = "Thứ 4";
weekday[4] = "Thứ 5";
weekday[5] = "Thứ 6";
weekday[6] = "Thứ 7";
weekday[0] = "Chủ nhật";

/* time frame */

//var currentHour = formatDisplayTime(new Date());
var currentTime = "";
var startTime = new Date(new Date().setHours(0, 0, 0, 0)).toISOString();

var checkSlotArray = [];
var workingHours = {};

var merchant_timezone = "+07:00";
merchant_timezone = merchant_timezone ? merchant_timezone : "+07:00";

// when document ready
$(document).ready(function () {

    GetWorkingTimeFrame();
    GetWorkingDay();
    showBookingDateTime(currentTime, new Date());
   
    $("#mobile").keypress(function (e) {
        checkPhoneNumber();
        $(this).parents('div.col-xs-12').removeClass('has-error');
    });

    $("#customer_name").keypress(function (e) {
        $(this).parents('div.col-xs-12').removeClass('has-error');
    });


    $("#mobile").keydown(function (e) {
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            // Allow: Ctrl+A, Command+A
            (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
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

    $(document).on('click', function (e) {
        var phone = $('#mobile').val();
        var cus = $("#customer_name").val();
        // you clicked something else
        if (e.target.id != 'mobile') {
            //error phone number
            if (!checkPhoneNumber()) {
                if (phone == "") {
                    $('.general-message').html("<div class='error'>" + "(*) Vui lòng nhập thông tin bắt buộc!" + '</div>').show();
                }
                else if (phone.length < 10) {
                    $('.general-message').html("<div class='error'>" + "(*) Số điện thoại không hợp lệ!" + '</div>').show();
                }
                else {
                    $('.general-message').html("<div class='error'>" + "(*) Đầu số điện thoại di động không hợp lệ!" + '</div>').show();
                }
                $('#mobile').parents('div.col-xs-12').addClass('has-error').removeClass('valid-data'); // ID của trường Số điện thoại
                $('#customer_name').parents('div.col-xs-12').addClass('has-error') // ID của trường Họ và tên
                $('#mobile').focus(); // ID của trường Số điện thoại
                $("#ok").prop("disabled", true);
                e.preventDefault();

            }
                //valid phone number
            else {
                if (e.target.id != 'customer_name' && cus == "") {
                    $('#customer_name').parents('div.col-xs-12').addClass('has-error'); // ID của trường Họ và tên
                    $('#customer_name').focus()
                }
                $('#mobile').parents('div.col-xs-12').removeClass('has-error').addClass('valid-data');
                $('.general-message').hide();
                $("#ok").prop("disabled", false);

            }
        }
    });
    // reset select time frame
    $('#timeFrame .btn-time-selected').removeClass('btn-time-selected').addClass('btn-time-success');

    // today
    //var dateToday = moment(), todayDayInWeek;
    //if (dateToday.day() == 0) {
    //    todayDayInWeek = 'Chủ nhật';
    //} else {
    //    todayDayInWeek = 'Thứ ' + (dateToday.day() + 1);
    //}
    //$('#today .day-in-week').html(todayDayInWeek);
    //$('#today .day').html(dateToday.format('DD/MM'));

    // tomorrow
    //var dateTomorrow = moment().add(1, 'days'), tomorrowDayInWeek;
    //if (dateTomorrow.day() == 0) {
    //    tomorrowDayInWeek = 'Chủ nhật';
    //} else {
    //    tomorrowDayInWeek = 'Thứ ' + (dateTomorrow.day() + 1);
    //}
    //$('#tomorrow .day-in-week').html(tomorrowDayInWeek);
    //$('#tomorrow .day').html(dateTomorrow.format('DD/MM'));

    // afterTomorrow
    //var dateAfterTomorrow = moment().add(2, 'days'), afterTomorrowDayInWeek;
    //if (dateAfterTomorrow.day() == 0) {
    //    afterTomorrowDayInWeek = 'Chủ nhật';
    //} else {
    //    afterTomorrowDayInWeek = 'Thứ ' + (dateAfterTomorrow.day() + 1);
    //}
    //$('#afterTomorrow .day-in-week').html(afterTomorrowDayInWeek);
    //$('#afterTomorrow .day').html(dateAfterTomorrow.format('DD/MM'));

    // when user click on
    $("#timeFrame").on('click', 'button', function (e) {

        //lấy giá trị time từ button hiện tại
        currentTime = $(this).attr('time-frame');

        $("#timeFrame .btn-time-selected .slot").html('Còn chỗ');
        $("#timeFrame .btn-time-selected").removeClass('btn-time-selected').addClass('btn-time-success');
        $(this).removeClass('btn-time-success').addClass('btn-time-selected');
        var currentVal = $(this).attr('time-frame');

        $(this).addClass('btn-time-selected').html("<div class='time'>" + currentVal + '</div><div class="slot">Đã chọn</div>');

        // remove error
        $('.general-message').hide();
        $('#time-select').removeClass('has-error');
    });


    // change date when click change date
    $('#today').click(function (e) {
        $('#groupDay .select-day-active')
        .removeClass('select-day-active')
        .addClass('select-day');
        $('#today').addClass('select-day-active');
        showBookingDateTime(currentTime, new Date());
        //startTime = new Date(new Date().setHours(0, 0, 0, 0)).toISOString();
        GetWorkingTimeFrame();
    });

    $("#tomorrow").click(function (e) {
        $('#groupDay .select-day-active')
        .removeClass('select-day-active')
        .addClass('select-day');
        $('#tomorrow').addClass('select-day-active');

        var tomorrowDate = new Date();
        tomorrowDate.setDate(tomorrowDate.getDate() + 1);
        showBookingDateTime(currentTime, tomorrowDate);
        //tomorrowDate.setHours(0, 0, 0, 0);
        //startTime = tomorrowDate.toISOString();
        GetWorkingTimeFrame();
    });

    $("#afterTomorrow").click(function (e) {
        $('#groupDay .select-day-active')
        .removeClass('select-day-active')
        .addClass('select-day');
        $('#afterTomorrow').addClass('select-day-active');

        var afterTomorrowDate = new Date();
        afterTomorrowDate.setDate(afterTomorrowDate.getDate() + 2);
        showBookingDateTime(currentTime, afterTomorrowDate);
        //afterTomorrowDate.setHours(0, 0, 0, 0);
        //startTime = afterTomorrowDate.toISOString();
        GetWorkingTimeFrame();
    });

    // get current time
    function showBookingDateTime(currentTime, date_selected) {
        var dateObj = new Date(date_selected);

        var getDay = ("0" + dateObj.getDate()).slice(-2);
        var getMonth = ("0" + (dateObj.getMonth() + 1)).slice(-2);
        var getYear = dateObj.getFullYear();
        var currentDay = weekday[dateObj.getDay()];

        $('#timeLabel').html(currentDay + ", " + getDay + "/" + getMonth + "/" + getYear);
        //$('.booking_time').html(currentDay + ", " + getDay + "/" + getMonth + "/" + getYear + " " + currentTime);
    }

    //Lấy khung giờ
    function GetWorkingTimeFrame() {
        var day = $(".select-day-active").data("id");
        $.ajax({
            url: "/Booking/GetWorkingTimeFrame",
            data: {
                BookingDate: day
            },
            type: "GET",
            success: function (data) {
                if (data) {
                    checkSlotArray = data.timeFrame;
                    if (checkSlotArray == undefined) {
                        $("#timeFrame").html('<p style="color: red; text-align: center; margin-top: 20px">' + data + '</p>');
                        $("#ok").prop('disabled', true);
                    }
                    else {
                        renderTimeFrame();
                        $("#ok").prop('disabled', false);
                    }

                }

            },
        });
    }

    //Lấy ngày
    function GetWorkingDay() {
        $.ajax({
            url: "/Booking/GetWorkingDay",
            type: "GET",
            success: function (data) {
                //Today
                $("#today .select-day-title").html(data.Today);
                $("#today .day-in-week").html(data.DayOfWeekToday);
                $("#today .day").html(data.DayWithFormatToday);
                //Tomorrow
                $("#tomorrow .select-day-title").html(data.Tomorrow);
                $("#tomorrow .day-in-week").html(data.DayOfWeekTomorrow);
                $("#tomorrow .day").html(data.DayWithFormatTomorrow);
                //AfterTomorrow
                $("#afterTomorrow .select-day-title").html(data.AfterTomorrow);
                $("#afterTomorrow .day-in-week").html(data.DayOfWeekAfterTomorrow);
                $("#afterTomorrow .day").html(data.DayWithFormatAfterTomorrow);
            },
        });
    }

    // render time table
    function renderTimeFrame() {

        // remove html before render
        $("#timeFrame").empty();

        // render html
        for (var i = 0; i < checkSlotArray.length; i++) {
            tempMoment = checkSlotArray[i];

            var btn = $('<button type="button"></button>');
            //add 1 attribute chứa time frame
            btn.attr('time-frame', tempMoment.TimeFrame);
            btn.addClass('btn time-frame mb-10');

            if (tempMoment.Color == 'RED') {
                btn.html("<div class='time'>" + tempMoment.TimeFrame + '</div><div class="slot">Hết chỗ</div>');
                btn.addClass('disable-click btn-time-danger');

            } else {
                btn.html("<div class='time'>" + tempMoment.TimeFrame + '</div><div class="slot">Còn chỗ</div>');
                btn.addClass('btn-time-success');

            }

            $("#timeFrame").append(btn);
        }
    }

    //Kiểm tra số điện thoại Việt Nam
    function checkPhoneNumber() {
        var flag = false;
        var phone = $("input[name='mobile']").val().trim(); // ID của trường Số điện thoại
        phone = phone.replace('(+84)', '0');
        phone = phone.replace('+84', '0');
        phone = phone.replace('0084', '0');
        phone = phone.replace(/ /g, '');
        if (phone != '') {
            //Đầu 3 số
            var firstNumber1 = phone.substring(0, 3);

            //Đầu 4 số
            var firstNumber2 = phone.substring(0, 4);

            //Xét 10 số
            if (phone.length == 10) {
                //Mobifone 
                if (firstNumber1 == '089' || firstNumber1 == '090' || firstNumber1 == '093' ||
                    //Viettel
                    firstNumber1 == '086' || firstNumber1 == '096' || firstNumber1 == '097' || firstNumber1 == '098' ||
                    //Vinaphone
                    firstNumber1 == '088' || firstNumber1 == '091' || firstNumber1 == '094') {
                    if (phone.match(/^\d{10}/)) {
                        flag = true;
                    }
                }
            }

                //Xét 11 số
            else {
                //Mobifone 
                if (firstNumber2 == '0120' || firstNumber2 == '0121' || firstNumber2 == '0122' || firstNumber2 == '0126' || firstNumber2 == '0128' ||
                    //Viettel
                    firstNumber2 == '0162' || firstNumber2 == '0163' || firstNumber2 == '0164' || firstNumber2 == '0165' ||
                    firstNumber2 == '0166' || firstNumber2 == '0167' || firstNumber2 == '0168' || firstNumber2 == '0169' ||
                    //Vinaphone
                    firstNumber2 == '0123' || firstNumber2 == '0124' || firstNumber2 == '0125' || firstNumber2 == '0127' || firstNumber2 == '0129') {
                    if (phone.match(/^\d{11}/)) {
                        flag = true;
                    }
                }
            }

        }
        return flag;
    }

    //Xử lý button đặt lịch
    $("#ok").click(function (e) {
        //Họ và tên
        var cusName = $("input[name='customer_name']").val();
        //Sđt
        var phoneNum = $("input[name='mobile']").val();
        //Giới tính
        var gender = $("input:checked").val();
        //Giờ đặt
        var bookingTime = $(".btn-time-selected").attr('time-frame');
        //Ngày đặt
        var day = $(".select-day-active").data("id");
        //Dịch vụ KH chọn
        var service = [];
        $(".buttonactive").each(function () {
            service.push(parseInt($(this).data('id')));
        });
        //Ghi chú
        var note = $("#note").val();

        //Lưu thông tin đặt trước của KH
        $.ajax({
            type: "POST",
            url: "/Booking/BookingOrder",
            data: {
                FullName: cusName,
                Phone: phoneNum,
                Gender: gender,
                BookingTime: bookingTime,
                BookingDate: day,
                ServiceNoteList: service,
                Note: note
            },
            success: function (data) {
                //Chưa nhập sđt
                if (data.Message) {
                    Alert_Popup(data.Message);
                }

                    //Chưa chọn khung giờ 
                else if (data.Message1) {
                    $('.bookingtime-message').html("<div class='error'>" + data.Message1 + '</div>').show();
                }
                    //Số điện thoại này đã được đặt trong ngày
                else if (data.Message2) {
                    Alert_Popup(data.Message2);
                }
                    //Thành công
                else if (data) {
                    //loading button
                    var $this = $("#ok");
                    $this.button('loading');
                    setTimeout(function () {
                        $this.button('reset');
                    }, 8000);
                    window.location.href = "/Booking/OrderSuccess";
                }

            },
        });
    });

    //Thông báo
    function Alert_Popup(Centent_Alert) {
        $.ajax({
            type: "POST",
            url: "/Booking/Alert",
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

    //Multiple active button
    $('.button').click(function () {
        //$('.button').not(this).removeClass('buttonactive'); // remove buttonactive from the others
        $(this).toggleClass('buttonactive') // toggle current clicked element
        if ($(this).hasClass('buttonactive')) {
            $(".buttonactive div").removeClass('sNote');
        }
        else {
            $(".button").not(".buttonactive").children("div").addClass('sNote');
        }
    });
});