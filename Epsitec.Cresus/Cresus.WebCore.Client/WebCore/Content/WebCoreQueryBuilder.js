var app = angular.module("webCoreQueryBuilder", [
  "ngSanitize",
  "webCore.Services",
  "webCore.QueryBuilder"
]);

app.controller('CoreQueryBuilder', ['$scope', '$location', 'webCoreServices',
  function($scope, $location, webCoreServices) {
    var druid = $location.path().replace('/', '');
    $scope.fields = [];
    $scope.columns = [];
    $scope.availableQueries = [];

    webCoreServices.loadQueries(druid).success(function(data, status, headers) {
      $scope.availableQueries = data.content;
    });

    webCoreServices.database(druid).success(function(data, status, headers) {
      $scope.database = data.content;
      $scope.filter = {
        group: {
          operator: {
            name: 'ET',
            value: 'and'
          },
          rules: []
        }
      };

      angular.forEach($scope.database.columns, function(column) {

        if (column.filter.filterable) {
          if (column.hidden === false) {
            $scope.columns.push(column);
          }

          $scope.fields.push({
            name: column.title,
            id: column.name,
            type: column.type.type,
            enumId: column.type.enumerationName
          });
        }
      });

      $scope.ready = true;
    });

    $scope.addSelectedColumn = function(column) {
      $scope.columns.push(column);
    };

    $scope.removeColumn = function(column) {
      var index = $scope.columns.indexOf(column);
      $scope.columns.splice(index, 1);
    };

    $scope.testQuery = function() {
      $scope.displayTotal = false;
      var query = [];
      query.push($scope.filter.group);
      var columns = $scope.columns.map(function(c) {
        return c.name;
      }).join(';');

      webCoreServices.query(druid, columns, JSON.stringify(query)).success(
        function(data, status, headers) {
          $scope.total = data.content.total;
          $scope.displayTotal = true;
          $scope.json = JSON.stringify(data.content, ' ', 2);
        });
    };

    $scope.saveQuery = function (queryName) {
      var query = [];
      query.push($scope.filter.group);

      var columns = $scope.columns.map(function(c) {
        return c.name;
      }).join(';');

      webCoreServices.saveQuery(druid, queryName, columns, JSON.stringify(query)).success(function() {
        webCoreServices.loadQueries(druid).success(function(data, status, headers) {
          $scope.availableQueries = data.content;
        });
      });
    };

    function htmlEntities(str) {
      return String(str).replace(/</g, '&lt;').replace(/>/g, '&gt;');
    }

    function computedForHuman(group) {
      if (!group) return "";
      for (var str = "(", i = 0; i < group.rules.length; i++) {
        i > 0 && (str += " <strong>" + group.operator.name + "</strong> ");
        str += group.rules[i].group ?
          computedForHuman(group.rules[i].group) :
          group.rules[i].field.name + " " + htmlEntities(group.rules[i].comparison) +
          " " + group.rules[i].value;
      }

      return str + ")";
    }

    function computedQuery(group) {
      if (!group) return "";
      for (var str = "{'op':'" + group.operator.value + "'", i = 0; i < group
        .rules.length; i++) {
        str += group.rules[i].group ?
          computedQuery(group.rules[i].group) :
          "{field':'" + group.rules[i].field.id + "','comparison':'" + group.rules[
            i].comparison + "','value':' " + group.rules[i].value + "'}";
      }

      return str + "}";
    }

    $scope.json = null;

    $scope.$watch('filter', function(newValue) {
      if (newValue !== undefined) {
        $scope.humanOutput = computedForHuman(newValue.group);
      }
    }, true);
  }
]);
