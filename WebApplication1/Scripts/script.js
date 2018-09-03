
//$(document).keypress(function (event) {

//    var keycode = (event.keyCode ? event.keyCode : event.which);
//    if (keycode == '13') {
//        if ($("#myModal").is(":hidden")) {
//            alert("Opening Modal");
//            $("#myBtn").trigger("click");
//            return;
//        }
//        $("#myModal #enterBtn").trigger("click");

//        return;
//    }
//    //&& (String.fromCharCode(event.which).toLowerCase() === 'u')
//    else if ( event.shiftKey ) {
//        alert("Loading UserAccounts");
//        window.location = $("#nav-users").attr("href");
//    }

//});
var iShortCutControlKey = 18; // ALT; 
var keyEnter = 13;
var keyA = 65, keyB = 66, keyC = 67, keyF = 70, keyG = 71, keyL = 76, keyM = 77, keyP = 80, keyT = 84, keyU = 85;
var iKeyActive;
var iLastKey;
var bIsControlKeyActived = false;
var bIsLastKeyActive = false;
var bIsKeyActive = false;
var isHidden = true;

$("select:visible:first").focus();
function Load(url) {
    window.location.replace(url);
}
$("#profileImg .drop,#profileImg .drop li ").on('click', function () {
   // alert('clicked');
    var url = "UserAccounts/ChangePassword";
    Load(url);
});

$(document).keydown(function (e) {

    if (e.keyCode == keyEnter) {
        if ($("#myModal").is(":hidden")) {
            // alert("Opening Modal");
            if ($("#myEditBranchModal").css('display') === "block") {
                $(" #myEditBranchModal #myEditBranchBtn").trigger("click");
                return;
            }



            $("#myBtn").trigger("click");
            return;
        }
        if ($("#myAddTellerModal").is(":hidden")) {
            $(" #myOpenTellerModalBtn").trigger("click");
            return;
        }
//        if ($("#myAccountTypeModal").is(":hidden") &&
//            $("#myAccountDetailsModal").is(":hidden") &&
//            $("#myLoanFirstModal").is(":hidden") &&
//            $("#myLoanSecondModal").is(":hidden") &&
//            $("#myTermsModal").is(":hidden")) {
//            $("#myFirstBtn").trigger("click");
//            return;
//        } else {
//            return;
//        }
//        else {
//            if ($("#myAccountTypeModal").css('display') === "block" &&
//                $("#myAccountDetailsModal").is(":hidden") &&
//                $("#myLoanFirstModal").is(":hidden") &&
//                $("#myLoanSecondModal").is(":hidden") &&
//                $("#myTermsModal").is(":hidden")) {
//                $("#myAccountTypeBtn").trigger("click");
//                return;
//            }
//            if ($("#myAccountDetailsModal").css('display') === "block" &&
//                $("#myAccountTypeModal").is(":hidden") &&
//                $("#myLoanFirstModal").is(":hidden") &&
//                $("#myLoanSecondModal").is(":hidden") &&
//                $("#myTermsModal").is(":hidden")) {
//                $("#myAccountDetailsBtn").trigger("click");
//                return;
//            }
//            if ($("#myLoanFirstModal").css('display') === "block" &&
//                $("#myAccountTypeModal").is(":hidden") &&
//                $("#myAccountDetailsModal").is(":hidden") &&
//                $("#myLoanSecondModal").is(":hidden") &&
//                $("#myTermsModal").is(":hidden")) {
//                $("#myLoanFirstBtn").trigger("click");
//                return;
//            }
//            if ($("#myLoanSecondModal").css('display') === "block" &&
//                $("#myAccountTypeModal").is(":hidden") &&
//                $("#myAccountDetailsModal").is(":hidden") &&
//                $("#myLoanFirstModal").is(":hidden") &&
//                $("#myTermsModal").is(":hidden")) {
//                $("#myLoanSecondBtn").trigger("click");
//                return;
//            }
//            if ($("#myTermsModal").css('display') === "block" &&
//                $("#myAccountTypeModal").is(":hidden") &&
//                $("#myAccountDetailsModal").is(":hidden") &&
//                $("#myLoanFirstModal").is(":hidden") &&
//                $("#myLoanSecondModal").is(":hidden")) {
//                $("#myDisburseLoanBtn").trigger("click");
//                return;
//            }
//        }
        $(" #myAddTellerPostingModal #myAddTellerPostingBtn").trigger("click");

        $("#myModal #enterBtn").trigger("click");


        return;
    }
    else if (e.altKey && e.keyCode == keyU) {
        console.log("Key ALT+U was pressed!");
        window.location = $("#nav-users").attr("href");
    }
    else if (e.altKey && e.keyCode == keyL) {
        console.log("Key ALT+L was pressed!");
        alert("Logging Out");
        $("#nav-log-out").submit();

    }
    else if (e.altKey && e.keyCode == keyA) {
        console.log("Key ALT+A was pressed!");

        window.location = $("#nav-account-type").attr("href");

    }
    else if (e.altKey && e.keyCode == keyB) {
        console.log("Key ALT+B was pressed!");

        window.location = $("#nav-branch").attr("href");

    }
    else if (e.altKey && e.keyCode == keyF) {
        console.log("Key ALT+F was pressed!");

        window.location = $("#nav-financial-report").attr("href");

    }

});

