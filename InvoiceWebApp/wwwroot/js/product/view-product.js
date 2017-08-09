//Run when page has loaded
$(document).ready(function () {
    $("#category_select").material_select();

    //Set value of price input
    setPrice();

    //Set value of category input
	setCategory();

	//Reinitialize all labels
	Materialize.updateTextFields();
});

function setCategory() {
	$("#category_select").material_select();
	$("#category_select").find('option:contains("' + category + '")').prop('selected', true);
	$("#category_select").material_select();

    $("#details-product > div:nth-child(3) > div > form > fieldset > div:nth-child(6) > div > div > input").prop("disabled", true);
    $("#remove-product > div:nth-child(3) > div > form > fieldset > div:nth-child(6) > div > div > input").prop("disabled", true);

    $("#category_select").prop("disabled", true);
}

function setPrice() {
    $("#icon_price").val(price);
}