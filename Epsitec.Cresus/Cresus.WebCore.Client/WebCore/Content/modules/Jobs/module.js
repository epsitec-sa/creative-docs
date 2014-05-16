var mod = angular.module("webCore.Jobs", ['webCore.Services']);

mod.controller("CoreJobs", ['$scope', 'webCoreServices', function ($scope, webCoreServices) {

    $scope.getJobs = function () {
        webCoreServices.jobs().success(function (data, status, headers) {
            $scope.jobs = data;
        });
    }

    $scope.cancelJob = function (id) {
        webCoreServices.cancelJob(id).success(function (data, status, headers) {
            $scope.getJobs();
        });
    }

}]);


mod.directive("jobsView", function () {
    return {
        restrict: "E",
        templateUrl: "/content/modules/Jobs/view.html"
    };
});






