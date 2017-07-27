//Variables
var firstRun = true;

//--------------------------------- DOCUMENT READY
$(document).ready(function () {
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

//--------------------------------- SET PRODUCT ROWS
function setRows() {
    for (var i = 0; i < amounts.length; i++) {
        var amount = amounts[i];

        var html = `
                    <div id="product-row-` + i + `" class="row">
                        <div class="input-field col s12 col m8 col l8">
                            <input id="_product-` + i + `" disabled/>
                        </div>

                        <div class="input-field col s8 col m2 col l2">
                            <input id="_amount" placeholder="Qnt." value="` + amount + `" disabled />
                        </div>
                    </div>
                `;

        $("#products").append(html);
    }

    for (var i = 0; i < amounts.length; i++) {
        var pid = pids[i];
        var pname = pnames[i];

        var element = "#products #product-row-" + i.toString() + " #_product-" + i.toString();

        $(element).prop("value", pname);
        $(element).prop("disabled", true);

        //Reset variables
        pid = "";
        pname = "";
    }
}