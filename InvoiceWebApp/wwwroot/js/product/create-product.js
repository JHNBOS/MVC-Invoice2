//Add parameters to form
$("#create-product-btn").on("click", function () {
    var price = $("#price").val();
    $("#form").attr("asp-route-price", price);
})