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
    $scope.selectableColumns = [];

    var loadAvailableQueries = function() {
      webCoreServices.loadQueries(druid).success(function(data, status,
        headers) {

        data.shift();
        $scope.availableQueries = data;
      });
    };

    loadAvailableQueries();

    webCoreServices.database(druid).success(function(data, status, headers) {
      $scope.database = data.content;
      $scope.filter = {
        group: {
          operator: 'and',
          rules: []
        }
      };

      angular.forEach($scope.database.columns, function(column) {
        //populate displayed/displayable columns
        if (column.hidden === false) {
          $scope.columns.push(column);
        } else {
          $scope.selectableColumns.push(column);
        }

        //populate filterable
        if (column.filter.filterable) {
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
      var index = $scope.selectableColumns.indexOf(column);
      $scope.selectableColumns.splice(index, 1);
    };

    $scope.removeColumn = function(column) {
      $scope.selectableColumns.push(column);
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

    $scope.saveQuery = function(queryName) {
      var query = [], columns;

      columns = $scope.columns.map(function(c) {
        return c.name;
      }).join(';');

      $scope.filter.group.columns = columns;

      query.push($scope.filter.group);

      webCoreServices.saveQuery(druid, queryName, columns, JSON.stringify(
        query)).success(function() {
          loadAvailableQueries();
        });
    };

    $scope.deleteQuery = function(queryName) {
      webCoreServices.deleteQuery(druid, queryName).success(function() {
        loadAvailableQueries();
      });
    };

    $scope.loadQuery = function(query) {
      var group = JSON.parse(query)[0];
      var columnsId = group.columns.split(';');
      var toRemove = [];
      var toAdd = [];

      //clear lists
      $scope.columns.splice(0, $scope.columns.length);
      $scope.selectableColumns.splice(0, $scope.selectableColumns.length);
      //for each database columns
      angular.forEach($scope.database.columns, function(col) {
        //if column is in query
        if (columnsId.indexOf(col.name) !== -1) {
          //add column to displayed
          $scope.columns.push(col);
        } else {
          //add column to available
          $scope.selectableColumns.push(col);
        }
      });

      var filter = {
        group: group
      };

      $scope.filter = filter;
    };

    function htmlEntities(str) {
      return String(str).replace(/</g, '&lt;').replace(/>/g, '&gt;');
    }

    function computedForHuman(group) {
      if (!group) return "";
      for (var str = "(", i = 0; i < group.rules.length; i++) {

        if (i > 0) {
          str += " <strong>" + group.operator.name + "</strong> ";
        }
        if (group.rules[i].group) {
          str += computedForHuman(group.rules[i].group);
        } else {
          str += group.rules[i].field +
                " " +
                htmlEntities(group.rules[i].comparison) +
                " " +
                group.rules[i].value;
        }
      }

      return str + ")";
    }

    $scope.json = null;

    $scope.$watch('filter', function(newValue) {
      if (newValue !== undefined) {
        $scope.humanOutput = computedForHuman(newValue.group);
      }
    }, true);
  }
]);
