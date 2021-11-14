public partial class LocalizationsEditor
{
    private static class LocalizationsEditorTexts
    {
        #region General
        public static string WINDOW_TITLE = "Localizations Editor";
        public static string CONTINUE = "Continue";
        public static string DELETE = "Delete";
        public static string CANCEL = "Cancel";
        public static string DONE = "Done!";
        public static string CLEAR = "Clear";
        public static string SAVE = "Save";
        public static string NO_RESULTS = "There's no results...";
        #endregion
        
        #region Errors
        public static string ERROR_REPEATED_KEY = "The key you want to create already exist!";
        public static string ERROR_EMPTY_KEY = "The key you want to create is empty.";
        public static string ERROR_KEY_HAS_WHITESPACES = "The key you want to create has whitespaces. Replace them with underscores to avoid reading issues.";
        public static string ERROR_GROUP_NOT_DEFINED = "The key group has not been defined.";
        public static string ERROR_SUBGROUP_NOT_DEFINED = "The key sub group has not been defined.";
        public static string ERROR_PRIMARY_LANGUAGE_EMPTY = "The Primary Language (default) text is not defined.";
        #endregion
        
        #region Toolbar
        public static string TOOLBAR_CREATE = "Create";
        public static string TOOLBAR_SETTINGS = "Settings";
        public static string TOOLBAR_IMPORT_EXPORT = "Import/Export";
        public static string TOOLBAR_ANALYZE = "Analyze";
        public static string TOOLBAR_SEARCH_USAGES= "Search Usages";
        public static string TOOLBAR_BUILD_ASSETS = "Build Assets";
        public static string TOOLBAR_PING = "Ping";
        public static string TOOLBAR_HELP = "Help";
        public static string TOOLBAR_PING_WRAPPER = "Ping Wrapper";
        public static string TOOLBAR_PING_BUILT_ASSETS = "Ping Built Assets";
        public static string TOOLBAR_NEW_LOC = "New Localization";
        #endregion

        #region Sidebar
        public static string SIDEBAR_SEARCH = "Search:";
        public static string SIDEBAR_NO_KEYS = "There's no Keys...";
        public static string SIDEBAR_NO_GROUPS = "There's no Groups...";
        #endregion
        
        #region Setup
        public static string INIT_TAB_NAME = "Initial Setup";
        public static string INIT_TAB_DESC = "Select the language that will be supported by the game. \nPrimary Language determines the default language of the game and\n"
                                             + "the game will start with this language when nothing is settled or\n"
                                             + "when the player language is not found on the available languages.\n"
                                             + "(Primary and available languages can be modified in the future)";
        public static string INIT_AVAILABLE_LANGS = "Available Languages";
        public static string INIT_AVAILABLE_LANGS_DESC = "Select the languages that will be available in the game.";
        public static string INIT_START_BUTTON = "Start";
        #endregion

        #region Create
        public static string CREATE_TAB_TITLE = "Create Key";
        public static string CREATE_KEY = "Key:";
        public static string CREATE_DESC = "Description:";
        public static string CREATE_NEW_GROUP = "New Group";
        public static string CREATE_NEW_SUBGROUP = "New SubGroup";
        public static string CREATE_GROUP = "Group:";
        public static string CREATE_SUBGROUP = "SubGroup:";
        public static string CREATE_LANG= "Language:";
        public static string CREATE_TRANS= "Translation:";
        public static string CREATE_EMPTY_FIELDS= "Empty fields will display the raw key when called by undefined languages.";
        public static string CREATE_BUTTON= "Create Key";
        #endregion
        
        #region Explorer
        public static string EXPLORER_KEY = "Key:";
        public static string EXPLORER_USAGES = "Usages:";
        public static string EXPLORER_SEARCH_USAGES = "Search Usages";
        public static string EXPLORER_COPY_KEY = "Copy Key";
        public static string EXPLORER_KEY_EXISTS = "This key already exists!";
        public static string EXPLORER_DESCRIPTION = "Description: " ;
        public static string EXPLORER_GROUP = "Group: ";
        public static string EXPLORER_SUBGROUP = "Subgroup: ";
        public static string EXPLORER_LANG = "Language:";
        public static string EXPLORER_TRANSLATION = "Translation:";
        public static string EXPLORER_NO_USAGES = "There's no usages!";
        public static string EXPLORER_USAGES_NOT_SEARCHED = "Usages weren't searched yet.";
        #endregion
        
        #region Import/Export
        public static string IE_TAB_TITLE = "Import/Export";
        public static string IE_TAB_DESC = "Import or Export all the information of the localizations into CSV format for readability on database programs.";
        public static string IE_EXPORT = "Export";
        public static string IE_EXPORT_DESC = "Select a folder to export all the localizations to a CSV file.";
        public static string IE_EXPORT_BUTTON = "Export to CSV";
        public static string IE_GENERATE_BASE = "Generate Spreadsheet Base";
        public static string IE_GENERATE_BASE_DESC = "Create a CSV file with the base needed to edit localizations on an external software.";
        public static string IE_GENERATE_BASE_BUTTON = "Generate CSV Base";
        public static string IE_SAVE_CSV_PANEL_TITLE = "Save CSV Localization File";
        public static string IE_FILE_CREATED = "File Created!";
        public static string IE_FILE_CREATED_BODY = "The CSV file was created successfully on the path: \n";
        public static string IE_VIEW_FILE = "View File";
        public static string IE_IMPORT = "Import";
        public static string IE_IMPORT_DESC = "Select a CSV File to Import new Localizations.";
        public static string IE_IMPORT_NOTE = "Note: File should match the required format or will be ignored.";
        public static string IE_IMPORT_METHOD = "Import Method:";
        public static string IE_IMPORT_BUTTON = "Import CSV File";
        public static string IE_IMPORT_SELECT_METHOD_TITLE = "Select Import Method";
        public static string IE_IMPORT_SELECT_METHOD_BODY = "Choose the Import Method you want to use to import new keys from a CSV File.";
        public static string IE_IMPORT_SEARCH_FILE_TITLE = "Import CSV Localizations File";
        public static string IE_IMPORTING_CSV_HEADER = "Importing Localizations from CSV File...";
        public static string IE_IMPORTING_CSV_READING = "Reading CSV File...";
        public static string IE_IMPORTING_CSV_PARSING = "Parsing Elements...";
        public static string IE_IMPORTING_CSV_EMPTY_KEY = "There's an input with an empty key";
        public static string IE_IMPORTING_CSV_IMPORTING_KEY = "Importing ";
        public static string IE_IMPORTING_SUCCESS_TITLE = "CSV File Imported!";
        public static string IE_IMPORTING_SUCCESS_BODY = "Localizations where updated from the CSV file.";
        #endregion

        #region Analysis Tab
        public static string ANALYSIS_TAB_NAME = "Analyze";
        public static string ANALYSIS_TAB_DESCRIPTION = "Show missing elements and issues inside the localizations data.";

        public static string ANALYSIS_NOT_TRANSLATED_ERROR = "The Key has languages without translation!";
        public static string ANALYSIS_EMPTY_GROUPS_ERROR = "The Group is Empty!";
        public static string ANALYSIS_EMPTY_SUBGROUPS_ERROR = "The SubGroup is Empty!";
        public static string ANALYSIS_REPEATED_ELEMENTS_ERROR = "The Key is repeated and will be overriden!";
        public static string ANALYSIS_ELEMENTS_WITHOUT_USE_ERROR = "The Key doesn't have usages inside the project.";
        
        public static string ANALYSIS_NOT_TRANSLATED = "Not Translated Keys";
        public static string ANALYSIS_EMPTY_GROUPS = "Empty Groups";
        public static string ANALYSIS_EMPTY_SUBGROUPS = "Empty SubGroups";
        public static string ANALYSIS_REPEATED_ELEMENTS = "Repeated Elements";
        public static string ANALYSIS_ELEMENTS_WITHOUT_USE = "Elements without use";
        
        public static string ANALYZING_PROGRESS_TITLE = "Analysing Localizations Data";
        public static string ANALYZING_PROGRESS_GROUP = "Analysing Group {0}";
        public static string ANALYZING_PROGRESS_KEY = "Analysing Key {0}";
        public static string ANALYZING_SEARCH_CRITERIA = "Search Criteria:";
        #endregion

        #region Language
        public static string LANG_TAB_TITL = "Languages";
        public static string LANG_TAB_DESC = "Select the languages that will be available in the game.";
        public static string LANG_PRIMARY_LANGUAGE = "";
        public static string LANG_PRIMARY_LABEL = " (Primary)";
        public static string LANG_APPLY_BUTTON = "Apply Languages";
        public static string LANG_CHANGE_TITLE = "Change Languages?";
        public static string LANG_CHANGE_BODY = "New languages will be added on all the keys and removed one will be removed on all the keys! This has no turning back. Do you want to continue?";
        public static string LANG_MODIFIED_TITLE = "Languages Modified!";
        public static string LANG_MODIFIED_BODY = "All keys are modified with the new Languages configuration!";
        public static string LANG_CHANGES_WILL_APPLY = "Changes will apply to all existing localizations and removed languages will be erased from the database!";
        #endregion
        
        #region Try Ping

        public static string TRYPING_NO_ASSETS = "No Assets";
        public static string TRYPING_NO_BUILT_ASSETS_YET = "There's no built assets yet.";

        #endregion
        
        #region Try Erase
        public static string TRYERASE_TITLE = "Erase Key?";
        public static string TRYERASE_BODY = "The key and all its translations will be deleted. Do you want to continue";
        
        #endregion

        #region Search All Usages
        public static string SEARCHALL_TITLE = "Search all Usages";

        public static string SEARCHALL_BODY ="This option will search all the usages for every key registered! \nThis can take time depending on the project size and the amount of keys. \nDo you want to continue?";
        public static string SEARCHING_TITLE = "Searching Usages...";
        public static string SEARCHING_BODY = "Searching usages of key {0}";
        public static string SEARCHDONE_TITLE = "Search Done";
        public static string SEARCHDONE_BODY = "Usages were already searched. You can find the usages of each key inside of its data.";
        #endregion

        #region Build Assets
        public static string BUILDASSETS_NOLOCS_TITLE = "There's no Localizations";
        public static string BUILDASSETS_NOLOCS_BODY = "There's no content to build the Localization Assets.";
        public static string BUILDASSETS_BUILDING_TITLE = "Building Localization Assets";
        public static string BUILDASSETS_BUILDING_BODY = "Creating Instance for {0}...";
        public static string BUILDASSETS_PARSING_BODY = "Parsing key {0} ({1} of {2})";
        public static string BUILDASSETS_CREATINGASSET_BODY = "Creating Asset for {0}";
        public static string BUILDASSETS_ASSETSCREATED_BODY = "Localization Assets have been created successfully.";
        #endregion

    }
}
