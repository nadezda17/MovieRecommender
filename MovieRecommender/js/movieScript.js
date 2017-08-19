var moviesTemplate = null;
var currentID = null;
var currentMovie = null;
var usernames = null;
var genres = null;
var directors = null;
var actors = null;
var cinemas = null;

deleteMovie = function (id) {
    $.getJSON("/api/Movie/delete/" + id).done(function () {
        //alert("Successfully deleted movie.");
        showAllMovies();
    })
    .fail(function () {
        alert("Error connecting to server.");
    });
}

$("#insertMovieBtn").click(function () {

    
    var imageName = $("#insertMovieForm input[name='imageName']").val();
    if (imageName != "")
        uploadImage();

    var newMovie = JsonMovieString();

    $.ajax({
        type: "POST",
        url: "/api/Movie/insert",
        contentType: "application/json",
        data: newMovie,
        success: function () {
            
        }
    });

});
$("#filterBtn").click(function () {
    var genre = $("#genreFilterEl").val();
    var director = $("#directorFilterEl").val();
    var actor = $("#actorFilterEl").val();
    var cinema = $("#cinemaFilterEl").val();

    var dataObject = "{genre:\"" + genre
        + "\",director:\"" + director
        + "\",actor:\"" + actor
        + "\", cinema:\"" + cinema + "\"}";

    $.ajax({
        type: "POST",
        url: "api/Movie/filterMovies",
        contentType: "application/json",
        data: dataObject,
        success: function (data) {
            renderMovies(data, "#searchMoviesDiv", true);
        }
    });

});

function triggerMovieLike(movieId, like) {

    //var username = "nadja123";
    //var movieId = 2;
    //var like = true;

    var dataObject = "{username:\"" + user.username
        + "\",movieId:\"" + movieId
        + "\", like:\"" + like + "\"}";

    $.ajax({
        type: "POST",
        url: "/api/User/movieLiked",
        contentType: "application/json",
        data: dataObject,
    });

    var res = (like) ? "<h3>Movie liked!</h3>" : "<h3>Movie disliked!</h3>";
    $('#movieLikedPopup').html(res);
    $.magnificPopup.open({
        items: {
            src: '#movieLikedPopup'
        },
        type: 'inline',
        closeBtnInside:false
    });
}

initRecomm = function () {
    $.ajax({
        async: true,
        type: "GET",
        url: "/api/Movie/initRecomm",
        success: function (data) {
            renderMovies(data, "#recommList", false);
            $("#firstMsg").html("Welcome to Movie Recommender! In order to recommend you movies, we would like to now which one of these movies do you prefer:");

        }
    })
    .fail(function () {
        alert("Error connecting to server.");
    });

}
showAllMovies = function () {
    $.ajax({
        async: true,
        type: "GET",
        url: "/api/Movie/getAll",
        success: function (data) {
            renderMovies(data, "#moviesManager", false);

        }
    })
    .fail(function () {
        alert("Error connecting to server.");
    });
}
showMovieDetails = function (id)
{
    $.ajax(
        {
            type: "GET",
            url: "api/movie/getMovie/" + id,
            success: function (data) {
                currentMovie = data;
                renderMovieDetails(currentMovie);
            }

        });

    $.magnificPopup.open({
        items: {
            src: '#popupMovieDetails'
        },
        type: 'inline',
        closeBtnInside: false
    });
}
showExplanation = function (id) {
    currentID = id;
    $.ajax(
        {
            type: "GET",
            url: "api/Movie/getExplanation/" + user.username,
            //data: {username:user.username, movieId:id},
            success: function (data) {
                explanation = data;
                usernames = data[currentID]["explanationCF"].map(function (user) { return user.username; });
                renderExplanation(currentID, usernames);

            }

        });


    $.magnificPopup.open({
        items: {
            src: '#popupMovieExpl'
        },
        type: 'inline',
        closeBtnInside: false,
        callbacks: {
            open: function () {
                $.magnificPopup.instance.close = function () {
                    $("#popupMovieExpl").html("");
                    $.magnificPopup.proto.close.call(this);
                };
            }
        }
    });
}
showTrailer = function (link)
{
    $.magnificPopup.open({
        items: {
            src: link
        },
        type: 'iframe',        
        closeBtnInside:false
    });
}

