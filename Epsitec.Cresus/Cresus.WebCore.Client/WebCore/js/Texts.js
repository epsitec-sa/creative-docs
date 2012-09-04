Ext.define('Epsitec.cresus.webcore.Texts', {
  alternateClassName: ['Epsitec.Texts'],

  statics: {
    getGlobalWarning: function() {
      return '<i><b>ATTENTION:</b> Les modifications effectu\u00E9es ici ' +
             'seront r\u00E9percut\u00E9es dans tous les enregistrements.</i>';
    },

    getSortLabel: function() {
      return 'Sort';
    },

    getRefreshLabel: function() {
      return 'Refresh';
    },

    getRefreshTip: function() {
      return 'Refresh';
    },

    getAddLabel: function() {
      return 'Add';
    },

    getAddTip: function() {
      return 'Add a new item';
    },

    getRemoveLabel: function() {
      return 'Remove';
    },

    getRemoveTip: function() {
      return 'Remove this item';
    },

    getCreateLabel: function() {
      return 'Create';
    },

    getDeleteLabel: function() {
      return 'Delete';
    },

    getResetLabel: function() {
      return'Reset';
    },

    getSaveLabel: function() {
      return 'Save';
    },

    getOkLabel: function() {
      return 'Ok';
    },

    getCancelLabel: function() {
      return 'Cancel';
    },

    getEmptySummaryText: function() {
      return 'Empty';
    },

    getEmptyItemText: function() {
      // This is the long dash character.
      return '\u2014';
    },

    getNullItemText: function() {
      // This is the non breakable space (&nbsp;).
      return '&#160;';
    },

    getEmptyListText: function() {
      return 'Nothing to display';
    },

    getLoadingText: function() {
      return '...';
    },

    getSummaryHeader: function() {
      return 'Summary';
    },

    getEntityPickerTitle: function() {
      return 'Entity selection';
    },

    getErrorTitle: function() {
      return 'Error';
    },

    getErrorMessage: function() {
      return 'An unexpected error occured while communicating with the ' +
             'remote server';
    },

    getServerErrorMessage: function() {
      return 'An unexpected error occured on the remote server';
    },

    getJsonErrorMessage: function() {
      return 'An error occured while decoding the answer from the remote ' +
             'server';
    },

    getLoginTitle: function() {
      return 'Cr\u00E9sus.Core Login';
    },

    getLoginLabel: function() {
      return 'Log in';
    },

    getUsernameLabel: function() {
      return 'Username';
    },

    getPasswordLabel: function() {
      return 'Password';
    },

    getDatabasesTitle: function() {
      return 'Databases';
    },

    getToolsTitle: function() {
      return 'Options';
    },

    getAboutLabel: function() {
      return 'About';
    },

    getLogoutLabel: function() {
      return 'Logout';
    },

    getSortTitle: function() {
      return 'Sort selection';
    }
  }
});
