Ext.define('Epsitec.cresus.webcore.Locale', {
  alternateClassName: ['Epsitec.Locale'],
  statics: {
    getLocaleName: function() {
      return 'en';
    },

    getTexts: function() {
      return {
        globalWarning: '<i><b>WARNING:</b> The modifications made here will ' +
            'be propagated to every record.</i>',
        sortLabel: 'Sort',
        refreshLabel: 'Refresh',
        refreshTip: 'Refresh',
        addLabel: 'Add',
        addTip: 'Add a new item',
        removeLabel: 'Remove',
        removeTip: 'Remove this item',
        createLabel: 'Create',
        deleteLabel: 'Delete',
        resetLabel: 'Reset',
        saveLabel: 'Save',
        okLabel: 'Ok',
        cancelLabel: 'Cancel',
        emptySummaryText: 'Empty',
        emptyItemText: '\u2014', // This is the long dash character.
        nullItemText: '&#160;', // This is the non breakable space (&nbsp;).
        emptyListText: 'Nothing to display',
        loadingText: '...',
        summaryHeader: 'Summary',
        entityPickerTitle: 'Entity selection',
        errorTitle: 'Error',
        errorMessage: 'An unexpected error occured while communicating with ' +
            'the remote server',
        serverErrorMessage: 'An unexpected error occured on the remote server',
        jsonErrorMessage: 'An error occured while decoding the answer from ' +
            'the remote server',
        loginTitle: 'Cr\u00E9sus.Core Login',
        loginLabel: 'Log in',
        usernameLabel: 'Username',
        passwordLabel: 'Password',
        databasesTitle: 'Databases',
        toolsTitle: 'Options',
        aboutLabel: 'About',
        logoutLabel: 'Logout',
        sortTitle: 'Sort selection',
        scopeTitle: 'Scopes',
        windowTitle: 'Cr\u00E9sus.Core',
        warningTitle: 'Warning',
        entityCreationWarningMessage: 'A filter is active. The entity you ' +
            'are about to create will not appear in the entity list, if it ' +
            'does not satisfy the filter condition. Do you want to create it?',
        entitySelectionErrorMessage: 'The entity that you have created ' +
            'cannot be displayed in the list, probably because of a filter.'
      };
    }
  }
});
