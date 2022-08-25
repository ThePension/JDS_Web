// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const menuHamburger = document.querySelector(".jds-burger");
const navMobileMenu = document.querySelector(".jds-mobile-nav");
const test = document.querySelector(".jds-close-nav");

menuHamburger.addEventListener("click", () => {
    navMobileMenu.classList.toggle("jds-mobile-nav-opened");
    test.classList.toggle("jds-close-nav-opened");
})

// TODO : Refaire au propre + listener sur redimensionnement page
//        (étirement, le menu doit disparaître)