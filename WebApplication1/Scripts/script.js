$(document).ready(function () {
    $(".nav-list .row").removeClass('active');
    var navIndex = localStorage.getItem("nav-index");
    var trueIndex = parseInt(navIndex / 7, 10);
    console.log("True Index = " + trueIndex);
    
    $(".nav-list .row:nth-child("+trueIndex+")").addClass('active');
//    $(".nav-list .row").removeClass('active');
//    $(".nav-list div")[trueIndex].addClass('active');
    // Get the modal
    var modal = document.getElementById('myModal');

    // Get the button that opens the modal
    var btn = document.getElementById("myBtn");

    // Get the <span> element that closes the modal
    var span = document.getElementsByClassName("close")[0];

    // When the user clicks on the button, open the modal 
    btn.onclick = function (e) {
        e.preventDefault();
        $(".modal-content").addClass("fadeInDown");
        modal.style.display = "block";

    }

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