function renderMovies(data, elemSelector, filter) {
    var source;
    if (filter)
        source = $("#filteredMoviesTemplate").html();
    else
        source = $("#allMoviesTemplate").html();

    moviesTemplate = Handlebars.compile(source);
    var mode = (user.admin == true) ? true : false;

    var renderData = {
        movies: data,
        admin: mode,
    };

    $(elemSelector).html(moviesTemplate(renderData));
}
function renderRecommendation(username) {
    
    $.ajax({
        type: "GET",
        url: "/api/Recommendation/get/" + username,
        contentType: "application/json",
        success: function (data) {
            renderMovies(data, "#recommList",false);
            $("#firstMsg").html("Recommended to you:");
        }
    });
}
function renderExplanation(id, usernames) {
    var CB = explanation[id]["explanationCB"];
    var CF = explanation[id]["explanationCF"];
    var terms = Object.keys(CB);
    var weights = Object.values(CB);

    var htmlContent = "<ul class='container float'>" +
    "<li class='item float-item'>" +
    "<div style='width:300px;background: darkseagreen; opacity:0.9;border-radius: 15px;'>" +
    "Based on your interest:";
    for (var i = 0; i < terms.length; i++) {
        htmlContent += "\n<div>" + terms[i] + ":" + weights[i] + "</div>";
    }
    htmlContent += "</ul>";

    htmlContent += "<ul class='container float'>" +
   "<li class='item float-item'>" +
   "<div style='width:300px;background: darkseagreen; opacity:0.9; border-radius: 15px;'>" +
   "Users similar to you liked this movie:";
    for (var i = 0; i < usernames.length; i++) {
        htmlContent += "\n<div>" + usernames[i] + "</div>";
    }
    htmlContent += "</ul>";



    $("#popupMovieExpl").html(htmlContent);
}
function renderMovieDetails(movie)
{
    var htmlString="<ul class='container'>"+
    "<li class='item float-item'>"+
    "<div style='width:220px;opacity:0.9;background: floralwhite;border-radius: 15px;'>" +
            "<div>Title:"+ movie.title+"</div>"+
            "<div>Genres:";
    for(var i=0;i<movie.genres.length;i++)
        htmlString+=movie.genres[i]+", ";

    htmlString+="<div>Director:"+ movie.director+"</div>"+
        "<div>Actors:";
    for(var i=0;i<movie.actors.length;i++)
        htmlString+=movie.actors[i]+", ";

    htmlString+="<div>Cinema:"+ movie.cinema+"</div>";
    htmlString += "<div>Time period:" + movie.timePeriod + "</div> </ul>";


    $("#popupMovieDetails").html(htmlString);
}
function renderFilters()
{
    //genres
    $.ajax(
        {
            type: "GET",
            url: "api/Movie/getGenres",
            success: function (data) {
                var genres = data;
                var htmlGenre = "<option value='' selected></option>";
                for (var i = 0; i < genres.length; i++)
                    htmlGenre += "\n<option value='" + genres[i] + "'>" + genres[i] + "</option>";
                $("#genreFilterEl").html(htmlGenre);

            }

        });
    //directors
    $.ajax(
        {
            type: "GET",
            url: "api/Movie/getDirectors",
            success: function (data) {
                var directors = data;
                var htmlDirector = "<option value='' selected></option>";
                for (var i = 0; i < directors.length; i++)
                    htmlDirector += "\n<option value='" + directors[i] + "'>" + directors[i] + "</option>";
                $("#directorFilterEl").html(htmlDirector);

            }
        });
    //actors
    $.ajax(
        {
            type: "GET",
            url: "api/Movie/getActors",
            success: function (data) {
                var actors = data;
                var htmlActor= "<option value='' selected></option>";
                for (var i = 0; i < actors.length; i++)
                    htmlActor += "\n<option value='" + actors[i] + "'>" + actors[i] + "</option>";
                $("#actorFilterEl").html(htmlActor);

            }
        });
    //cinema
    $.ajax(
    {
        type: "GET",
        url: "api/Movie/getCinemas",
        success: function (data) {
            var cinemas = data;
            var htmlCinemas= "<option value='' selected></option>";
            for (var i = 0; i < cinemas.length; i++)
                htmlCinemas += "\n<option value='" + cinemas[i] + "'>" + cinemas[i] + "</option>";
            $("#cinemaFilterEl").html(htmlCinemas);

        }
    });


}

function JsonMovieString() {
    var title = $("#insertMovieForm input[name='title']").val();
    var imageName = $("#insertMovieForm input[name='imageName']").val();
    var releaseDate = $("#insertMovieForm input[name='releaseDate']").val();

    var director = $("#insertMovieForm input[name='director']").val();
    var cinema = $("#insertMovieForm input[name='cinema']").val();
    var timePeriod = $("#insertMovieForm input[name='tperiod']").val();

    var genres = $("#insertMovieForm input[name='genres']").map(function (_, el) {
        if ($(el).val() != "")
            return $(el).val();
    }).get();
    var actors = $("#insertMovieForm input[name='actors']").map(function (_, el) {
        if ($(el).val() != "")
            return $(el).val();
    }).get();

    if (title == "" || releaseDate == "" || genres == "" || director == "")
        return;

    var newMovie = "{imageFileName:\"" + imageName
       + "\",title:\"" + title
       + "\",releaseDate:\"" + releaseDate
       + "\",  genres:\"" + genres + "\", director:\"" + director + "\", actors:\"" + actors
       + "\" , cinema:\"" + cinema + "\", timePeriod:\"" + timePeriod + "\"}";

    return newMovie
}