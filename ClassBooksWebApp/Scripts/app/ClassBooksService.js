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

    classBookService.GetPhotos = function (student) {
        return $http.post('/students/GetUserPhotos/', student);
    };
    classBookService.GetPhoto = function (id) {
        return $http.post('/students/GetPhoto/'+ id);
    };


    return classBookService;

}]);  