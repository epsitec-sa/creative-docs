var app = angular.module("webCoreAdmin", ['webCoreAdmin.services']);

angular.module('webCoreAdmin.services', [])
  .factory('webCoreServices', ['$http', function ($http) {

      var getJobs = function () {
          return $http({
              method: 'GET',
              url: '/proxy/jobs/list'
          });
      }

      var cancelJob = function (id) {
          return $http({
              method: 'GET',
              url: '/proxy/jobs/cancel/' + id
          });
      }

      var getDownloads = function () {
          return $http({
              method: 'GET',
              url: '/proxy/downloads/list'
          });
      }

      var deleteDownload = function (filename) {
          return $http({
              method: 'GET',
              url: '/proxy/downloads/delete/' + filename
          });
      }

      return {
          jobs: function () { return getJobs(); },
          cancelJob: function (id) { return cancelJob(id); },
          downloads: function () { return getDownloads(); },
          deleteDownload: function (filename) { return deleteDownload(filename); },
      };
  }]);

app.controller("CoreJobs", ['$scope','webCoreServices',function ($scope, webCoreServices) {

    $scope.getJobs = function () {
        webCoreServices.jobs().success(function (data, status, headers) {
            $scope.jobs = data;
        });
    }

    $scope.cancelJob = function (id) {
        webCoreServices.cancel(id).success(function (data, status, headers) {
            $scope.getJobs();
        });
    }

}]);

app.controller("Downloads", ['$scope', 'webCoreServices', function ($scope, webCoreServices) {

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







