
/**
 * Navbar mobile toggle when :
 * - Click on burger
 * - Click next to mobile menu
 * - Resize page over 990px
 */

$(document).ready(() => {
    $("#jds-burger").click(() => {
        toggleMobileMenu();
    });

    $("#jds-close-nav").click(() => {
        toggleMobileMenu();
    });

    $(window).resize(() => {
        if ($("#jds-mobile-nav").hasClass("jds-mobile-nav-opened") && $(document).width() >= 990) {
            toggleMobileMenu();
        }
    });

    function toggleMobileMenu() {
        $("#jds-mobile-nav").toggleClass("jds-mobile-nav-opened");
        $("#jds-close-nav").toggleClass("jds-close-nav-opened");
    }
});