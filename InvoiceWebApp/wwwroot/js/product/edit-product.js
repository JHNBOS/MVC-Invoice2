//Run when page has loaded
$(document).ready(function () {
    //Set value of price input
    setPrice();

    //Set value of category
	setCategory();
});

$("#category_select").on("change", function () {
	console.log("I am selected!");
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

function setCategory() {
	$("#category_select").material_select();
	$("#category_select").find('option:contains("' + category + '")').prop('selected', true);
	$("#category_select").material_select();
}

function setPrice() {
    $("#icon_price").val(price);
}