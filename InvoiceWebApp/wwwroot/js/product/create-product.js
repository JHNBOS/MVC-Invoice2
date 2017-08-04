//Run when page has loaded
$(document).ready(function () {
    $("#category_select").material_select();
});

//Add parameters to form
$("#create-product-btn").on("click", function () {
    var price = $("#icon_price").val();
    var category = $("#category_select").val();

    console.log("JS Price: " + price);
    console.log("JS Category: " + category);

	$("#form").attr("action", "Create/?price=" + price.toString() + "&category=" + category.toString());
	$("#form").submit();
});