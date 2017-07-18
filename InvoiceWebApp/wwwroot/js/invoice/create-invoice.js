//Variables
var total = "";
var count = 0;
var productArray = new Array();
var amountArray = new Array();
var products = [];

//Run when page has loaded
$(document).ready(function () {
    $("#products #product-control #_product").material_select();
    $("#debtor-row #select_debtor").material_select();
    $("#debtor-row #select_company").material_select();
    $("#type-row #select_type").material_select();
});

//-------------------------------------------------------------------------------------------
//Disable the company dropdownlist when this dropdownlist has an option selected
$("#debtor-row #select_debtor").change(function () {
    var optionSelected = $(this).find("option:selected");
    var valueSelected = optionSelected.val();
    var textSelected = optionSelected.text();

    if (textSelected != "Select Debtor") {
        $("#debtor-row > div:nth-child(2) > div > input").prop("disabled", true);
    }
    if (textSelected == "Select Debtor") {
        $("#debtor-row > div:nth-child(2) > div > input").prop("disabled", false);
    }
});

//Disable the debtor dropdownlist when this dropdownlist has an option selected
$("#debtor-row #select_company").change(function () {
    var optionSelected = $(this).find("option:selected");
    var valueSelected = optionSelected.val();
    var textSelected = optionSelected.text();

    if (textSelected != "Select Company") {
        $("#debtor-row > div:nth-child(1) > div > input").prop("disabled", true);
    }
    if (textSelected == "Select Company") {
        $("#debtor-row > div:nth-child(1) > div > input").prop("disabled", false);
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
$("#products option:selected").each(function () {
    $("#_product").on('change', function () {
        console.log("Product: " + $(this).val());
        calcTotal();
    });
})

//Calculate the total amount when changing the amount of product(s)
$("#products #_amount").on('change', function () {
    calcTotal();
});

//Calculate the total amount when changing the amount of discount
$("#icon_discount").on('change', function () {
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
    $("#products option:selected").each(function () {
        productArray.push($(this).val().split('_')[0]);
    })

    $("#products #_amount").each(function () {
        amountArray.push($(this).val());
    })

    var totalPrice = $("#total").val();
    var companyID = $().val();

    $("#form").attr("action", "Create/?pids=" + productArray.toString() + "&amounts=" + amountArray.toString() + "&total=" + totalPrice.toString());
    $("#form").submit();
});

//-------------------------------------------------------------------------------------------
//Add a new product row
$("#add-row-btn").on("click", function () {
    $("#product-control").show();
    var copy = $("#product-control")
        .clone(true)
        .appendTo("#products")
        .prop("id", "product-row")
        .find("input").val("");

    $("#product-control").hide();
    $("#products #product-row select").material_select();

    return false;
});

//-------------------------------------------------------------------------------------------
//Remove a product row
$("#delete-row-btn").on("click", function () {
    $(this).parent().parent().remove();
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

    $("#products option:selected").each(function () {
        if ($(this).val() != "-1") {
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
        $("#total").val("");
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
}