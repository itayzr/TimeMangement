
myApp.controller('WorkManagerCtrl', function ($scope, $http, $dialog) {
       
        var workEvents = [];
        var projects = [];
        var outsideProject = [];
        var projectsCounter = 1;
              
        /* first load*/
        $http.get('/api/WorkManageApi/GetDay/date?=').success(function (data) {
            loadOutsideProj();
            $scope.day = data;
            $scope.timeInfo = $scope.day.TimeInfos[0];
            $scope.selectedDay = $scope.day.Date.split('T')[0];
            buildProjects();
        });


        /*change day from the timepicker*/
        $scope.ChangeDay = function () {
            $http.get('/api/WorkManageApi/GetDay/date?=' + $scope.selectedDay).success(function(data) {
                $scope.day = data;
                $scope.timeInfo = $scope.day.TimeInfos[0];
                buildProjects();
                var spiltdate=$scope.selectedDay.split('-');
                $scope.myCalendar.fullCalendar('gotoDate', spiltdate[0],spiltdate[1]-1,spiltdate[2]);
            }); 
        };
        
        $scope.AddActivity = function () {
            if ($scope.day.TimeInfos.length!=0)
                if ($scope.day.TimeInfos[0].StartTime == null || $scope.day.TimeInfos[0].Activity=="Holiday")
                    $scope.timeInfo = $scope.day.TimeInfos[0];
                else {
                    var start = $scope.day.TimeInfos[$scope.day.TimeInfos.length - 1].FinishTime;
                    $scope.day.TimeInfos.push({ StartTime: start, FinishTime: "", Activity: "Work", Projects: Array[0], Description: null });
                    $scope.timeInfo = $scope.day.TimeInfos[$scope.day.TimeInfos.length - 1];
                }        
        };

        $scope.DelActivity = function () {
            var start = $scope.timeInfo.StartTime;
            for (var i = 0; i < $scope.day.TimeInfos.length; i++)
                if ($scope.day.TimeInfos[i].StartTime === start) {
                    $scope.day.TimeInfos.splice(i, 1);
                    break;
                }
            if ($scope.day.TimeInfos.length == 0) {
                $http.post("/api/WorkManageApi/Delete/dayInput?=", $scope.day).success(function () {
                    $scope.myCalendar.fullCalendar('refetchEvents');
                        $http.get('/api/WorkManageApi/GetDay/date?=' + $scope.selectedDay).success(function (data) {
                            $scope.day = data;
                            $scope.timeInfo = $scope.day.TimeInfos[0];
                            buildProjects();
                    });
                });
            }
            else
            {
                Save();
                $scope.timeInfo = $scope.day.TimeInfos[$scope.day.TimeInfos.length-1];
            }
        };
        
        $scope.showDel = function () {
            if ($scope.timeInfo != null) {
                if ($scope.timeInfo.StartTime !=null)
                    return true;
            }
            return false;
        };
    
        $scope.showHours = function () {
            var result = true;
            if ($scope.timeInfo != null) {
                if ($scope.timeInfo.Activity == "Holiday" || $scope.timeInfo.Activity == "HalfHoliday") {
                    $scope.timeInfo.StartTime = "00:00";
                    $scope.timeInfo.FinishTime = "00:00";
                    $scope.timeInfo.Project = ({
                        id: 0,
                        text: ''
                    });
                    result = false;
                }
            }
            return result;
        };
        
        function Save() {
            if ($scope.timeInfo.Project != null)
                $scope.timeInfo.Project = $scope.timeInfo.Project.text;
            $http.post("/api/WorkManageApi/Save/dayInput?=", $scope.day).success(function () {
                $scope.myCalendar.fullCalendar('refetchEvents');
                buildProjects();
            });
        };

        $scope.submit = function () {
            
            if ($scope.timeInfo.Project!=null)
                $scope.timeInfo.Project = $scope.timeInfo.Project.text;
            $http.post("/api/WorkManageApi/Save/dayInput?=", $scope.day).success(function () {
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

        $scope.daySelect = function (date) {
            var time = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
            $http.get('/api/WorkManageApi/GetDay/date?=' + time).success(function (data) {
                $scope.day = data;
                $scope.selectedDay = $scope.day.Date.split('T')[0];
                $scope.timeInfo = $scope.day.TimeInfos[0];
                $scope.AddActivity();
                buildProjects();
            });
        };


        /* config calendar */
        $scope.uiConfig = {
            calendar: {
                height: 650,
                selectable: true,
                theme: true,
                weekMode: 'variable',
                timeFormat: 'H:mm',
                header: {
                    left: 'title',
                    center: '',
                    right: 'prev,next'
                },
                dayClick: $scope.daySelect,
                eventClick: function (event) {
                    var date = event.start;
                    var hours = date.getHours() < 10 ? '0' + date.getHours() : date.getHours();
                    var minutes = date.getMinutes() < 10 ? '0' + date.getMinutes() : date.getMinutes();
                    var start = hours+':'+minutes;
                    var time = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
                    $http.get('/api/WorkManageApi/GetDay/date?=' + time).success(function (data) {             
                        $scope.day = data;
                        $scope.selectedDay = $scope.day.Date.split('T')[0];
                        angular.forEach($scope.day.TimeInfos, function (timeInfo) {
                            if (timeInfo.Activity == "Holiday")
                                $scope.timeInfo = timeInfo;
                            else {
                                if(timeInfo.StartTime==start)
                                    $scope.timeInfo = timeInfo;
                            }
                        });
                        buildProjects();
                    });
                }
            }
          
        };

     
        /* config project select */
        $scope.selcetUi = {
            multiple: false,
            data: [],
            placeholder: "Select a Project",
            createSearchChoice: function(term, data) {
                if ($(data).filter(function() { return this.text.localeCompare(term) === 0; }).length === 0) {
                    return { id: term, text: term };
                }
            }
        };

        function buildProjects() {
            var counter = projectsCounter;
            var temp = '';
            projects.length = 0;
            if ($scope.timeInfo.Project != null) {
               temp = $scope.timeInfo.Project;
                projects.push({
                    id: counter++,
                    text: $scope.timeInfo.Project,
                });
                $scope.timeInfo.Project = projects[0];
            }
            /* choose only the outside projects that not already exist */
            angular.forEach(outsideProject, function (project) {
                if (temp!=project.text)
                    projects.push(project);
            });
            angular.copy(projects, $scope.selcetUi.data);
        }
        

        /* Build month to fit event view */
        function buildMonth(date) {
            $http.get('/api/WorkManageApi/GetMonth/monthReturn?=' + date).success(function (data) {
                angular.forEach(data.Days, function (day) {
                    angular.forEach(day.TimeInfos, function (timeinfo) {
                        var startHour = day.Date.split('T')[0]+ 'T' + timeinfo.StartTime + ':00';
                        if (timeinfo.Activity == "Work") {
                            workEvents.push({ title: timeinfo.Activity, start: startHour, end: day.Date, allDay: false });
                            if (timeinfo.Project != '')
                                workEvents[workEvents.length-1].title += " - " + timeinfo.Project;
                            if (timeinfo.Description != null)
                                workEvents[workEvents.length-1].title += " - " + timeinfo.Description;
                        }
                        else if (timeinfo.Activity == "HalfHoliday") {
                            
                            workEvents.push({ title: timeinfo.Activity, start: day.Date, end: day.Date, backgroundColor: '#1DD300', allDay: false });
                        }                  
                        else
                            workEvents.push({ title: timeinfo.Activity, start: day.Date, end: day.Date, backgroundColor: '#FF7C00' });
                        });
                });
            });      
        }

        function loadOutsideProj() {
            $http.get('/api/WorkManageApi/GetProjects').success(function (data) {
                outsideProject.length = 0;
                projectsCounter = 1;
                angular.forEach(data, function (project) {
                    outsideProject.push({
                        id: projectsCounter++,
                        text: project,
                    });
                });
                angular.extend($scope.selcetUi.data, outsideProject);
            });
        }
    
        var t = '<div class="modal-header">' +
                 '<h3>Description editor</h3>' +
                 '</div>' +
                 '<div class="modal-body">' +
                 '<p> <textarea ng-model="result" class="text-area-editor" autofocus=""></textarea></p>' +
                 '</div>' +
                 '<div class="modal-footer">' +
                 '<button ng-click="close(result)" class="btn btn-primary" >Apply</button>' +
                 '</div>';

        $scope.opts = {
            backdrop: true,
            keyboard: true,
            backdropClick: true,
            template: t, 
            controller: 'TestDialogController'
        };

        $scope.openEditor = function () {
            description = $scope.timeInfo.Description;
            var d = $dialog.dialog(angular.extend($scope.opts));
            d.open().then(function (result) {
                $scope.timeInfo.Description = result;
            });
        };

});

function TestDialogController($scope, dialog) {
    $scope.result = description;
    $scope.close = function (result) {
        dialog.close(result);
    };
};
