// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.


function randomSeed() {
    var seed = Math.floor(Math.random() * 100000);
    document.getElementById("seed").value = seed;
    updateResultSeedChange(seed);
}

function updateTextInput(val) {
    document.getElementById('error').value = val;

}

function updateSliderInput(val) {
    document.getElementById("range").value = val;
}

function updateResultCountryChange(val) {
    var selectedCountry = val;
    updateTableData();
}

function updateResultErrorChange(val) {
        var error = val;
        updateSliderInput(error);
        updateTableData(); 
}

function updateResultSliderErrorChange(val) {
        var error = val;
        updateTextInput(error);
        updateTableData();
}

function updateResultSeedChange(val) {
    var seed = val;
    updateTableData();
}


function updateTableData() {
    var { selectedCountryValue, errorValue, seedValue, lengthCurrent } = fetchOptions();


    var lengthCurrent = 0;

    //console.log("Update Table! Data length: " + lengthCurrent  + " Region: " + selectedCountryValue + " Error: " + errorValue + " Seed: " + seedValue);

    var fakerOptions =
    {
        length: lengthCurrent,
        selectedCountry: selectedCountryValue,
        error: errorValue,
        seed: seedValue
    };

    //reload data, append only 20 entries
        $.ajax({
            type: "POST",
            url: "api/generator/initial",
            data: JSON.stringify(fakerOptions),
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                $("#datas").empty();
                response.map((user, index) => {
                    appendUser(user, index + 1);
                })
                //console.log(response);
            }
        });
}

function appendTableAdditional() {
    var { selectedCountryValue, errorValue, seedValue, lengthCurrent } = fetchOptions();

    var fakerOptions =
    {
        length: lengthCurrent,
        selectedCountry: selectedCountryValue,
        error: errorValue,
        seed: seedValue
    };

    $.ajax({
        type: "POST",
        url: "api/generator/additional",
        data: JSON.stringify(fakerOptions),
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            response.map((user, index) => {
                appendUser(user, lengthCurrent + index + 1);
            })
        }
    });
}


function fetchOptions() {
    var seedValue = document.getElementById("seed").value;
    var errorValue = document.getElementById("error").value;
    var selectedCountryValue = document.getElementById("country").value;
    var lengthCurrent = document.getElementById("tableData").rows.length;

    if (seedValue == null) {
        seedValue = 0;
    }
    else if (errorValue == null) {
        console.log("error nule");
        errorValue = 0;
    }
    return { selectedCountryValue, errorValue, seedValue, lengthCurrent };
}

function appendUser(user, i) {
    $("#datas").append(
        "<tr>" +
        "<td>" + i + "</td>" +
        "<td>" + user.id + "</td>" +
        "<td>" + user.name + "</td>" +
        "<td>" + user.address + "</td>" +
        "<td>" + user.phone + "</td>" +
        "</tr>"
    )
}


function download() {
    const table2Excel = new Table2Excel("#tableData");
    table2Excel.export('generated-data', 'xlsx');
}



var timeout;
jQuery(function ($) {
    $("#data").on('scroll', function () {
        var scrollTop = $(this).scrollTop();
        clearTimeout(timeout);
        var innerHeight = $(this).innerHeight();
        var scrollHeight = $(this)[0].scrollHeight;
        timeout = setTimeout(function () {
            if (scrollTop + innerHeight >= scrollHeight) {
                appendTableAdditional();
            }
            }, 500);
    });
});