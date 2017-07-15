onboardapp.directive('inputText', function () {
    return {
        restrict: 'E',
        scope: {
            type: '=type',
            validationInfo: '=validation'
        },
        templateUrl: function (elem, attr) {
            return "/Input/Text"
        }
    };
});