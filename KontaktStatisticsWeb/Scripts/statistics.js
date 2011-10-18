/// <reference path="jquery-1.6.4-vsdoc.js" />

$(document).ready(function () {
    $("#ddStatistic").change(function () {
        var value = parseInt($(this).val());
        if (value > 0) {
            var test = 0;
        }
    });
});