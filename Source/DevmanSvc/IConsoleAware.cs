namespace DevmanSvc
{
    internal interface IConsoleAware
    {
        void StartApplication(string[] args);

        void StopApplication();
    }
}
