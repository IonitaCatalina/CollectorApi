UserApp.factory('PatternsService', ['$http', function ($http) {

    var patternsService = {};

    patternsService.GetAnswerSheet = function (id) {
        return $http.post('/Patterns/GetAnswerSheet/', id);
    };

    patternsService.GetPatterns = function () {
        return $http.get('/Patterns/GetPatterns/');
    }

    patternsService.CreatePattern = function (data) {
        return $http.post('/Patterns/AddPattern/', data);
    };

    patternsService.RemovePattern = function (data) {
        return $http.post('/Patterns/RemovePattern/'+ data);
    };

    return patternsService;

}]);