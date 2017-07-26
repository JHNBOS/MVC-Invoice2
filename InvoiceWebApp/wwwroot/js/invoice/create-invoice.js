//Variables
var total = "";
var count = 1;
var productArray = new Array();
var amountArray = new Array();
var products = [];

//Run when page has loaded
$(document).ready(function () {

    //Initialize Chosen JS dropdowns
    $("#debtor-row #select_debtor").chosen({
        width: "100%",
        search_contains: true,
        allow_single_deselect: true,
        no_results_text: "No debtor matches: "
    });
    $("#debtor-row #select_company").chosen({
        width: "100%",
        search_contains: true,
        allow_single_deselect: true,
        no_results_text: "No company matches: "
    });

    //Initialize Materialize CSS dropdowns
    $("#products #product-control select").material_select();
    $("#type-row #select_type").material_select();

    //Hide delete button product-control
    $("#products #product-control #delete-row-btn").hide();

    //Set value of total input
    $("#total").val("0,00");
});

//-------------------------------------------------------------------------------------------
//Disable the company dropdownlist when this dropdownlist has an option selected
$("#debtor-row #select_debtor").change(function (e, params) {
    var selectedOption = "";

    if (e.target.value != "" || e.target.value != null) {
        var selectedOption = $("#debtor-row #select_debtor").find("option[value='" + e.target.value + "']").text();
    }
    if (selectedOption != "Choose debtor") {
        $("#debtor-row #select_company").prop("disabled", true);
        $("#debtor-row #select_company").trigger("chosen:updated");
    } else {
        $("#debtor-row #select_company").prop("disabled", false);
        $("#debtor-row #select_company").trigger("chosen:updated");
    }
});

//Disable the debtor dropdownlist when this dropdownlist has an option selected
$("#debtor-row #select_company").change(function (e, params) {
    var selectedOption = "";

    if (e.target.value != "" || e.target.value != null) {
        var selectedOption = $("#debtor-row #select_company").find("option[value='" + e.target.value + "']").text();
    }
    if (selectedOption != "Choose company") {
        $("#debtor-row #select_debtor").prop("disabled", true);
        $("#debtor-row #select_debtor").trigger("chosen:updated");
    } else {
        $("#debtor-row #select_debtor").prop("disabled", false);
        $("#debtor-row #select_debtor").trigger("chosen:updated");
    }
});

//-------------------------------------------------------------------------------------------
// Automatically set expiration date based on selected invoice date
$("#icon_created").on("change", () => {
    var selectedDate = new Date($("#icon_created").val());

    selectedDate.setDate(selectedDate.getDate() + 30);
    selectedDate.setMonth(selectedDate.getMonth() + 1);

    var day = selectedDate.getDate();
    var month = selectedDate.getMonth();
    var year = selectedDate.getFullYear();

    if (month < 10) month = "0" + month;
    if (day < 10) day = "0" + day;

    var today = year + "-" + month + "-" + day;

    $("#icon_expired").val(today);
});

//-------------------------------------------------------------------------------------------
//Calculate the total amount when adding a new product
$("#products div div:nth-child(2) #_product").on("change", function () {
    $("#products div div:nth-child(2) #_product option:selected").each(function () {
        calcTotal();
    })
});

//-------------------------------------------------------------------------------------------
//Calculate the total amount when changing the amount of product(s)
$("#products #_amount").on("change", function () {
    calcTotal();
});
$("#products #_amount").on("keyup", function () {
    calcTotal();
});
$("#products #_amount").mouseup(function () {
    calcTotal();
});
$("#products #_amount").mousedown(function () {
    calcTotal();
});

//-------------------------------------------------------------------------------------------
//Calculate the total amount when changing the amount of discount
$("#icon_discount").on("change", function () {
    calcTotal();
});
$("#icon_discount").mouseup(function () {
    calcTotal();
});
$("#icon_discount").mousedown(function () {
    calcTotal();
});

