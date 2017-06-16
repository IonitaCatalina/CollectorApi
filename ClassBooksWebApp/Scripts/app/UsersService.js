UserApp.factory('UsersService', ['$http', function ($http) {

    var usersService = {};

    usersService.Login = function (userData) {
        return $http.post('/Login/LoginUser/', userData);
    };

    usersService.Register = function (userData) {
        return $http.post('/Login/RegisterUser/', userData);
    };

    usersService.GetSessionId = function (data) {
        return $http.get('/Login/GetSessionId/');
    }

    usersService.GetUsersByRole = function (data) {
        return $http.get('/Login/GetUsersBasedOnRole/' + data);
    }

    return usersService;

}]);  