$(document).keyup(function (e) {
    if (e.which == iShortCutControlKey) { bIsControlKeyActived = false; console.log("Key up : " + e.which); }
    if (e.which == iKeyActive && bIsControlKeyActived == true) { bIsKeyActive = true; console.log("Key up : ALT +" + e.which); }
    else {
        bIsKeyActive = false;
    }
    if (e.which == iLastKey && bIsLastKeyActive && bIsControlKeyActived == true) { bIsLastKeyActive = true; console.log("Key up : ALT +" + iKeyActive + "+" + e.which); }
    else {
        bIsLastKeyActive = false;
    }


}).keydown(function (e) {

    if (e.which == iShortCutControlKey) bIsControlKeyActived = true;
    if (bIsControlKeyActived == true && e.which != iShortCutControlKey && bIsKeyActive == false) {
        iKeyActive = e.which;
        console.log("Key Down :ALT + " + iKeyActive + " + " + e.which);
        bIsKeyActive = true;
    }
    if (bIsKeyActive == true && bIsControlKeyActived == true && e.which != iShortCutControlKey && e.which != iKeyActive && bIsLastKeyActive == false) {
        iLastKey = e.which;
        bIsLastKeyActive = true;
        shortcutLogic(iKeyActive, iLastKey);

    }

});

var shortcutLogic = function (iKeyActive, lastKey) {
    if (iKeyActive == keyC) {
        if (lastKey == keyM) {
            window.location = $("#nav-customer-mgt").attr("href");
        }
        else if (lastKey == keyA) {
            window.location = $("#nav-customer-acc").attr("href");
        }

    }
    if (iKeyActive == keyT) {
        if (lastKey == keyM) {
            window.location = $("#nav-teller-mgt").attr("href");
        }
        else if (lastKey == keyP) {
            window.location = $("#nav-teller-pst").attr("href");
        }

    }
    if (iKeyActive == keyG) {
        if (lastKey == keyC) {
            window.location = $("#nav-gl-ctgry").attr("href");
        }
        else if (lastKey == keyP) {
            window.location = $("#nav-gl-pst").attr("href");
        }
        else if (lastKey == keyA) {
            window.location = $("#nav-gl-acc").attr("href");
        }

    }
}



var GetStatus = function () {
    $.ajax({
        type: 'POST',
        url: '/api/EOD/GetStatus',
        data: JSON.stringify(businessStatusDto),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {

            $(".switch input").prop("checked", false);
            if (data == "Opened") {
                $("#postWarning").fadeOut();
                $(".switch #radio-d").prop("checked", true);
                $("#myAddGLPostingModal").css("display", "block");
                $("#myAddTellerPostingModal").css("display", "block");
            }
            else {
                $("#myAddGLPostingModal").css("display", "none");
                $("#myAddTellerPostingModal").css("display", "none");
                $("#postWarning").fadeIn();
                $(".switch #radio-c").prop("checked", true);

            }

        },
        error: function (errorMessage) {
            // alert('request failed');
            //                            var msg = JSON.stringify(errorMessage);
            var msg = JSON.parse(errorMessage.responseText);
            alert(msg, msg.message);
            //    $('myBusinessStatusModal #errorMessage').fadeIn(200,
            //      function () { $('myBusinessStatusModal #errorMessage').html(msg.message) });
        }

    });
}
GetStatus();

