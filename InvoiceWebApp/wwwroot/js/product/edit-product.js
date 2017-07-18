//Run when page has loaded
$(document).ready(function () {
    //Set value of price input
    $("#price").val(price);

    //Set value of vat input
    $("#vat").val(vat);
});

//Add parameters to form
$("edit-product-btn").on("click", function () {
    var price = $("#price").val();
    $("#form").attr("asp-route-price", price);
})