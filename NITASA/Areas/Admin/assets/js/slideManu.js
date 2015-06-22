
$(document).ready(function () {
    $(".toggle-primary-sidebar").click(function () {
        if ($(this).hasClass("open")) {
            $("#page").animate({ left: '0' });
            $(".primary-sidebar").animate({ left: '-200px' });
            $(".primary-sidebar").css("z-index", "-1");
            $(this).removeClass("open");
        }
        else {
            $("#page").animate({ left: '200px' });
            $(".primary-sidebar").animate({ left: '0px' });
            $(".primary-sidebar").css("z-index", "1");
            $(this).addClass("open");
            var window_height = $("body").height();
            $(".primary-sidebar").css("min-height", +window_height + "px");
        }
    });
    var widnow_width = $(window).width();
    if (widnow_width > 768) {
        $("#page").animate({ left: '0' });
        $("#page").animate({ right: '0' });
        $(".primary-sidebar").css("z-index", "-1");
    }

});
$(window).resize(function () {
    var widnow_width = $(window).width();
    if (widnow_width > 768) {
        $("#page").animate({ left: '0' });
        $("#page").animate({ right: '0' });
        $(".primary-sidebar").css("z-index", "-1");
        $(".primary-sidebar").css("min-height", "0px");
        $(".primary-sidebar").css("min-height", "0px");
    }
});