var action, intendedAction, presentAction;
var businessStatusDto;
// : Switching From Open to Close
$('.switch input').on('click', function (e) {
    e.preventDefault();
    var id = $(this).attr('id');
    //alert(id);
    var label = $(this).siblings('label');

    // : To get the radio label text
    label.each(function (index) {
        if ($(this).attr('for') == id) {
            action = $(this).text();

        }
    });

    if (action == "Closed") {
        intendedAction = "Close"
        presentAction = "Open";
    }
    else {
        intendedAction = "Open"
        presentAction = "Close";
    }
    $('#myBusinessStatusModal #businessStatus b').text(intendedAction);
    $('#myBusinessStatusModal #warning').fadeIn();
    $('#myBusinessStatusModal').fadeIn();
    businessStatusDto = {
        IntendedAction: intendedAction
    }


    //alert(action);
});
$('#myBusinessStatusModal .close').on('click', function () {
    location.reload();
});


$('#myBusinessStatusModal #myProceedBtn').on('click', function (e) {
    e.preventDefault();
    $.ajax({
        type: 'POST',
        url: '/api/EOD/Start',
        data: JSON.stringify(businessStatusDto),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            $('#myBusinessStatusModal #confirmationForm').fadeOut();

            $('#myBusinessStatusModal #successMessage').fadeIn(200,
                function () {


                    $('#myBusinessStatusModal #successMessage').html(data);
                    $('.switch input').prop('checked', false);
                    if (presentAction == "Open") {

                        $('.switch #radio-d').prop('checked', true);
                    }
                    else {
                        $('.switch #radio-c').prop('checked', true);
                    }
                });
            return;




        },
        error: function (errorMessage) {
            //alert('request failed');
            //                            var msg = JSON.stringify(errorMessage);
            var msg = JSON.parse(errorMessage.responseText);

            $('#myBusinessStatusModal #errorMessage').fadeIn(200,
                function () {
                    $('#myBusinessStatusModal #errorMessage').html(msg.message);
                });
        }

    });

});
$('#myBusinessStatusModal #myCancelBtn').on('click', function (e) {
    e.preventDefault();

});
$(document).ready(function () {
    $('.loader-container').fadeOut();
    var url = document.location.href;
    //    var str = url.substr(0, url.lastIndexOf('/'));
    //    var nUrl = str.substr(str.lastIndexOf('/') + 15);
    // alert(url);
    // $('.nav-list .row  a:contains(' + nUrl + ')').addClass('active');
    //    $(".nav-list .row").removeClass('active');
    //    var navIndex = localStorage.getItem("nav-index");
    //    var trueIndex = parseInt(navIndex / 7, 10);
    //    $('.nav-list .row ').on('click', function () {
    ////        $('.nav-list .row').removeClass('active');
    ////        $(this).addClass('active');
    ////        var active = $('.nav-list .active');
    ////        localStorage.setItem("nav-index", $("div").index(active));
    ////        $(".nav-list .row").removeClass('active');
    ////        console.log("initial Index = " + $("div").index(active));
    //        $('.nav-list .row').removeClass('active');
    //        $(this).addClass('active');
    //        var active = $('.nav-list .active');
    //        alert($(this).index(active));
    //    });
    //    console.log("True Index = " + trueIndex);

    //    $(".nav-list .row:nth-child("+trueIndex+")").addClass('active');
    //    $(".nav-list .row").removeClass('active');
    //    $(".nav-list div")[trueIndex].addClass('active');


    /**
     * BEGINNING OF MODAL SETTINGS
     */

    // Get the modal
    var modal = document.getElementById('myModal');

    // Get the button that opens the modal
    var btn = document.getElementById("myBtn");

    // When the user clicks on the button, open the modal 
    btn.onclick = function (e) {
        e.preventDefault();
        $(".modal-content").addClass("fadeInDown");
        modal.style.display = "block";
        // $("#myModal input:visible:first").focus();

    }

    // When user clicks to close icon, close the modal
    $('.close').on("click", function () {
        $(".modal").fadeOut();
    });

    // Get the <span> element that closes the modal
    var span = document.getElementsByClassName("close")[0];
    // When the user clicks on <span> (x), close the modal
    span.onclick = function () {

        modal.style.display = "none";

    }

    // When the user clicks anywhere outside of the modal, close it
    window.onclick = function (event) {
        if (event.target === modal) {
            modal.style.display = "none";
        }
    }


    /**
     * END OF MODAL SETTINGS
     */



    $("button").on('click', function () {

        $(this).children('.loader').fadeIn();
    });

    $(".pagination ").ready(function () {
        $(".pagination .previous").ready(function () {
            $(".pagination .previous a").html("<i class='fa fa-long-arrow-left small-text'></i>");
            $(".pagination .next a").html("<i class='fa fa-long-arrow-right small-text'></i>");
        });
    });





    //"<a href='#' aria-controls='branches' data-dt-idx='0' tabindex='0'><i class='fa fa-arrow-left'></i></a>"
});