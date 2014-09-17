var app = angular.module("webCoreQueryBuilder",
    [
        "ngSanitize",
        "webCore.Services",
        "webCore.QueryBuilder"
    ]);

app.controller('CoreQueryBuilder', ['$scope', '$location', 'webCoreServices', function ($scope, $location, webCoreServices) {
    var druid = $location.path().replace('/','');

    webCoreServices.database(druid).success(function (data, status, headers) {
        $scope.database = data.content;
        $scope.filter   = {
          group : {
            fields : [],
            operator : 'ET',
            rules : []
          }
        };

        angular.forEach ($scope.database.columns, function (column) {
          $scope.filter.group.fields.push ({
            name : column.title,
            id   : column.name,
            type : column.type.type,
            enumId : column.type.enumerationName
          });

        });
    });



    function htmlEntities(str) {
        return String(str).replace(/</g, '&lt;').replace(/>/g, '&gt;');
    }

    function computedForHuman(group) {
        if (!group) return "";
        for (var str = "(", i = 0; i < group.rules.length; i++) {
            i > 0 && (str += " <strong>" + group.operator + "</strong> ");
            str += group.rules[i].group ?
                computedForHuman(group.rules[i].group) :
                group.rules[i].field.name + " " + htmlEntities(group.rules[i].condition) + " " + group.rules[i].data;
        }

        return str + ")";
    }

    function computedQuery(group) {
        if (!group) return "";
        for (var str = "[", i = 0; i < group.rules.length; i++) {
            i > 0 && (str += " " + group.operator + " ");
            str += group.rules[i].group ?
                computedQuery(group.rules[i].group) :
                group.rules[i].field.id + " " + group.rules[i].condition + " " + group.rules[i].data;
        }

        return str + "]";
    }

    $scope.json = null;

    $scope.$watch('filter', function (newValue) {
        if(newValue !== undefined) {
          $scope.json = JSON.stringify(newValue, null, 2);
          $scope.humanOutput = computedForHuman(newValue.group);
          $scope.queryOutput = computedQuery(newValue.group);
        }
    }, true);
}]);
