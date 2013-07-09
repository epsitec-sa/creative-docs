Ext.require([
  'Epsitec.cresus.webcore.tools.ErrorHandler'
],
function() {
  Ext.define('Epsitec.cresus.webcore.tools.Tools', {
    alternateClassName: ['Epsitec.Tools'],

    statics: {
      getValueOrDefault: function(value, defaultValue) {
        return Ext.isDefined(value) ? value : defaultValue;
      },

      getValueOrNull: function(value)  {
        return this.getValueOrDefault(value, null);
      },

      doCallback: function(callback, context, callbackArguments) {
        if (!Ext.isDefined(callback) || callback === null) {
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
        try {
          return this.decodeResponseInternal(response);
        }
        catch (e) {
          Epsitec.ErrorHandler.handleJsonError();
          return null;
        }
      },

      tryDecodeRespone: function(response) {
        try {
          return {
            success: true,
            data: this.decodeResponseInternal(response)
          };
        }
        catch (e) {
          return {
            success: false
          };
        }
      },

      decodeResponseInternal: function(response) {
        var text = response.responseText;

        if (!Ext.isDefined(text)) {
          return null;
        }

        return Ext.decode(text);
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
      },

      addParameterToUrl: function(url, key, value) {
        var separator = url.indexOf('?') >= 0 ? '&' : '?';

        return url + separator + key + '=' + value;
      }
    }
  });
});
