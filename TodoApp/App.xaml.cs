namespace TodoApp;

public partial class App : Application
{
    public App(MainPage mainPage)
    {
        InitializeComponent();
        MainPage = new NavigationPage(mainPage)
        {
            BarBackgroundColor = Color.FromArgb("#512BD4"),
            BarTextColor = Colors.White
        };
        
        // Definir tema padrão
        UserAppTheme = AppTheme.Light;
    }
}
