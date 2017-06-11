UserApp.factory('UsersService', ['$http', function ($http) {

    var UsersService = {};
    var urlBase = "http://192.168.0.101:90";

    var user = JSON.stringify({ "Email": "catalina.ionita@centric.eu", "Password": "Password1!", "ConfirmPassword": "Password1!" });

    UsersService.Login = function () {
        return $http.post(urlBase + '/api/users/GetUserByEmail', user);
    };

    UsersService.SetSession = function (data) {
        $http.post('/Login/SetSession/'+data);
    }
    return UsersService;

}]);  