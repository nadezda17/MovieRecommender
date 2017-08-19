
$(document).ready(function () {

    if (user != null) {
        if (!user.admin) {

            if ($.isEmptyObject(user.interestVector))
                initRecomm();
            else
            {
                renderSimilarUsers();
                renderRecommendation(user.username);
            }

            $("#userSec").removeAttr("style");
            $("#profileLink").removeAttr("style");
            $("#helloMsg").html("Hello, " + user.firstName + "!");


            $("#loginLink").css("display", "none");
            $("#registerLink").css("display", "none");
            $("#logoutLink").removeAttr("style");

            renderFilters();
        }
        else {
            $("#adminSec").removeAttr("style");
            showAllMovies();
            renderUsers();

            $("#loginLink").css("display", "none");
            $("#registerLink").css("display", "none");
            $("#profileLink").css("display", "none");
            $("#logoutLink").removeAttr("style");
        }
    }
    else {
        $("#startSec").removeAttr("style");
    }
    
    $("#relDatePicker").datepicker();
});

$('.popup-with-form').magnificPopup({

       type:'inline',
       fixedContentPos: false,
       removalDelay: 200,
       showCloseBtn: false,
       mainClass: 'mfp-fade'       

});

$('.movieTrailer').magnificPopup({
    items:
        {
        src: "https://www.youtube.com/watch?v=GvyNyFXHj4k",
        type: 'iframe'
    }
});

$('.movieExplanation').magnificPopup({
    
});

$('.movieDetails').magnificPopup({
});

$(document).on('click', '.close-popup', function (e) {
    e.preventDefault();
    $.magnificPopup.close();
});

function uploadImage() {
    var file = $(".image").get(0).files;
    if (file.length == 1) {
        var data = new FormData();
        data.append("image", file[0]);
        $.ajax({
            type: "POST",
            url: "api/movie/uploadImage",
            contentType: false,
            processData: false,
            data: data,
            success: function (message) {
                alert(message);
            },
            error: function () {
                alert("Error connecting to server.");
            }
        });
    } else {
        return;
    }
};

$("input.image").change(function () { 
    var file = $(this)[0].files[0];
    if (file != null)
        $("#insertMovieForm input[name='imageName']").val(file.name);
    else
        $("#insertMovieForm input[name='imageName']").val("");
});

function changeImageName() { //ovo ne radi :)
    var coffeeName = $("input.newImage").data("oldName");
    var file = $(this)[0].files[0];
    if (file != null)
        $("#newImageName" + oldName).val(file.name);
    else
        $("#newImageName" + oldName).val("");
}

$(document).on('change', 'input.newImage', function () {
    changeImageName();
});
