
/**
 * Navbar mobile toggle when :
 * - Click on burger
 * - Click next to mobile menu
 * - Resize page over 990px
 * 
 * Search navbar button
 * - Display searching form
 * - Hide main content
 */

$(document).ready(() => {
    $("#jds-burger").click(() => {
        toggleMobileMenu();
    });

    $("#jds-close-nav").click(() => {
        toggleMobileMenu();
    });

    $("#jds-navbar-search-btn").click(() => {
        toggleSearchNavbar();
    });

    $("#jds-search-close-btn").click(() => {
        toggleSearchNavbar();
    });

    $(window).resize(() => {
        if ($("#jds-mobile-nav").hasClass("jds-mobile-nav-opened") && $(document).width() >= 990) {
            toggleMobileMenu();
        }

        if ($("#jds-main-content").hasClass("jds-navbar-search-opened") && $(document).width() < 990) {
            toggleSearchNavbar();
        }
    });

    function toggleMobileMenu() {
        burgerAnimation();
        $("#jds-mobile-nav").toggleClass("jds-mobile-nav-opened");
        $("#jds-close-nav").toggleClass("jds-close-nav-opened");
    }

    function toggleSearchNavbar() {
        $("#jds-navbar-search").toggleClass("jds-navbar-search-opened");
        $("#search_input").focus();

        if ($("#jds-navbar-search").hasClass("jds-navbar-search-opened")) {
            setTimeout(() => {
                $("#jds-main-content").addClass("jds-navbar-search-opened");

                if (!$("#jds-navbar-search").hasClass("jds-navbar-search-opened")) {
                    $("#jds-main-content").removeClass("jds-navbar-search-opened");
                }
            }, 500);

            $("#jds-navbar-search-btn").html("<i class=\"fa-solid fa-xmark\"></i>");
        } else {
            $("#jds-main-content").removeClass("jds-navbar-search-opened");
            $("#jds-navbar-search-btn").html("<i class=\"fa-solid fa-magnifying-glass\"></i>");
        }
    }
});

/**
 * Burger animation
 */

let mobileMenuOpen = false;
const menuBurger = document.querySelector(".jds-menu-btn");

function burgerAnimation() {
    if (!mobileMenuOpen) {
        menuBurger.classList.add("open");
        mobileMenuOpen = true;
    } else {
        menuBurger.classList.remove("open");
        mobileMenuOpen = false;
    }
}