//-------------------------------------------------------------------------------------------
//Add parameters to form and submit
$("#create-invoice-btn").on("click", function () {
    $("#products div div:nth-child(2) #_product option:selected").each(function () {
        productArray.push($(this).val().split('_')[0]);
    })

    $("#products #_amount").each(function () {
        amountArray.push($(this).val());
    })

    var totalPrice = $("#total").val();

    $("#form").attr("action", "Create/?pids=" + productArray.toString() + "&amounts=" + amountArray.toString() + "&total=" + totalPrice.toString());
    $("#form").submit();
});

//-------------------------------------------------------------------------------------------
//Add a new product row
$("#add-row-btn").on("click", function () {
    count++;

    var copy = $("#product-control")
        .clone(true)
        .appendTo("#products")
        .prop("id", "product-row");

    //Empty inputs
    $("#products #product-row:nth-child(" + count + ") #_product").empty();
    $("#products #product-row:nth-child(" + count + ") #_amount").prop("value", "");

    //Append option
    $("#products #product-row:nth-child(" + count + ") #_product").append("<option value=''>Select a product</option>");

    //Show delete button
    $("#products #product-row:nth-child(" + count + ") #delete-row-btn").show();

    //Initialize Materialize CSS dropdowns
    $("#products #product-row:nth-child(" + count + ") select").material_select();
    $("#products #product-control select").material_select();

    return false;
});

//-------------------------------------------------------------------------------------------
//Remove a product row
$("#delete-row-btn").on("click", function () {
    $(this).closest(".row").remove();

    count--;
    calcTotal();

    return false;
});

//-------------------------------------------------------------------------------------------
//Calculate total amount based on selected products and their quantities
function calcTotal() {
    var amounts = new Array();
    var pids = new Array();
    var discount = 0;
    var totalPrice = Number.parseFloat("0");
    var continueCalc = false;

    $("#products div div:nth-child(2) #_product option:selected").each(function () {
        if ($(this).val() != "") {
            pids.push($(this).val());
            continueCalc = true;
        }
    });

    $("#products #_amount").each(function () {
        var amount = 0;

        if ($(this).val() != "") {
            amount = $(this).val();
        } else {
            amount = 0;
        }

        amounts.push(amount);
    });

    $("#icon_discount").each(function () {
        if ($(this).val() != "") {
            discount = $("#icon_discount").val();
        }
    });

    if (amounts.length != pids.length) {
        $("#total").prop("readonly", false);
        $("#total").val("0,00");
        $("#total").prop("readonly", true);
    }

    if (continueCalc == true) {
        for (var i = 0; i < pids.length; i++) {
            var id = pids[i].split('_')[0];
            var p = pids[i].split('_')[1];
            var price = Number.parseFloat(p).toFixed(2);
            var amount = Number.parseInt(amounts[i]);

            var discountPercentage = 100 - discount;
            var totalBeforeDiscount = (price * amount);
            var totalAfterDiscount = (totalBeforeDiscount * discountPercentage) / 100;

            totalPrice += totalAfterDiscount;
        }

        $("#total").prop("readonly", false);
        $("#total").val(totalPrice.toLocaleString("nl-NL", { minimumFractionDigits: 2 }));

        total = totalPrice.toLocaleString("nl-NL", { minimumFractionDigits: 2 });
        $("#total").prop("readonly", true);
    }
    if (continueCalc == false && pids.length == 0 && ($("#total").val() != 0 || $("#total").val() != "0")) {
        $("#total").prop("readonly", false);
        $("#total").val("0,00");
        $("#total").prop("readonly", true);
    }
}

//-------------------------------------------------------------------------------------------
//Call for cascading dropdown
$("#_category").change(function () {
    //Empty product select
    var productSelect = $(this).closest(".row").find("#_product");

    productSelect.empty();

    //Make ajax call
    $.ajax({
        type: "POST",
        url: ajaxURL,
        dataType: "json",
        data: { id: $(this).val() },
        success: function (items) {
            $.each(items, function (i, item) {
                $(productSelect)
                    .append('<option value="' + item.value + '">' + item.text + '</option>');
            });
            $(productSelect).material_select();
        },
        error: function (ex) {
            alert('Failed to retrieve products.' + ex);
        }
    });

    return false;
});