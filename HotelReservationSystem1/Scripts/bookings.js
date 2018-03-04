
$(function () {
    var dtToday = new Date();

    var month = dtToday.getMonth() + 1;
    var day = dtToday.getDate();
    var year = dtToday.getFullYear();

    if (month < 10)
        month = '0' + month.toString();
    if (day < 10)
        day = '0' + day.toString();

    var minCheckInDate = year + '-' + month + '-' + day;

    $('#CheckInDate').attr('min', minCheckInDate);
    $('#CheckOutDate').attr('min', minCheckInDate);
});

$(function () {
    $('#RoomId').on('change', function () {
        var selectedIndex = $('#RoomId :selected').index();
        var rate = $('#RoomId2 option').eq(selectedIndex).text();

        if (parseFloat(rate)) {
            rate = 'Rate(per night): $' + rate;
            $('#roomRate').text(rate);
        }
        else {
            $('#roomRate').text('');
        }
    });
});
