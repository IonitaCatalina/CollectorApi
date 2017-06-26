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

        $scope.GetImage = function (id) {
            patternsService.GetPatternImage(id).then(function successCallback(response) {
                $scope.image = _arrayBufferToBase64(angular.fromJson(response.data).Image);
            }, function errorCallback(response) {
                $scope.status = 'Could not delete the pattern!';
            });
        };

        function _arrayBufferToBase64(buffer) {
            var binary = '';
            var bytes = new Uint8Array(buffer);
            var len = bytes.byteLength;
            for (var i = 0; i < len; i++) {
                binary += String.fromCharCode(bytes[i]);
            }
            return window.btoa(binary);
        }
    }

}]);