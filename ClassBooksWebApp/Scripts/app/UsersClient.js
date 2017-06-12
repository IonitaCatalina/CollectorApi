var UserApp = angular.module('UserApp', [])

UserApp.controller('UsersController', function ($scope, UsersService) {

   $scope.Login = function Login() {
       UsersService.Login()
           .then(function successCallback(response) {
               //set session in web app
               UsersService.SetSession(response.data);

           }, function errorCallback(response) {
               $scope.status = 'Unable to load customer data: ' + response.message;
               console.log($scope.status);
           });
        }
});