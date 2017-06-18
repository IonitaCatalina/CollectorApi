UserApp.factory('ClassBooksService', ['$http', function ($http) {

    var classBookService = {};

    classBookService.GetClassBooks = function () {
        return $http.get('/ClassBook/GetClassBooks/');
    };

    classBookService.CreateClassBook = function (classBook) {
        return $http.post('/ClassBook/CreateClassBook/', classBook);
    };

    classBookService.AddStudentToClassBook = function (refs) {
        return $http.post('/ClassBook/AddStudentToClassBook/', refs);
    }

    classBookService.RemoveStudent = function (refs) {
        return $http.post('/ClassBook/RemoveStudent/', refs);
    };
    return classBookService;

}]);  