Ext.define('Epsitec.cresus.webcore.ErrorHandler', {
  alternateClassName: ['Epsitec.ErrorHandler'],

  statics: {
    handleError: function(response) {
      var json, errors, title, message;

      try {
        json = Ext.decode(response.responseText);
      }
      catch (e) {
        json = null;
      }

      errors = json !== null ? json.errors : null;

      if (errors !== null)
      {
        title = errors.title || null;
        message = errors.message || null;
        if (title !== null && message !== null) {
          Epsitec.ErrorHandler.handleErrorTitleAndMessage(title, message);
          return;
        }
      }

      Epsitec.ErrorHandler.handleErrorDefault();
    },

    handleErrorTitleAndMessage: function(title, message) {
      Ext.Msg.alert(title, message);
    },

    handleErrorDefault: function() {
      var title = 'Error',
          content = 'Something wrong happened and you shouldn\'t have seen ' +
                    'this message if only I did my job properly!';
      Ext.Msg.alert(title, content);
    }
  }
});
