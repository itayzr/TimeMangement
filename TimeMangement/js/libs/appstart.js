

var myApp = angular.module('WorkManagmentApp', ['ui.calendar', 'ui.select2', 'ngGrid', 'googlechart.directives', 'ui.bootstrap'], function ($dialogProvider) {
    $dialogProvider.options({ backdropClick: false, dialogFade: true });
    ;
});

var description;