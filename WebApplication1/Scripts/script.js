$(document).ready(function () {
    $(".nav-list .row").removeClass('active');
    var navIndex = localStorage.getItem("nav-index");
    var trueIndex = parseInt(navIndex / 7, 10);
    console.log("True Index = " + trueIndex);
    
    $(".nav-list .row:nth-child("+trueIndex+")").addClass('active');
//    $(".nav-list .row").removeClass('active');
//    $(".nav-list div")[trueIndex].addClass('active');


    /**
     * BEGINNING OF MODAL SETTINGS
     */


  

    //VARIABLES FOR CUSTOMER ACCOUNT CREATION

    var accountTypeId = 0, customerId, branchId = 0, accountName, loanAmount, linkedCustomerAccountBank, linkedCustomerAccountNumber, bvn,tenure,collateral;
    var accountNumber,paymentRate,accountId;

    //MODALS - Get modal for Customer Account Modal

    // Modal for selecting account type
    var accountTypeModal = document.getElementById('myAccountTypeModal');
    // Modal for inputting customer linked account and loan amount
    var loanFirstModal = document.getElementById('myLoanFirstModal');
    // Modal for displaying interest rate and terms
    var loanSecondModal = document.getElementById('myLoanSecondModal');
    // Modal for entering account details for Customer Account
    var accountDetailsModal = document.getElementById('myAccountDetailsModal');


    // BUTTONS - Get the buttons that open the modals for Customer Account

    var firstBtn = document.getElementById("myFirstBtn");
    // Button for Account Type Modal
    var accountTypeBtn = document.getElementById("myAccountTypeBtn");
    // Button for Account Details Modal
    var accountDetailsBtn = document.getElementById("myAccountDetailsBtn");
    // Modal for Loan First Modal
    var loanFirstBtn = document.getElementById("myLoanFirstBtn");
    // Modal for Loan Second Modal
    var loanSecondBtn = document.getElementById("myLoanSecondBtn");

    // Get the <span> element that closes the modal
    var span = document.getElementsByClassName("close")[0]; 

    //CUSTOMER ACCOUNT PROCESS USER CLICKS
//
//    firstBtn.onclick = function (e) {
//
//        //alert('clicked');
//        e.preventDefault();
//
//        //$("#accountDetailsModal .modal-content").addClass("fadeInDown");
//        accountTypeModal.style.display = "block";
//
//
//    }


//    accountTypeBtn.onclick = function (e) {
//
//        e.preventDefault();
//        var thisBtn = $("#myAccountTypeModal");
////        var accType = $(".accountTypeDropdown").text();
////        accountTypeId = $(".accountTypeDropdown").val();
//        accountDetailsModal.fadeIn();
//       // thisBtn.fadeOut();
////        if (accType === "Loan Account") {
////
////            thisBtn.fadeOut(300, function () {
////                $("#myLoanFirstModal .modal-content").addClass("fadeInDown");
////                loanFirstModal.style.display = "block";
////            });
////            
////        } else {
////            thisBtn.fadeOut(300, function () {
////                $("#myAccountDetailsModal .modal-content").addClass("fadeInDown");
////               accountDetailsModal.style.display = "block";
////            });
////        }
//
//    }

//    accountDetailsBtn.onclick = function (e) {
//        var thisModal = $("myAccountDetailsModal");
//        e.preventDefault();
//
//        branchId = $("branchId").val();
//        customerId = $("customerId").val();
//        accountName = $("accountName").val();
//
//
//        /*** AJAX CALL ***/
//
//    }
//
//
//    loanFirstBtn.onclick = function (e) {
//        var thisModal = $("myLoanFirstModal");
//        e.preventDefault();
//        loanAmount = $("loanAmount").val();
//        customerLinkedAccountId = $("customerLinkedAccountId").val();
//
//        /*** AJAX CALL ***/
//
//        thisModal.fadeOut(300, function () {
//            $("#myLoanSecondModal .modal-content").addClass("fadeInDown");
//            loanSecondModal.style.display = "block";
//        });
//
//    }
//
//    loanSecondBtn.onclick = function (e) {
//        
//        e.preventDefault();
//        loanSecondModal.style.display = "none";
//
//    }

//    // When the user clicks on <span> (x), close the modal
//    span.onclick = function () {
//
//        modal.style.display = "none";
//        accountDetailsModal.style.display = "none";
//        accountTypeModal.style.display = "none";
//        loanFirstModal.style.display = "none";
//        loanSecondModal.style.display = "none";
//    }

    $('.close').on("click", function() {
        $(".modal").fadeOut();
    });

    // When the user clicks anywhere outside of the modal, close it
    window.onclick = function (event) {
        if (event.target === modal || event.target === accountDetailsModal || event.target === accountTypeModal || event.target === loanFirstModal || event.target === loanSecondModal) {
            modal.style.display = "none";
//            accountDetailsModal.style.display = "none";
//            accountTypeModal.style.display = "none";
//            loanFirstModal.style.display = "none";
//            loanSecondModal.style.display = "none";
        }
    }

    /** GENERAL SETTINGS */

    // Get the modal
    var modal = document.getElementById('myModal');

    // Get the button that opens the modal
    var btn = document.getElementById("myBtn");


    // When the user clicks on the button, open the modal 
    btn.onclick = function (e) {
        e.preventDefault();
        $(".modal-content").addClass("fadeInDown");
        modal.style.display = "block";

    }


    /**
     * END OF MODAL SETTINGS
     */



    $('.nav-list .row ').on('click', function () {
        $('.nav-list .row').removeClass('active');
        $(this).addClass('active');
        var active = $('.nav-list .active');
        localStorage.setItem("nav-index", $("div").index(active));
        $(".nav-list .row").removeClass('active');
        console.log("initial Index = " + $("div").index(active));
    });

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