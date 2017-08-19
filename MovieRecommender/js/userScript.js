userString = Cookies.get('user');

var userTemplate = null;
var user;
var explanation=null;

if (userString == undefined)
    user = null;
else
    user = JSON.parse(userString);

$("#signUpBtn").click(function () {

    var username = $("#signUpForm input[name='username']").val();
    var password = $("#signUpForm input[name='password']").val();

    var firstName = $("#signUpForm input[name='firstName']").val();
    var lastName = $("#signUpForm input[name='lastName']").val();

    if (username == "" || password == "" || firstName == "" || lastName == "") {
        alert("You must enter all fields!");
        return;
    }

    var newUser = "{username:\"" + username
        + "\",password:\"" + password
        + "\", firstName:\"" + firstName + "\", lastName:\"" + lastName + "\"}";

    $.ajax({
        type: "POST",
        url: "/api/User/registerUser",
        contentType: "application/json",
        data: newUser,
        success: function (data) {
            var res;
            if (data == true) {
                res = "Successfully signing up.";

                $.magnificPopup.close();

                $("#signUpForm input[name='username']").val("");
                $("#signUpForm input[name='password']").val("");

                $("#signUpForm input[name='firstName']").val("");
                $("#signUpForm input[name='lastName']").val("");


            }
            else
                res = "Username already exists!";

            alert(res);
        }
    });
});

$("#loginBtn").click(function () {
    var username = $("#loginForm input[name='username']").val();
    var password = $("#loginForm input[name='password']").val();

    if (username == "" || password == "") {
        alert("You must enter all fields!");
        return;
    }

    var userL = "{username:\"" + username
        + "\",password:\"" + password + "\"}";


    $.ajax({
        type: "POST",
        url: "api/user/loginUser",
        contentType: "application/json",
        data: userL,
        success: function (data) {
            if (data == null) {
                alert("Error logging in!");
            } else {
                user = data;
                user.admin = (username == "admin") ? true : false;
                if (user.admin)
                    $("#adminSec").removeAttr("style");
                else
                    $("#userSec").removeAttr("style");

                Cookies.set('user', user);
                $("#startSec").css("display", "none");

                $.magnificPopup.close();

                $("#loginForm input[name='username']").val("");
                $("#loginForm input[name='password']").val("");

                $("#editProfileForm input[name='username']").val(user.username);
                $("#editProfileForm input[name='password']").val(user.password);
                $("#editProfileForm input[name='firstName']").val(user.firstName);
                $("#editProfileForm input[name='lastName']").val(user.lastName);

                $("#loginLink").css("display", "none");
                $("#registerLink").css("display", "none");
                $("#logoutLink").removeAttr("style");
                $("#profileLink").removeAttr("style");

                $("#helloMsg").html("Hello, " + user.firstName + "!");
                $("#firstMsg").html("Recommended to you:");

                if (!user.admin) {
                    if ($.isEmptyObject(user.interestVector))
                        initRecomm();
                    else
                        renderRecommendation(user.username);
                }
                else {
                    renderUsers();
                    showAllMovies();
                }

                renderFilters();
            }
        }
    });
});

$("#logoutLink").click(function () {
    if (user.admin)
        $("#adminSec").css("display", "none");
    else
        $("#userSec").css("display", "none");

    user = null;
    Cookies.remove('user');

    $("#logoutLink").css("display", "none");
    $("#profileLink").css("display", "none");
    $("#loginLink").removeAttr("style");
    $("#registerLink").removeAttr("style");

    $("#startSec").removeAttr("style");


    $("#helloMsg").html("Movie Recommender");
});

$("#editProfileBtn").click(function () {

    var username = $("#editProfileForm input[name='username']").val();
    var password = $("#editProfileForm input[name='password']").val();

    var firstName = $("#editProfileForm input[name='firstName']").val();
    var lastName = $("#editProfileForm input[name='lastName']").val();

    var newUser = "{username:\"" + username
        + "\",password:\"" + password
        + "\", firstName:\"" + firstName + "\", lastName:\"" + lastName + "\"}";

    $.ajax({
        type: "POST",
        url: "/api/User/update",
        contentType: "application/json",
        data: newUser,
        success: function () {
            alert("Successfully updated.");
            $.magnificPopup.close();

            $("#editProfileForm input[name='username']").val(username);
            $("#editProfileForm input[name='password']").val(password);
            $("#editProfileForm input[name='firstName']").val(firstName);
            $("#editProfileForm input[name='lastName']").val(lastName);
            user.username = username;
            user.password = password;
            user.firstName = firstName;
            user.lastName = lastName;

        }
    });
});

function deleteUser(username) {
    $.getJSON("/api/User/delete/" + username).done(function () {
        //alert("Successfully deleted user.");
        renderUsers();
    })
    .fail(function () {
        alert("Error connecting to server.");
    });
}

renderUsers = function () {

    $.ajax({
        async: true,
        type: "GET",
        url: "/api/User/getAll",
        success: function (data) {
            var source = $("#allUsersTemplate").html();
            usersTemplate = Handlebars.compile(source);

            var renderData = {
                users: data
            };

            $("#usersManager").html(usersTemplate(renderData));
        }
    });
}

renderSimilarUsers = function ()
{
    $.ajax({
        async: true,
        type: "GET",
        url: "/api/user/getSimUsers/"+user.username,
        success: function (data) {
            var source = $("#allUsersTemplate").html();
            usersTemplate = Handlebars.compile(source);
            var mode = (user.admin == true) ? true : false;

            var renderData = {
                users: data,
                admin: mode
            };

            $("#similarUsersDiv").html(usersTemplate(renderData));
        }
    });
}

watchedMovies = function (username)
{
    $.ajax({
        async: true,
        type: "GET",
        url: "/api/user/getWatchedMovies/" + username,
        success: function (data) {
            var source = $("#filteredMoviesTemplate").html();
            moviesTemplate = Handlebars.compile(source);

            var renderData = {
                movies: data,
                admin: false
            };

            $("#watchedMoviesDiv").html(moviesTemplate(renderData));
        }
    });
}




