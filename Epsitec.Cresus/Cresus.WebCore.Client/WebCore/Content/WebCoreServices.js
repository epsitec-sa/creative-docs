angular.module('webCore.Services', [])
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