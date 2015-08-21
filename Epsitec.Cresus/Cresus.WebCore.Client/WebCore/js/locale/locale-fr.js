// This file defines the locale text values for the french language.

Ext.define('Epsitec.cresus.webcore.locale.Locale', {
  alternateClassName: ['Epsitec.Locale'],

  /* Static methods */

  statics: {
    getLocaleName: function() {
      return 'fr';
    },

    getTexts: function() {
      return {
        globalWarning: '<i><b>ATTENTION:</b> Les modifications ' +
            'effectu\u00E9es ici seront r\u00E9percut\u00E9es dans tous les ' +
            'enregistrements.</i>',
        sortLabel: 'Trier',
        exportLabel: 'Export',
        exportCsvLabel: 'Fichier CSV',
        exportLabelLabel: 'Etiquettes',
        exportLabelText: 'Texte des \u00E9tiquettes',
        exportLabelLayout: 'Format des \u00E9tiquettes',
        exportReport: 'Rapport',
        exportReportText: 'Choix du rapport',
        exportReportLabel: 'Rapports',
        searchLabel: 'Rechercher',
        refreshLabel: 'Rafra\u00EEchir',
        refreshTip: 'Rafra\u00EEchir',
        pinToBag: 'Ajouter au panier',
        addLabel: 'Ajouter',
        addTip: 'Ajoute un nouvel \u00E9l\u00E9ment',
        removeLabel: 'Enlever',
        removeTip: 'Enl\u00E8ve cet \u00E9l\u00E9ment',
        createLabel: 'Cr\u00E9er',
        deleteLabel: 'D\u00E9truire',
        resetLabel: 'R\u00E9initialiser',
        saveLabel: '<b>Enregistrer</b>',
        okLabel: 'Ok',
        cancelLabel: 'Annuler',
        emptySummaryText: 'Vide',
        emptyItemText: '\u2014', // This is the long dash character.
        nullItemText: '&#160;', // This is the non breakable space (&nbsp;).
        emptyListText: 'Vide',
        loadingText: '...',
        summaryHeader: 'R\u00E9sum\u00E9',
        entityPickerTitle: 'S\u00E9lection d\'entit\u00E9s',
        errorTitle: 'Erreur',
        errorMessage: 'Une erreur est survenue lors de la communication avec ' +
            'le serveur distant.',
        serverErrorMessage: 'Une erreur est survenue sur le serveur distant',
        jsonErrorMessage: 'Une erreur est survenue lors du d\u00E9codage de ' +
            'la r\u00E9ponse du serveur distant.',
        errorBusinessTitle: 'Erreure de validation',
        loginTitle: 'Connexion \u00E0 AIDER',
        loginLabel: 'Connexion',
        usernameLabel: 'Nom d\'utilisateur',
        passwordLabel: 'Mot de passe',
        databasesTitle: 'Bases de donn\u00E9es',
        toolsTitle: 'Outils',
        aboutLabel: 'A propos',
        logoutLabel: 'Termin\u00E9',
        sortTitle: 'Configuration du tri',
        scopeTitle: 'Cercles',
        windowTitle: 'AIDER',
        warningTitle: 'Attention',
        entityCreationWarningMessage: 'Un filtre est actif. L\'entit\u00E9 ' +
            'que vous allez cr\u00E9er ne sera pas affich\u00E9e dans la ' +
            'liste si elle ne correspond pas \u00E0 ces crit\u00E8res.' +
            'Voulez-vous quand m\u00EAme la cr\u00E9er?',
        entitySelectionErrorMessage: 'L\'entit\u00E9 ' +
            'ne peut pas \u00EAtre affich\u00E9e dans la liste.',
        entityRemoveWarningMessage: 'Voulez-vous l\'enlever de la liste?',
        pickerFavouriteItems: 'Favoris',
        pickerAllItems: 'Liste compl\u00E8te',
        exportTitle: 'Configuration de l\'Export',
        exportExportedColumns: 'Colonnes export\u00E9es',
        exportDiscardedColumns: 'Colonnes ignor\u00E9es',
        exportImpossibleTitle: 'Export impossible',
        exportImpossibleEmpty: 'Vous ne pouvez pas exporter une liste vide.',
        exportImpossibleTooMany: 'Vous ne pouvez pas exporter une liste avec ' +
            'plus de 10000 elements.',
        exportImpossibleTooManyForBag: 'Vous ne pouvez pas ajouter une liste ' +
            'si grande au panier.',
        timeFilterBefore: 'Avant',
        timeFilterOn: 'A',
        timeFilterAfter: 'Apr\u00E8s'
      };
    }
  }
});
