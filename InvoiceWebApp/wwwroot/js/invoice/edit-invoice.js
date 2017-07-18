//Variables
var firstRun = true;
var count = 0;
var productArray = new Array();
var amountArray = new Array();

//-------------------------------------------------------------------------------------------
//Hide product-control row
$("#product-control").hide();

//-------------------------------------------------------------------------------------------
//Run when page has loaded
$(document).ready(() => {
    $("#products select").material_select();
    $("#debtor-row #select_debtor").material_select();
    $("#company-row #select_company").material_select();
    $("#type-row #select_type").material_select();

    //Set value of the total amount in the input box
    $("#total").val(totalAmount);

    //Set product rows is this is the first run
    if (firstRun == true) {
        setRows();
    }
});

//-------------------------------------------------------------------------------------------
// Automatically set expiration date based on selected invoice date
$("#icon_created").on("change", () => {
    var selectedDate = new Date($("#icon_created").val());

    var day = selectedDate.getDate() + 30;
    var month = selectedDate.getMonth() + 1;
    var year = selectedDate.getFullYear();

    if (month < 10) month = "0" + month;
    if (day < 10) day = "0" + day;

    var today = year + "-" + month + "-" + day;
    $("#icon_expired").val(today);
});

//-------------------------------------------------------------------------------------------
//Calculate the total amount when adding a new product
$("#products option:selected").each(() => {
    $("#product").on('change', () => {
        calcTotal();
    });
})

$(document).on("change", "#products option:selected", () => {
    calcTotal();
});

//Calculate the total amount when changing the amount of product(s)
$("#products #_amount").on('change', () => {
    calcTotal();
});

$(document).on("change", "#products #_amount", () => {
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

$(document).on("change", "#icon_discount", () => {
    calcTotal();
});

//-------------------------------------------------------------------------------------------
//Add parameters to form and submit
$("#edit-invoice-btn").on("click", () => {
    $("#products option:selected").each(function () {
        productArray.push($(this).val().split('_')[0]);
    });

    $("#products [id^=_product-]").each(function () {
        var text = $(this).val();
        var id = $(this).attr("id");
        var pid = id.split("x")[1];

        productArray.push(pid);
    });

    $("#products #_amount").each(function () {
        amountArray.push($(this).val());
    });

    productArray.splice(0, 1);
    amountArray.splice(0, 1);

    var totalPrice = $("#total").val();

    $("#form").append('<input type="hidden" name="total" value="' + totalPrice + '" /> ');
    $("#form").append('<input type="hidden" name="pids" value="' + productArray.toString() + '" /> ');
    $("#form").append('<input type="hidden" name="amounts" value="' + amountArray.toString() + '" /> ');

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
$(document).on("click", "#delete-row-btn", function () {
    $(this).parent().parent().remove();
    calcTotal();

    return false;
})

//-------------------------------------------------------------------------------------------
//Calculate total amount based on selected products and their quantities
function calcTotal() {
    var amounts = new Array();
    var pids = new Array();
    var discount = 0;
    var totalPrice = Number.parseFloat("0");
    var continueCalc = false;

    $("#products option:selected").each(function () {
        var id = $(this).val();
        pids.push($(this).val());
    });

    $("#products [id^=_product-]").each(function () {
        var text = $(this).val();
        var id = $(this).attr("id");
        var pid = id.split("x")[1];

        //pids.push(pid);

        if (pid != "-1") {
            pids.push(pid);
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

    pids.splice(0, 1);
    amounts.splice(0, 1);

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