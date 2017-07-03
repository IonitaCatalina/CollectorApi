var UserApp = angular.module('UserApp', [])

UserApp.controller('PatternsController', ['$scope', 'PatternsService', '$window', function ($scope, patternsService, $window) {
    $scope.questions = Array(0);
    $scope.newQuestions = Array(0);

    GetPatterns();
    function GetPatterns() {

        patternsService.GetPatterns()
            .then(function successCallback(response) {
                $scope.patterns = angular.fromJson(response.data);
            }, function errorCallback(response) {
                $scope.status = "There are no patterns in your repository. Create one, yo!"
            });
    }


    GetPublishedPatterns();
    function GetPublishedPatterns() {

        patternsService.GetPublishedPatterns()
            .then(function successCallback(response) {
                $scope.publishedPatterns = angular.fromJson(response.data);
            }, function errorCallback(response) {
                $scope.status = "There are no patterns in your repository. Create one, yo!"
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

    $scope.showPatternDetails = false;
    $scope.Edit = function (pattern) {

        $scope.showPatternDetails = true;
        $scope.showQuestionsTable = false;
        $scope.patternId = pattern.Id;
        if (pattern.AnswerSheet.length === 0) {
            $scope.status = "Test pattern contains no question. Please add questions! ";
            $scope.showQuestionsTable = false;
        } else {
            $scope.questions = pattern.AnswerSheet;
            $scope.showQuestionsTable = true;
        }         
    };

    $scope.ShowPublishedPatternDetails = function (pattern) {

        $scope.showPatternDetails = true;
        $scope.showQuestionsTable = false;
        $scope.patternId = pattern.Id;
        if (pattern.AnswerSheet.length === 0) {
            $scope.status = "Test pattern contains no question. Please add questions! ";
            $scope.showQuestionsTable = false;
        } else {
            $scope.questions = pattern.AnswerSheet;
            $scope.showQuestionsTable = true;
        }
    };

    $scope.Publish = function (id) {
        patternsService.PublishPattern(id).then(function successCallback(response) {
            GetPatterns();
        }, function errorCallback(response) {
        });
    };

    $scope.Back = function () {
        $scope.showPatternDetails = false;
        $scope.newQuestions = Array(0);
        $scope.questions = Array(0);
    }

    $scope.AddQuestion = function () {
        var answer = '';

        if ($scope.qOne === true) answer += '1'; else answer += '0';
        if ($scope.qTwo === true) answer += '1'; else answer += '0';
        if ($scope.qThree === true) answer += '1'; else answer += '0';
        if ($scope.qFour === true) answer += '1'; else answer += '0';

        var question = {
            Question: $scope.q,
            Answer: answer,
            AnswerString: $scope.Answer.One + ';' + $scope.Answer.Two + ';' + $scope.Answer.Three + ';' + $scope.Answer.Four + ';',
            PatternId: $scope.patternId
        };

        var serialized = JSON.stringify(question);
        // nu uita sa adaugi created by in controller
        $scope.newQuestions.push(angular.fromJson(serialized));
        $scope.questions.push(angular.fromJson(serialized));
        $scope.showQuestionsTable = true;

    }

    $scope.SaveAnswerSheet = function () { // $scope.newquestions trimis din html
        patternsService.SaveAnswerSheet($scope.newQuestions).then(function successCallback(response) {
            GetPatterns();
            $scope.newQuestions = Array(0);
        }, function errorCallback(response) {
        });
    };

    $scope.showAnswers = false;
    $scope.OnAdding = function (NumberOfAnswers) {
        $scope.answerNumber = NumberOfAnswers;
        $scope.showAnswers = true;
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

}]);