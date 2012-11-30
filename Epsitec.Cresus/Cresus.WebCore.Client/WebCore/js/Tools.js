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

      processProxyError: function(response) {
        var status, success;

        status = response.status;

        // The http status codes from 200 to 299 are the http status codes for
        // valid http responses.
        success = status >= 200 && status <= 299;

        return this.processResponse(success, response);
      },

      processResponse: function(success, response) {
        var json;

        if (!success) {
          Epsitec.ErrorHandler.handleError();
          return null;
        }

        json = this.decodeResponse(response);
        if (json === null) {
          return null;
        }

        if (!json.success) {
          Epsitec.ErrorHandler.handleFailure(json.content);
          return null;
        }

        return json;
      },

      decodeResponse: function(response) {
        if (this.isUndefined(response.responseText)) {
          return null;
        }

        try {
          return Ext.decode(response.responseText);
        }
        catch (e) {
          Epsitec.ErrorHandler.handleJsonError();
          return null;
        }
      },

      createUrl: function(base, parameters) {
        var url, i, parameter, separator;

        url = base;

        for (i = 0; i < parameters.length; i += 1) {
          parameter = parameters[i];
          separator = i === 0 ? '?' : '&';
          url += separator + parameter[0] + '=' + parameter[1];
        }

        return url;
      }
    }
  });
});
