namespace CardGame.Console
{
    /// <summary>
    ///     This class is part of the composition root and is used to store specific string values.
    /// </summary>
    internal static class Constants
    {
        private static string appBasePath;

        static Constants()
        {
            appBasePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
        }

        public static string NullValue { get; } = "~NULL~";

        public static string SettingsFileName { get; } = "settings.xml";

        public static string SettingsFilePath
        {
            get
            {
                return appBasePath + SettingsFileName;
            }
        }

        public static string ConfigFileName { get; } = "CardGame.config";

        public static string ConfigFilePath
        {
            get
            {
                return appBasePath + ConfigFileName;
            }
        }
    }
}