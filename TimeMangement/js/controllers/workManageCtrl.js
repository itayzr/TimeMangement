angular.module('TimeManagment')
    .controller('WorkManagerCtrl', function ($scope) {
        $scope.months = window.workManagement.months;
        $scope.currentMonth = $scope.months[0];
    });