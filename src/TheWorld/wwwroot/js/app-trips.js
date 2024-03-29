﻿/// <reference path="../lib/angular/angular.js" />
/// <reference path="../lib/angular/angular.min.js" />


(function () {
    "use strict";

    angular.module("app-trips", ["simpleControls", "ngRoute"])
           .config(function ($routeProvider) {

               $routeProvider.when("/", {
                   controller: "tripsController",
                   controllerAs: "vm",
                   templateUrl: "/views/tripsView.html"
               });
               $routeProvider.when("/editor/:tripName", {
                   controller: "tripEditorController",
                   controllerAs: "vm",
                   templateUrl: "/views/tripEditorView.html"
               });
               $routeProvider.otherwise({
                   redirectTo: "/"
               });
           });

})();