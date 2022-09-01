// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function isTouchDevice() {
    return ('ontouchstart' in window);
}

$(document).ready(() => {
    if (isTouchDevice()) {
        $(".jds-mobile-hover").each(function (index, obj) {
            $(obj).addClass('jds-hover');
        });
    }

    $("#cookie_toast").on('hide.bs.toast', () => {
        document.cookie = "c_Cookie.Accepted=true;path=/";
    });
});