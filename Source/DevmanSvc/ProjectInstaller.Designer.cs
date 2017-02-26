namespace DevmanSvc
{
    partial class ProjectInstaller
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.devmanProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.devmanInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // devmanProcessInstaller
            // 
            this.devmanProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.devmanProcessInstaller.Password = null;
            this.devmanProcessInstaller.Username = null;
            // 
            // devmanInstaller
            // 
            this.devmanInstaller.Description = "Предоставляет интерфейсы управления POS-терминальными устройствами";
            this.devmanInstaller.DisplayName = "Диспетчер POS-устройств";
            this.devmanInstaller.ServiceName = "POSDeviceManager";
            this.devmanInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.devmanProcessInstaller,
            this.devmanInstaller});

		}

		#endregion

		private System.ServiceProcess.ServiceProcessInstaller devmanProcessInstaller;
        private System.ServiceProcess.ServiceInstaller devmanInstaller;
	}
}