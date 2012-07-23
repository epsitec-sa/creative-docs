Ext.define('Epsitec.cresus.webcore.ErrorHandler', {
  alternateClassName: ['Epsitec.ErrorHandler'],

  statics: {
    handleError: function(response) {

      var responseData = Ext.decode(response.responseText);
      var errors = responseData.errors;

      var code = errors.code || null;
      var title = errors.title || null;
      var message = errors.message || null;

      if (code !== null) {
        Epsitec.ErrorHandler.handleErrorCode(code);
      }
      else if (title !== null && message !== null) {
        Epsitec.ErrorHandler.handleErrorTitleAndMessage(
            title, message
        );
      }
      else {
        Epsitec.ErrorHandler.handleErrorDefault();
      }
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
      var title = 'Error';
      var content = 'Something wrong happened and you shouldn\'t have seen ' +
                    'this message if only I did my job properly!';
      Ext.Msg.alert(title, content);
    },

    handleSessionTimeout: function() {
      Ext.Msg.alert(
          'Session timout.',
          'Your session has timed out. Please log in again.',
          function() { window.location.reload(); }
      );
    }
  }
});
