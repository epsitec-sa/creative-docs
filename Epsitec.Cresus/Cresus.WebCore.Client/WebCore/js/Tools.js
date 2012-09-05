Ext.require([
  'Epsitec.cresus.webcore.ErrorHandler'
],
function() {
  Ext.define('Epsitec.cresus.webcore.Tools', {
    alternateClassName: ['Epsitec.Tools'],

    statics: {
      isUndefined: function(item) {
        return typeof item === 'undefined';
      },

      isArrayEmpty: function(array) {
        return array && array.length === 0;
      },

      getValueOrDefault: function(value, defaultValue) {
        return this.isUndefined(value) ?
            defaultValue : value;
      },

      getValueOrNull: function(value)  {
        return this.getValueOrDefault(value, null);
      },

      doCallback: function(callback, context, callbackArguments) {
        if (this.isUndefined(callback) || callback === null) {
          return;
        }

        callback.apply(context, callbackArguments);
      },

      processResponse: function(success, response) {
        var json;

        if (!success) {
          Epsitec.ErrorHandler.handleError();
          return null;
        }

        try {
          json = Ext.decode(response.responseText);
        }
        catch (e) {
          Epsitec.ErrorHandler.handleJsonError();
          return null;
        }

        if (!json.success) {
          Epsitec.ErrorHandler.handleFailure(json.content);
          return null;
        }

        return json;
      }
    }
  });
});
