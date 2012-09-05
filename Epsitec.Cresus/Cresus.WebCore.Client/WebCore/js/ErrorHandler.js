Ext.require([
  'Epsitec.cresus.webcore.Texts'
],
function() {
  Ext.define('Epsitec.cresus.webcore.ErrorHandler', {
    alternateClassName: ['Epsitec.ErrorHandler'],

    statics: {
      handleError: function() {
        var title = Epsitec.Texts.getErrorTitle(),
            message = Epsitec.Texts.getErrorMessage();
        this.showError(title, message);
      },

      handleFormError: function(action) {
        switch (action.failureType) {
          case Ext.form.action.Action.CONNECT_FAILURE:
          case Ext.form.action.Action.LOAD_FAILURE:
            this.handleError();
            break;

          case Ext.form.action.Action.CLIENT_INVALID:
          case Ext.form.action.Action.SERVER_INVALID:
            // Nothing to do here, the form is automatically marked as invalid.
            break;
        }
      },

      handleJsonError: function() {
        var title = Epsitec.Texts.getErrorTitle(),
            message = Epsitec.Texts.getJsonErrorMessage();
        this.showError(title, message);
      },

      handleFailure: function(failure) {
        var title, message;

        if (failure === null)
        {
          this.handleDefaultFailure();
          return;
        }

        title = failure.title || null;
        message = failure.message || null;

        if (title === null || message === null) {
          this.handleDefaultFailure();
          return;
        }

        this.showError(title, message);
      },

      handleDefaultFailure: function() {
        var title = Epsitec.Texts.getErrorTitle(),
            message = Epsitec.Texts.getServerErrorMessage();
        this.showError(title, message);
      },

      showError: function(title, message) {
        Ext.MessageBox.show({
          icon: Ext.MessageBox.ERROR,
          title: title,
          msg: message,
          buttons: Ext.MessageBox.OK
        });
      }
    }
  });
});
