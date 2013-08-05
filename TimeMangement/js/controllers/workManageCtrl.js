angular.module('WorkManagmentApp', ['ui.calendar', 'ui.select2'])
    .controller('WorkManagerCtrl', function ($scope, $http) {
        //$scope.selectedDay = day.date;
        var days = [];
        var projects = [];
        $scope.$watch('selectedDay', function () {
            $http.get('/api/WorkManageApi/GetDay/date?=' + $scope.selectedDay).success(function (data) {
                $scope.day = data;
            });
            
//            days.length = 0;
//            $http.get('api/WorkManageApi/GetMonth/monthReturn?=' + $scope.selectedDay).success(function (data) {
//                angular.forEach(data.Days, function (day, index) {
//                    days.push({ title: day.Activity + " - " + day.Projects + " - " + day.Details, start: day.Date, end: day.Date });
//                });
//            });
        });
        
 
        function getMonth(date) {
            var spiltdate=date.toString().split("-");
            var month = spiltdate[1];
            return month;
        }

        $scope.eventsF = function (start, end, callback) {
            var date = end.getFullYear() + "-" + (end.getMonth()) + "-" + end.getDate();
            days.length = 0;
            $http.get('api/WorkManageApi/GetMonth/monthReturn?=' + date).success(function (data) {
                angular.forEach(data.Days, function (day, index) {
                   if (day.Activity == "Work")
                        days.push({ title: day.Activity + " - " + day.Projects + " - " + day.Description, start: day.Date, end: day.Date});
                   else
                        days.push({ title: day.Activity, start: day.Date, end: day.Date, backgroundColor: '#FF7C00' });
                });
            });
            var events = days;
            callback(events);
        };

        $scope.AddHours = function () {
            $scope.day.TimeInfos.push({ StartTime: '', FinishTime: '' });
        };

        $scope.DelHours = function () {
            $scope.day.TimeInfos.pop();
        };
        
        $scope.AddProjects = function () {
            $scope.day.Projects.push($scope.newProject);
            $scope.newProject = '';
        };

        $scope.Save = function () {
            debugger;
            $http.post("/api/WorkManageApi/Save/dayInput?=", $scope.day);
        };
        
        $scope.events = days;
        

        $scope.eventSource = {
            url: "http://www.google.com/calendar/feeds/en.jewish%23holiday%40group.v.calendar.google.com/public/basic", //TO ADD HOLIDAYS
            backgroundColor: '#D25FD2',
            className: 'gcal-event',           
        };

        /* alert on eventClick */
        
        $scope.alertEventOnClick = function (date) {
            var counter = 1;
            var time = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
            $http.get('/api/WorkManageApi/GetDay/date?=' + time).success(function (data) {
                $scope.day = data;
            });
            angular.forEach(data.Projects, function (project, index) {
                projects.push({
                    id: counter++,
                    text: project
                });
                
            });
            angular.extend($scope.version3.data, projects);
            
        };

        /* config object */
        $scope.uiConfig = {
            calendar: {
                height: 650,
                selectable: true,
                header: {
                    left: 'title',
                    center: '',
                    right: 'today prev,next'
                },
                dayClick: $scope.alertEventOnClick,
                eventDrop: $scope.alertOnDrop,
                eventResize: $scope.alertOnResize,
    }
        };
     

        /* event sources array*/
        $scope.eventSources = [$scope.events, $scope.eventSource, $scope.eventsF];

        $scope.version3 = {
            multiple: true,
            data: []
    };
      
    });