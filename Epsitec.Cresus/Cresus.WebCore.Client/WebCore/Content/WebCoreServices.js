angular.module('webCore.Services', [])
  .factory('webCoreServices', ['$http', function ($http) {

      var getJobs = function () {
          return $http({
              method: 'GET',
              url: '/proxy/jobs/list'
          });
      };

      var cancelJob = function (id) {
          return $http({
              method: 'GET',
              url: '/proxy/jobs/cancel/' + id
          });
      };

      var getDownloads = function () {
          return $http({
              method: 'GET',
              url: '/proxy/downloads/list'
          });
      };

      var deleteDownload = function (filename) {
          return $http({
              method: 'GET',
              url: '/proxy/downloads/delete/' + filename
          });
      };

      var getDatabaseList = function () {
        return $http({
            method: 'GET',
            url: '/proxy/database/list'
        });
      };

      var getDatabaseDefinition = function (druid) {
        return $http({
            method: 'GET',
            url: '/proxy/database/definition/' + druid
        });
      };

      var getFieldValues = function (fieldId) {
        return $http({
            method: 'GET',
            url: '/proxy/enum/get/' + fieldId
        });
      };

      var testQuery = function (druid, query) {
        return $http({
            method: 'GET',
            url: '/proxy/database/get/' + druid + '?start=0&limit=100&page=1&query=' + query
        });
      };


      return {
          jobs: function () { return getJobs(); },
          cancelJob: function (id) { return cancelJob(id); },
          downloads: function () { return getDownloads(); },
          deleteDownload: function (filename) { return deleteDownload(filename); },
          databases : function () { return getDatabaseList();},
          database : function(druid) { return getDatabaseDefinition (druid); },
          fieldValues : function(id) { return getFieldValues (id); },
          query : function(druid, query) { return testQuery (druid, query); },
      };
  }]);
