var UserApp = angular.module('UserApp', [])

UserApp.controller('PatternsController', ['$scope', 'PatternsService', '$window', function ($scope, patternsService, $window) {

    GetPatterns();
    function GetPatterns() {

        patternsService.GetPatterns()
            .then(function successCallback(response) {
                $scope.patterns = angular.fromJson(response.data);         
            }, function errorCallback(response) {
                $scope.status = "There are no patterns in your repository. Create one, yo!"
            });

        function GetAnswerSheetQuestions(id)
        {
            patternsService.GetPatterns(id)
                .then(function successCallback(response) {
                    $scope.answerSheet = angular.fromJson(response.data);
                }, function errorCallback(response) {
                    $scope.status = "There are no answersheets for this pattern."
                });
        }

        $scope.AddPattern = function () {
            var pattern = {
                Name: $scope.PatternName
            }

            patternsService.CreatePattern(JSON.stringify(pattern))
                .then(function successCallback(response) {
                    GetPatterns();
                }, function errorCallback(response) {
                    $scope.status = 'Could not create the pattern!';
                });
        }

        $scope.RemovePattern = function (id) {

            patternsService.RemovePattern(JSON.stringify(id))
                .then(function successCallback(response) {
                    GetPatterns();
                }, function errorCallback(response) {
                    $scope.status = 'Could not delete the pattern!';
                });
        }
    }

}]);