using System.ComponentModel;
using System.Configuration.Install;

namespace TsManagerSvc
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}