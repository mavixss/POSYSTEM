

$(function () {
    var numCode;
    ///
    $.post("../home/getItemCode", {
        //this is part when you are going to send data to the server.
    }, function (res) {
        var num = parseInt(res[0].itmcode) + 1;
        numCode = $("#txtCode").val("I000000000" + num);
        $("#txtCode").prop('disabled', true);
    });
    ///
    $("#txtCode").val(numCode);
    $("#productRecord").submit(function () {
        $("#txtCode").prop('disabled', false);
    });


});


