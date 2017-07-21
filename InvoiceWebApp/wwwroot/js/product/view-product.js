//Run when page has loaded
$(document).ready(function () {
    $("#category_select").material_select();

    //Set value of price input
    setPrice();

    //Set value of category input
    setDropDown();
});

function setDropDown() {
    if (category != "") {
        $(".select-wrapper input").val(category);
        $(".select-wrapper ul").find('li span:contains("' + category + '")').parent().addClass("active selected");
    }

    $("#details-product > div:nth-child(3) > div > form > fieldset > div:nth-child(6) > div > div > input").prop("disabled", true);
    $("#remove-product > div:nth-child(3) > div > form > fieldset > div:nth-child(6) > div > div > input").prop("disabled", true);

    $("#category_select").prop("disabled", true);
}

function setPrice() {
    $("#icon_price").val(price);
}