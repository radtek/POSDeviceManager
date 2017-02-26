using System.ComponentModel;
using System.Configuration.Install;

namespace DevmanSvc
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