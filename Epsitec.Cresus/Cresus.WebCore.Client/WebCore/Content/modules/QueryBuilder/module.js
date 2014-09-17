var queryBuilder = angular.module("webCore.QueryBuilder", ['webCore.Services']);

queryBuilder.directive('queryBuilder', ['$compile','webCoreServices', function ($compile, webCoreServices) {
  return {
    restrict: 'E',
    scope: {
        group: '=',
    },
    templateUrl: '/content/modules/QueryBuilder/view.html',
    compile: function (element, attrs) {
      var content, directive;
      content = element.contents().remove();
      return function (scope, element, attrs) {
        scope.operators = [
            { name: 'ET' },
            { name: 'OU' }
        ];

        scope.fields = scope.group.fields;

        scope.conditions = [
            { name: '=' },
            { name: '<>' },
            { name: '<' },
            { name: '<=' },
            { name: '>' },
            { name: '>=' }
        ];

        scope.addCondition = function () {
            scope.group.rules.push({
                field: '',
                condition: '=',
                type: 'string',
                data: ''
            });
        };

        scope.removeCondition = function (index) {
            scope.group.rules.splice(index, 1);
        };

        scope.addGroup = function () {
            scope.group.rules.push({
                group: {
                    fields: scope.fields,
                    operator: 'ET',
                    rules: []
                }
            });
        };

        scope.setType = function (rule) {
          angular.forEach(scope.fields, function (field)
          {
            if(field.id == rule.field.id) {
              rule.type = field.type;

              if(rule.type === 'list') {
                webCoreServices.fieldValues(field.enumId).success(function (data, status, headers) {
                  rule.possibleValues = data.content.values;
                });
              }
              else {
                rule.possibleValues = null;
              }
            }
          });
        };

        scope.removeGroup = function () {
            "group" in scope.$parent && scope.$parent.group.rules.splice(scope.$parent.$index, 1);
        };

        directive || (directive = $compile(content));

        element.append(directive(scope, function ($compile) {
            return $compile;
        }));
      }
    }
  }
}]);
