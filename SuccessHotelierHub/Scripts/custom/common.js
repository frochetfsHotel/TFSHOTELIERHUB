﻿$(function () {

    LoadLeftSideMenu();

    $('.mydatepicker').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy'
    })
    .change(changeColorOfDatePickerLabel)
    .on('changeDate', changeColorOfDatePickerLabel);;
});

var RootPath = ""; //Store Site Root Location.

var StatusCode = {
    SUCCESS: 200,
    FAILURE: 400
};

var Alert = {
    SUCCESS: "alert-success",
    ERROR: "alert-danger",
    INFO: "alert-info",
    WARNING: "alert-warning"
};

var ToasterType = {
    SUCCESS: "success",
    ERROR: "error",
    INFO: "info",
    WARNING: "warning"
};

var ToasterTitle = {
    SUCCESS: "Success!",
    ERROR: "Error!",
    INFO: "Info!",
    WARNING: "Warning!"
};

var Delimeter = {
    BREAKLINE: "<br/>",
    NEWLINE: "\n",
    COMMA: ",",
    SEMICOLON: ";",
    PIPE: "|",
    DOUBLEPIPE: "||",
    DOT: ".",
    HASH: "#",
    SPACE: " "
};

var DateFormat = {
    DDMMYYYY: "dd/MM/yyyy",
    MMDDYYYY: "MM/dd/yyyy",
    YYYYMMDD: "yyyy/MM/dd"
};

var DateSeprator = {
    SLASH: "/",
    DASH: "-",
    DOT: "."
}

function loadToastrSetting() {
    toastr.options = {
        closeButton: true,
        progressBar: false,
        preventDuplicates: true,
        showMethod: 'slideDown',
        timeOut: 4000
    };

}

function showToaster(message, toastrType) {
    loadToastrSetting();

    if (toastrType == ToasterType.SUCCESS) {
        toastr.success(ToasterTitle.SUCCESS, message);
    }
    else if (toastrType == ToasterType.ERROR) {
        toastr.error(ToasterTitle.ERROR, message);
    }
    else if (toastrType == ToasterType.INFO) {
        toastr.info(ToasterTitle.INFO, message);
    }
    else if (toastrType == ToasterType.WARNING) {
        toastr.warnnig(ToasterTitle.WARNING, message);
    }
}

function showAlert(message, className) {
    $("#spMessage").html(message);
    var alertClass = "alert alert-dismissable " + className;
    $('#dvAlert').show().fadeIn();
    $("#dvAlert").removeClass();
    $('#dvAlert').addClass(alertClass);
    $("html, body").animate({ scrollTop: 30 }, 500);
}

function hideAlert() {
    setTimeout(function () { $('#dvAlert').fadeOut('slow') }, 5000);
}

function closeAlert() {
    $('.alert').fadeOut('slow');
}

// Fuction for scroll body at given element
function scrollToControl(control) {    
    $('html,body').animate({ scrollTop: $('#' + control).offset().top }, 500);
}


function IsNullOrEmpty(item) {
    if (item == null || item == undefined || item == "") {
        return true;
    }
    return false;
}

function showLoader() {
    $('#divLoader').show();
}

function hideLoader() {
    $('#divLoader').hide();
}

function validateEmail(email) {
    var pattern = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    if (pattern.test(email)) {
        return true;
    }
    return false;
}


function redirectTo(url) {
    if (!IsNullOrEmpty(url)) {
        window.location.href = (RootPath + url);
    }
}


function onlyDigits(e) {
    var key = e.keyCode;

    //key = 9; TAB Key. (Allow Tab Key)

    if (!((key == 9) || (key == 8) || (key == 46) || (key >= 35 && key <= 40) || (key >= 48 && key <= 57) || (key >= 96 && key <= 105))) {
        e.preventDefault();
    }  
}

function onlyNumeric(event) {
    if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
}


/*** Date Utility Functions START ****/
function GetDate(objDate, dateFormat) {
    var d = objDate;
    if (IsNullOrEmpty(d) || d == "Invalid Date") {
        return "";
    }

    var month, date, year;
    var MM, dd, yyyy;
    var fullDate = "";

    date = d.getDate();
    month = (d.getMonth() + 1); // : Month start with (0-11) e.g. Jan = 0, Feb = 1, Mar = 2 etc.
    year = d.getFullYear();

    //set initial value.
    yyyy = year;
    MM = month;
    dd = date;

    //Append '0' prefix in date.
    if (date >= 1 && date <= 9) {
        dd = ("0" + date);
    }

    //Append '0' prefix in month.
    if (month >= 1 && month <= 9) {
        MM = ("0" + month);
    }

    if (dateFormat == DateFormat.DDMMYYYY) {
        fullDate = dd + "/" + MM + "/" + yyyy;
    }
    else if (dateFormat == DateFormat.MMDDYYYY) {
        fullDate = MM + "/" + dd + "/" + yyyy;
    }
    else if (dateFormat == DateFormat.YYYYMMDD) {
        fullDate = yyyy + "/" + MM + "/" + dd;
    }
    return fullDate;
}

