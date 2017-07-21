//Run when page has loaded
$(document).ready(function () {
    $("#category_select").material_select();

    //Set value of price input
    setPrice();

    //Set value of category
    setDropDown();
});

$("#category_select").on("change", () => {
    var cat = $("#category_select").val();
    console.log("Selected Category: " + cat);
});

//Add parameters to form
$("#edit-product-btn").on("click", function () {
    var price = $("#icon_price").val();
    var category = $("#category_select").val();

    console.log("JS Price: " + price);
    console.log("JS Category: " + category);

    $("#form").append('<input type="hidden" name="price" value="' + price.toString() + '" /> ');
    $("#form").append('<input type="hidden" name="category" value="' + category.toString() + '" /> ');
})

function setDropDown() {
    if (category != "") {
        $(".select-wrapper input").prop("value", category);
        $(".select-wrapper ul").find('li span:contains("' + category + '")').parent().addClass("active selected");
    }
}

function setPrice() {
    $("#icon_price").val(price);
}