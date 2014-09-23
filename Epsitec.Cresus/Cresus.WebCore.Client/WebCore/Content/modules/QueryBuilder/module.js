var queryBuilder = angular.module("webCore.QueryBuilder", ['webCore.Services']);

queryBuilder.directive('queryBuilder', ['$compile', 'webCoreServices',
  function($compile, webCoreServices) {
    return {
      restrict: 'E',
      scope: {
        group: '=',
        fields: '=',
      },
      templateUrl: '/content/modules/QueryBuilder/view.html',
      compile: function(element, attrs) {
        var content, directive;
        content = element.contents().remove();
        return function(scope, element, attrs) {

          scope.operators = [{
            name: 'ET',
            value: 'and'
          }, {
            name: 'OU',
            value: 'or'
          }];

          scope.enum = {};

          scope.comparators = [{
            name: 'égal a',
            value: 'eq'
          }, {
            name: 'pas égal a',
            value: 'nq'
          }, {
            name: 'plus petit que',
            value: 'lt'
          }, {
            name: 'plus grand que',
            value: 'gt'
          }];

          scope.addCondition = function() {
            scope.group.rules.push({
              field: '',
              comparison: 'eq',
              type: 'string',
              value: ''
            });
          };

          scope.removeCondition = function(index) {
            scope.group.rules.splice(index, 1);
          };

          scope.addGroup = function() {
            scope.group.rules.push({
              group: {
                operator: {
                  name: 'ET',
                  value: 'and'
                },
                rules: []
              }
            });
          };

          scope.setType = function(rule) {
            angular.forEach(scope.fields, function(field) {
              if (field.id == rule.field.id) {
                rule.type = field.type;

                if (rule.type === 'list') {
                  webCoreServices.fieldValues(field.enumId).success(
                    function(data, status, headers) {
                      scope.enum[field.enumId] = data.content.values;
                    });
                }
              }
            });
          };

          scope.pushSelectedValue = function(rule) {
            rule.value = [];
            rule.value.push(rule.selectedValue);
          };

          scope.removeGroup = function() {
            "group" in scope.$parent && scope.$parent.group.rules.splice(
              scope.$parent.$index, 1);
          };

          directive || (directive = $compile(content));

          element.append(directive(scope, function($compile) {
            return $compile;
          }));
        };
      }
    };
  }
]);
