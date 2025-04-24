using System.Windows;
using WpfTargetProject.Models;

namespace WpfTargetProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            var p = new Employee
            {
                Name = "David"
            };

            // Ensure the App.xaml file exists and is properly linked to the project.
            WpfTargetProject.App app = new WpfTargetProject.App();
            app.InitializeComponent(); // This initializes the XAML resources.
            app.Run();
        }
    }
}