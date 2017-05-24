/// <reference path="../lib/jquery/dist/jquery.js" />
/// <reference path="../lib/jquery/dist/jquery.min.js" />
/// <reference path="../lib/jquery/dist/jquery.slim.js" />
/// <reference path="../lib/jquery/dist/jquery.slim.min.js" />
//site.js

(function () {

    var $sidebarAndWrapper = $("#sidebar, #wrapper");
    var $icon = $("#sidebarToggle i.fa");

    $("#sidebarToggle").on("click", function () {
        $sidebarAndWrapper.toggleClass("hide-sidebar");
        if ($sidebarAndWrapper.hasClass("hide-sidebar")) {
            $icon.removeClass("fa-angle-left");
            $icon.addClass("fa-angle-right");
        } else {
            $icon.removeClass("fa-angle-right");
            $icon.addClass("fa-angle-left");
        }
    });
})();
