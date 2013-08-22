google.load("visualization", "1", { packages: ["corechart"] });

myApp.controller('MonthStatCtrl', function($scope, $http) {

    var date = new Date;
    var month = (date.getMonth() + 1) < 10 ? '0' + (date.getMonth() + 1) : (date.getMonth() + 1);
    $scope.selecedMonth = date.getFullYear() + "-" + month;

    $scope.changeMonth = function() {
        var workMonth = $scope.selecedMonth;
        $http.get('/api/WorkManageApi/GetProjectsHours/' + $scope.employee + '/' + workMonth).success(function(data) {
            $scope.totalMonth = 0;
            var datainit = [];
            angular.forEach(data, function(hours, prj) {
                datainit.push({ Projcet: prj, Hours: hours.toFixed(2) });
                $scope.totalMonth = $scope.totalMonth + hours;
            });
            $scope.myData = datainit;
            $scope.totalMonth = $scope.totalMonth.toFixed(2);
        });
    };

    $scope.btnTable = {
        label: "Table view",
        state: false
    };

    $scope.chartSwitch = function () {
        if ($scope.chart != null) {
            this.btnTable.state = !this.btnTable.state;
            if (this.btnTable.state == false) {
                debugger;
                this.btnTable.label = "Chart view";
                $scope.chart.type = "Table";
                $scope.chart.options.width = 900;
            }
            else {
                this.btnTable.label = "Table view";
                $scope.chart.type = "ColumnChart";
                $scope.chart.options.width = 1150;
            }
        }
    };
    
    $scope.btnTotal = {
        label: "Total",
        state: false
    };

    $scope.totalSwitch = function () {
        if ($scope.chart != null) {
            this.btnTotal.state = !this.btnTotal.state;
            if (this.btnTotal.state == false) {
                debugger;
                this.btnTotal.label = "Per Project";
                $scope.chart.data = { "cols": $scope.dataTotal.H, "rows": $scope.dataTotal.K };
                $scope.title = "Month total hours";
            }
            else {
                this.btnTotal.label = "Total";
                $scope.chart.data = { "cols": $scope.data.H, "rows": $scope.data.K };
                $scope.title = "Projects hours per month";
            }
        }
    };

    $scope.next = function () {

        var year = parseInt($scope.selecedMonth.split('-')[0]);
        month = parseInt($scope.selecedMonth.split('-')[1]);
        if (month == 12) {
            year++;
            month = "01";
        }
        else {
            month++;
            month = month < 10 ? '0' + month : month;
        }
        $scope.selecedMonth = year + '-' + month;
        $scope.changeMonth();
    };
    
    $scope.prev = function () {
        var year = parseInt($scope.selecedMonth.split('-')[0]);
        month = parseInt($scope.selecedMonth.split('-')[1]);
        if (month == 1) {
            year--;
            month = "12";
        }
        else {
            month--;
            month = month < 10 ? '0' + month : month;
        }
        $scope.selecedMonth = year + '-' + month;
        $scope.changeMonth();
    };

    $scope.changeEmployee = function () {
        $scope.changeMonth();
        buildChart();
    };


    $scope.gridOptions = {
        data: 'myData',
        enableSorting: true,
        enableRowSelection: false,
        jqueryUITheme: true,
    };


    $http.get('/api/WorkManageApi/GetEmployeesList').success(function (data) {
        var datainit = [];
        angular.forEach(data, function (emp) {
            datainit.push(emp);
        });
        $scope.employees = datainit;
    });



    function buildChart() {
        var workYear = $scope.selecedMonth.split('-')[0];
        $scope.btnTable.label = "Table view";
        $scope.btnTable.state = false;
        $scope.btnTotal.label = "Total";
        $scope.btnTotal.state = false;
        var chart1 = {};
        chart1.type = "ColumnChart";
        chart1.displayed = true;
        $scope.title = "Projects hours per month";
        chart1.options = {
            "isStacked": "false",
            "width": 1150,
            "fill": 10,
            "animation":{
                   "duration": 2000,
                    "easing": 'out',
            },
            "chartArea": {
                "left":50
            },
            "displayExactValues": true,
            "vAxis": {
                "gridlines": { "count": 10 }
            },
            "hAxis": {
                "title": "Month",
                "textStyle":{"fontSize": 11}
    }
        };

        $scope.data = new google.visualization.DataTable();
        $scope.dataTotal = new google.visualization.DataTable();
        var dt = [];
        var totalHours = [];
        $scope.data.addColumn('string', 'Month');
        $scope.dataTotal.addColumn('string', 'Month');
        $scope.dataTotal.addColumn('number', 'Total');
        var month = new Array();
        month[0] = "January";
        month[1] = "February";
        month[2] = "March";
        month[3] = "April";
        month[4] = "May";
        month[5] = "June";
        month[6] = "July";
        month[7] = "August";
        month[8] = "September";
        month[9] = "October";
        month[10] = "November";
        month[11] = "December";
        for (var i = 0; i < 12; i++) {    
            dt.push([month[i]]);
            totalHours.push([month[i]]);
        }
        for (var i = 0; i < 12; i++) {
            totalHours[i].push(0);
        }
        $http.get('/api/WorkManageApi/GetProjects/emp?=' + $scope.employee).success(function (projects) {
            var flag = projects.length;
            angular.forEach(projects, function (prj) {
                $http.get('/api/WorkManageApi/GetProjectHourPerMonth/' +workYear + '/' + $scope.employee + '/' + prj).success(function (prjHours) {
                    $scope.data.addColumn('number', prj);
                    for (var i = 0; i < 12; i++) {
                        dt[i].push(0);
                    }
                    angular.forEach(prjHours, function (hours, month) {
                        var index = parseInt(month) - 1;
                        dt[index][dt[index].length - 1] = hours;
                        totalHours[index][1] = totalHours[index][1] + hours;
                    });
                    flag--;
                    if (flag == 0) {
                        $scope.data.addRows(dt);
                        $scope.dataTotal.addRows(totalHours);
                        chart1.data = { "cols": $scope.data.H, "rows": $scope.data.K };
                        $scope.chart = chart1;
                        $scope.chart.options.title = $scope.title + " - " + workYear + " - " + $scope.employee;
                    }
                });
            });

        });
    }
    

});




