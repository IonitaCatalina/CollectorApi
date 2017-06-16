var UserApp = angular.module('UserApp', [])

UserApp.controller('ClassBooksController', ['$scope', 'ClassBooksService', 'UsersService', '$window', function ($scope, classBookService, usersService) {

    GetClassBooks();
    $scope.students = null;
    $scope.allStudents = null;

    function GetClassBooks() {       

        classBookService.GetClassBooks()
            .then(function successCallback(response) {
                $scope.classBooks = angular.fromJson(response.data);
            }, function errorCallback(response) {
                $scope.status = "You don't have any class books yet."
            });
    }

    GetStudents();
    function GetStudents() {
        usersService.GetUsersByRole('Student')
            .then(function successCallback(response) {   
                $scope.allStudents = angular.fromJson(response.data);
            }, function errorCallback(response) {
                $scope.status = "You don't have any class books yet."
            });
    }

    $scope.CreateClassBook = function () {
        var classBook = {
            Name: $scope.ClassBookName
        }

        classBookService.CreateClassBook(JSON.stringify(classBook))
            .then(function successCallback(response) {
                $scope.status = 'Created!';
                $scope.classBooks = GetClassBooks();
            }, function errorCallback(response) {
                $scope.status = 'Could not create the class book!';
            });
    }

    $scope.AddStudent = function (newStudent) {

        if ($scope.selectedCatalogue != null || $scope.selectedCatalogue != 'undefined') {
            var refs = '{"ClassBookId":"' + $scope.selectedCatalogue + '","StudentId":"' + newStudent.Id + '"}';

            classBookService.AddStudentToClassBook(refs)
                .then(function successCallback(response) {
                    $scope.Table = response;
                    GetClassBooks();
                }, function errorCallback(response) {
                });
        }
    }

    $scope.SetSectionDetails = function(classBook)
    {
        $scope.CatalogueName = classBook.Name;
        $scope.selectedCatalogue = classBook.Id;
        if (classBook.Students.length > 0) {
            $scope.students = classBook.Students;
        }
        else {
            $scope.students = null;
        }
    }
}]);