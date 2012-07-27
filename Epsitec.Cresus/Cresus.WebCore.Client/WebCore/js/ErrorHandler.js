Ext.define('Epsitec.cresus.webcore.ErrorHandler', {
  alternateClassName: ['Epsitec.ErrorHandler'],

  statics: {
    handleError: function(response) {
      var json, errors, code, title, message;

      try {
        json = Ext.decode(response.responseText);
      }
      catch (e) {
        json = null;
      }

      errors = json !== null ? json.errors : null;

      if (errors !== null)
      {
        code = errors.code || null;
        if (code !== null) {
          Epsitec.ErrorHandler.handleErrorCode(code);
          return;
        }

        title = errors.title || null;
        message = errors.message || null;
        if (title !== null && message !== null) {
          Epsitec.ErrorHandler.handleErrorTitleAndMessage(title, message);
          return;
        }
      }

      Epsitec.ErrorHandler.handleErrorDefault();
    },

    handleErrorCode: function(code) {
      switch (code) {
        case '0':
          Epsitec.ErrorHandler.handleSessionTimeout();
          break;

        default:
          Epsitec.ErrorHandler.handleErrorDefault();
      }
    },

    handleErrorTitleAndMessage: function(title, message) {
      Ext.Msg.alert(title, message);
    },

    handleErrorDefault: function() {
      var title = 'Error',
          content = 'Something wrong happened and you shouldn\'t have seen ' +
                    'this message if only I did my job properly!';
      Ext.Msg.alert(title, content);
    },

    handleSessionTimeout: function() {
      var title = 'Session timout.',
          content = 'Your session has timed out. Please log in again.',
          callback = function() { window.location.reload(); };
      Ext.Msg.alert(title, content, callback);
    }
  }
});
