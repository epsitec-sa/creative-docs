var mod = angular.module("webCore.Downloads", ['webCore.Services']);

mod.controller("CoreDownloads", ['$scope', 'webCoreServices', function ($scope, webCoreServices) {

    $scope.getDownloads = function () {
        webCoreServices.downloads().success(function (data, status, headers) {
            $scope.downloads = data;
        });
    }

    $scope.deleteDownload = function (filename) {
        webCoreServices.deleteDownload(filename).success(function (data, status, headers) {
            $scope.getDownloads();
        });
    }
    
}]);


mod.directive("downloadsView", function () {
    return {
        restrict: "E",
        templateUrl: "/proxy/content/modules/Downloads/view.html"
    };
});






