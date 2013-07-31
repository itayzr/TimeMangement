angular.module('WorkManagmentApp')
    .controller('WorkManagerCtrl', function ($scope, $http) {
        //$scope.selectedDay = day.date;
        $scope.$watch('selectedDay', function () {
            $http.get('/api/WorkManageApi/date?=' + $scope.selectedDay).success(function (data) {
                $scope.day = data;
            });
        });

        $scope.AddHours = function () {
            $scope.day.TimeInfos.push({ StartTime: '', FinishTime: '' });
        };

        $scope.DelHours = function () {
            $scope.day.TimeInfos.pop();
        };

        $scope.Save = function () {
            debugger;
            $http.post("/api/WorkManageApi/Save", $scope.day);
        };
      
    });