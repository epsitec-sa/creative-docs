var formatNumber = function(num, length) {
    var r = '' + num;
    while (r.length < length) {
        r = '0' + r;
    }
    return r;
};

var queryBuilder = angular.module("webCore.QueryBuilder", ['webCore.Services']);
queryBuilder.directive('parseDate', function(){
    return {
        require: '?ngModel',
        link: function(scope, element, attr, ngModel) {
            $(element).updatePolyfill();
            ngModel.$formatters.push(function(value){
              var tokens = value.split ('.');
              var day    = parseInt(tokens[0], 10);
              var month  = parseInt(tokens[1], 10) - 1;
              var year   = parseInt(tokens[2], 10);
              var date   = new Date (year, month, day);
              return date;
            });
            ngModel.$parsers.push(function(value){
                var day   = formatNumber (value.getDate (), 2);
                var month = formatNumber (value.getMonth () + 1, 2);
                var year  = value.getFullYear ();
                var date  = day + '.' + month + '.' + year;
                return date;
            });
        }
    };
});

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
          scope.fieldEnumMap = {};
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

          // prefetch all enum values
          angular.forEach(scope.fields, function(field) {
            if (field.enumId) {
              webCoreServices
                .fieldValues (field.enumId)
                .success (function (data, status, headers) {
                  scope.fieldEnumMap[field.druid] = field.enumId;
                  scope.enum[field.enumId] = data.content.values;
                });
            }
          });

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
            scope.group.operator = scope.operators[0].value;
            scope.group.rules.push({
              group: {
                operator: scope.operators[0].value,
                rules: []
              }
            });
          };

          scope.setType = function(rule) {
            angular.forEach(scope.fields, function(field) {
              if (field.druid === rule.field) {
                rule.type = field.type;
              }
            });
          };

          scope.pushSelectedValue = function(rule) {
            rule.value = [];
            rule.value.push(rule.selectedValue);
          };

          scope.removeGroup = function() {
            return 'group' in scope.$parent && scope.$parent.group.rules.splice(
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
