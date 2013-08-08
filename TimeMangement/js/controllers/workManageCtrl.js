angular.module('WorkManagmentApp', ['ui.calendar', 'ui.select2'])
    .controller('WorkManagerCtrl', function ($scope, $http, $filter) {
       
        var workEvents = [];
        var projects = [];
        var outsideProject = [];
        var projectsCounter = 1;
        
        /*bring outside projects*/
        $http.get('/api/WorkManageApi/GetProjects').success(function (data) {      
            angular.forEach(data, function(project, index) {
                outsideProject.push({
                    id: projectsCounter++,
                    text: project,
                });
            });
            angular.extend($scope.version3.data, outsideProject);
        });
        
        /* first load-bring today*/
        $http.get('/api/WorkManageApi/GetDay/date?=').success(function (data) {
            $scope.day = data;
            $scope.selectedDay = $scope.day.Date.split('T')[0];
            buildProjects();   
        });

        $scope.ChangeDay = function () {
            $http.get('/api/WorkManageApi/GetDay/date?=' + $scope.selectedDay).success(function(data) {
                $scope.day = data;
                buildProjects();
                var spiltdate=$scope.selectedDay.split('-');
                $scope.myCalendar.fullCalendar('gotoDate', spiltdate[0],spiltdate[1]-1,spiltdate[2]);
            });
            
        };
        
        $scope.AddHours = function () {
            $scope.day.TimeInfos.push({ StartTime: '', FinishTime: '' });
            var size = $scope.day.TimeInfos.length - 1;
            $scope.day.TimeInfos[size].StartTime = $scope.day.TimeInfos[size - 1].FinishTime;
            $scope.day.TimeInfos[size].FinishTime = $scope.day.TimeInfos[size - 1].FinishTime;
        };

        $scope.DelHours = function () {
            $scope.day.TimeInfos.pop();
        };
        
        $scope.showDel = function () {
            if ($scope.day.TimeInfos.length > 1)
                return true;
        };

        $scope.Save = function() {
            projects.length = 0;
            angular.forEach($scope.day.Projects, function(project, index) {
                projects.push(project.text);
            });
            angular.copy(projects, $scope.day.Projects);
            $http.post("/api/WorkManageApi/Save/dayInput?=", $scope.day).success(function () {
                $scope.myCalendar.fullCalendar('refetchEvents');
                buildProjects();
            });  
        };

        $scope.submit = function() {
            projects.length = 0;
            angular.forEach($scope.day.Projects, function(project, index) {
                projects.push(project.text);
            });
            angular.copy(projects, $scope.day.Projects);
            $http.post("/api/WorkManageApi/Save/dayInput?=", $scope.day).success(function() {
                $scope.myCalendar.fullCalendar('refetchEvents');
                buildProjects();
            });
        };

        $scope.events = workEvents;
        
        /* Build  3 month view */
        $scope.eventsF = function (start, end, callback) {
            var startMonth = start.getFullYear() + "-" + (start.getMonth()+1) + "-" + start.getDate();
            var thisMonth = end.getFullYear() + "-" + (end.getMonth()) + "-" + end.getDate();
            var endMonth = end.getFullYear() + "-" + (end.getMonth()+1) + "-" + end.getDate();
            workEvents.length = 0;
            buildMonth(startMonth);
            if ((start.getMonth() + 1)!=end.getMonth())
                buildMonth(thisMonth);
            buildMonth(endMonth);
            var events = workEvents;
            callback(events);
        };
        

        
        $scope.eventSource = {
            url: "http://www.google.com/calendar/feeds/en.jewish%23holiday%40group.v.calendar.google.com/public/basic", //TO ADD HOLIDAYS
            backgroundColor: '#D25FD2',
            className: 'gcal-event',
        };
        
        /* event sources array*/
        $scope.eventSources = [$scope.events, $scope.eventSource, $scope.eventsF];

        /* alert on eventClick */

        $scope.alertEventOnClick = function (date) {
            
            var time = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
            $http.get('/api/WorkManageApi/GetDay/date?=' + time).success(function (data) {
                $scope.day = data;
                $scope.selectedDay = $scope.day.Date.split('T')[0];
                buildProjects();
            });
        };


        /* config calendar */
        $scope.uiConfig = {
            calendar: {
                height: 650,
                selectable: true,
                weekMode: 'variable',
                timeFormat: 'H:mm',
                header: {
                    left: 'title',
                    center: '',
                    right: 'today prev,next'
                },
                dayClick: $scope.alertEventOnClick
            }
        };
        
        /* config project select */
        $scope.selcetUi = {
            multiple: true,
            data: [],
            placeholder: "Select a Project",
            createSearchChoice: function(term, data) {
                if ($(data).filter(function() { return this.text.localeCompare(term) === 0; }).length === 0) {
                    return { id: term, text: term };
                }
            }
        };

        /* Build project to fit select view */
        function buildProjects() {
            var counter = projectsCounter;
            projects.length = 0;
            var temp = [];
            angular.forEach($scope.day.timeinfo.Projects, function (project, index) {
                projects.push({
                    id: counter++,
                    text: project,
                });
            });
            angular.copy($scope.day.timeinfo.Projects, temp);
            angular.copy(projects, $scope.day.timeinfo.Projects);
            
            /* choose only the outside projects that not already exist */
            angular.forEach(outsideProject, function (project, index) {
                if (temp.indexOf(project.text) == -1)
                    projects.push(project);
            });
            angular.copy(projects, $scope.selcetUi.data);
        }

        /* Build month to fit event view */
        function buildMonth(date) {
            $http.get('api/WorkManageApi/GetMonth/monthReturn?=' + date).success(function (data) {
                angular.forEach(data.Days, function (day) {
                    angular.forEach(day.TimeInfos, function (timeinfo) {
                        debugger;
                        var startHour = day.Date.split('T')[0]+ 'T' + timeinfo.StartTime + ':00';
                        if (timeinfo.Activity == "Work") {
                            workEvents.push({ title: timeinfo.Activity, start: startHour, end: day.Date, allDay: false });
                            if (timeinfo.Projects != '')
                                workEvents[workEvents.length-1].title += " - " + timeinfo.Projects;
                            if (timeinfo.Description != null)
                                workEvents[workEvents.length-1].title += " - " + timeinfo.Description;
                        }
                        else if (timeinfo.Activity == "HalfHoliday") {
                            
                            workEvents.push({ title: timeinfo.Activity, start: startHour, end: day.Date, backgroundColor: '#1DD300', allDay: false });
                        }                  
                        else
                            workEvents.push({ title: timeinfo.Activity, start: day.Date, end: day.Date, backgroundColor: '#FF7C00' });
                        });
                });
            });      
        }
          
    });