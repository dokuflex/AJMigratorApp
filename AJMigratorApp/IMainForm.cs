namespace AJMigratorApp
{
    public interface IMainForm
    {
        void SetProgressInfo(string text);
        void StartProgress();
        void StopProgress();
    }
}