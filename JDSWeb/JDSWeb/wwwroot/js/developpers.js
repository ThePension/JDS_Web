
$(document).ready(() => {
    var cards = $('.dev-card');

    cards.click(function () {
        clickedCard = $(this);
        $(".dev-card").removeClass("collapsed-active");   
        clickedCard.toggleClass("collapsed-active");

        $(".dev-info-collapsed").each(function (index, elem) {
            elem = $(elem);
            elem.removeClass("dev-info-collasped-active")
            if (clickedCard.data("dev_name") == elem.data("dev_name"))
            {
                elem.addClass("dev-info-collasped-active");
            }
        });
    });
});