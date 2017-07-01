var UserApp = angular.module('UserApp', [])

UserApp.controller('UsersController', ['$scope', 'UsersService', '$window', function ($scope, usersService) {

    $scope.LoginUser = function ($window) {
        var userData = {
            Email: $scope.Email,
            Password: $scope.Password,
            ConfirmPassword: $scope.Password
        }

        usersService.Login(JSON.stringify(userData))
            .then(function successCallback(response) {
                $scope.result = angular.fromJson(response.data);
                $scope.session = $scope.result.Id;
                if ($scope.result.Roles[0] === 'Student')
                    window.location.pathname = 'Students/Home';
                else
                    window.location.pathname = 'ClassBook/Index';
                $scope.status = 'Logged in!';
            }, function errorCallback(response) {
                window.location.pathname = 'Login/Login';
                $scope.status = 'Could not log in user with current credentials!';
            });
    }

    $scope.RegisterUser = function () {
        var userData = {
            Email: $scope.emailRegister,
            Username: $scope.usernameRegister,
            FirstName: $scope.FirstName,
            LastName: $scope.LastName,
            RoleName: $scope.Role,
            Password: $scope.passwordRegister,
            ConfirmPassword: $scope.confirmPassword
        }

        usersService.Register(JSON.stringify(userData))
            .then(function successCallback(response) {
                window.location.pathname = 'ClassBook/Index';
                $scope.status = 'Registered!';
            }, function errorCallback(response) {
                $scope.status = 'Could not register user with current credentials!';
            });
    }
}]);