/// <reference path="../lib/angular/angular.js" />
/// <reference path="../lib/angular/angular.min.js" />

(function () {
    "use strict";
    angular.module("app-trips")
    .controller("tripsController", tripsController);

    function tripsController($http) {
        var vm = this;

        vm.trips = [];

        vm.newTrip = {};
        vm.errorMessage = "";
        vm.isBusy = true;
        $http.get("/api/trips")
        .then(function (response) {
            //success
            angular.copy(response.data, vm.trips);

        },function(error){
            vm.errorMessage = "Failed to load data: " + error;        
        })
        .finally(function() {
            vm.isBusy = false;
            vm.errorMessage = "";
            });
        vm.addTrip = function () {
            vm.isBusy = true;
            $http.post("/api/trips", vm.newTrip)
            .then(function (response) {
                //Success
                vm.trips.push(response.data);
                vm.newTrip = {};
            }, function (response) {
                //failure
                vm.errorMessage = "Failed to save new trip";
            })
            .finally(function () {
                vm.isBusy = false;
            });

        };

    }
})();