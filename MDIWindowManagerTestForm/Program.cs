namespace MDIWindowManagerTestForm
{
    internal static class Program
    {
        // UseMdiWindowManager is used in the Advanced Example for toggling MDIWindowManager on/off and
        // reloading the form. In normal usage, you would set a config setting and reload the application.
        public static bool UseMdiWindowManager { get; set; } = true;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            Application.SetColorMode(SystemColorMode.System);
#pragma warning disable WPF0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            Application.Run(new SimpleMdiForm());
        }
    }
}