function ConvertDDMMYYYY_To_MMDDYYYY(inputDate, dateSeparator) {
    if (IsNullOrEmpty(inputDate)) { return ""; }
    if (IsNullOrEmpty(dateSeparator)) { dateSeparator = DateSeprator.SLASH; }


    var dateParts = inputDate.split(dateSeparator)

    var MM, dd, yyyy;

    dd = dateParts[0];
    MM = dateParts[1];
    yyyy = dateParts[2];

    return trim(MM + dateSeparator + dd + dateSeparator + yyyy);
}

function GetDateObject(inputDateString, dateSeparator) {    
    if (IsNullOrEmpty(inputDateString)) { return null; }

    var dateParts = inputDateString.split(dateSeparator)

    var MM, dd, yyyy;

    dd = dateParts[0];
    MM = dateParts[1];
    yyyy = dateParts[2];

    var dObj = new Date(Number(yyyy), (Number(MM) - 1), Number(dd));

    return dObj;
}

function GetDayOfWeek(day) {
    var weekDay = "";
    switch (day)
    {
        case 0:
            weekDay = "Sunday";
            break;
        case 1:
            weekDay = "Monday";
            break;
        case 2:
            weekDay = "Tuesday";
            break;
        case 3:
            weekDay = "Wednesday";
            break;
        case 4:
            weekDay = "Thursday";
            break;
        case 5:
            weekDay = "Friday";
            break;
        case 6:
            weekDay = "Saturday";
            break;
        default:
            weekDay = "";
            break;
    }

    return weekDay;
}

function ParseJsonDate(date) {
    if (!IsNullOrEmpty(date)) {
        var dt = new Date(parseInt(date.substr(6)));

       return GetDate(dt, DateFormat.DDMMYYYY);
    }
    return "";
}

/*** Date Utility Functions END ****/

function trim(item) {
    if (!IsNullOrEmpty(item)) {
        return item.trim();
    }
    return item;
}

function RemoveLastCharacter(item, char) {
    if (!IsNullOrEmpty(item)) {
        var lastChar = trim(item).slice(-1);
        if (lastChar == char) {
            item = trim(item).slice(0, -1);            
        }
    }

    return item;
}

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() +(exdays*24*60*60*1000));
    var expires = "expires="+d.toUTCString();
    document.cookie = cname + "=" +cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}


/**** Load Left Side Menu ***/

function setCurrentMenu(menu, redirectUrl) {
    setCookie('TopMenu', menu, 1);

    setTimeout(function () {
        redirectTo(redirectUrl);
    },200);
    
}

function LoadLeftSideMenu() {    
    var menu = getCookie('TopMenu');

    if (!IsNullOrEmpty(menu)) {
        hideAllLeftSideMenu();
        $('.topMenu').removeClass('active');

        if (menu == "Dashboard") {
            $('#MenusDashboard').show();
            $('#menuDashboard').addClass('active');
        }
        else if (menu == "Reservation") {
            $('#MenusReservation').show();
            $('#menuReservation').addClass('active');
        }
        else if (menu == "FrontDesk") {
            $('#MenusFrontDesk').show();
            $('#menuFrontDesk').addClass('active');
        }
        else if (menu == "Cashiering") {
            $('#MenusCashiering').show();
            $('#menuCashiering').addClass('active');
        }
        else if (menu == "MasterScreen") {
            $('#MenusMasterScreens').show();
            $('#menuMasterScreen').addClass('active');
        }
    }
}

function hideAllLeftSideMenu() {
    $('.leftsidemenu').hide();
}

function goToByScroll(id) {
    // Scroll
    $('html,body').animate({
        scrollTop: $("#" + id).offset().top
    }, 'slow');
}

function arrayContains(searchItem, array) {
    return (array.indexOf(searchItem) > -1) ? true : false;
}

function changeColorOfDatePickerLabel(element) {
    if (element) {
        if (!IsNullOrEmpty($(element)[0].target.value)) {
            $('label[for="' + $(element)[0].target.id + '"]').addClass("label-bold");
        } else {
            $('label[for="' + $(element)[0].target.id + '"]').removeClass("label-bold");
        }
    }
}

function RemoveLineBreak(data) {
    if (data != null && data != undefined && data != "") {
        return data.replace(/(\r\n|\n|\r)/gm, "");
    }
    return "";
}

function FormatNumberWithTwoDecimal(amount) {
    if (amount != null && amount != '' && amount != undefined) {
        return Number(amount).toFixed(2);
    }
    return "0.00";
}

function RemoveArrayItemByValue(itemToRemove, arr) {
    if (arr != null && arr.length > 0) {
        arr.splice($.inArray(itemToRemove, arr), 1);
    }

    return arr;
}