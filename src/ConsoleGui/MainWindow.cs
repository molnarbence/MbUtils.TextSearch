using Terminal.Gui;

namespace ConsoleGui;

public class MainWindow : Window
{   
    public MainWindow(SearchFormViewModel searchFormViewModel, StatisticsViewModel statisticsViewModel)
    {
        Title = "Text Search App";

        var searchForm = new SearchFormFrameView(searchFormViewModel)
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = 7
        };
        
        // Frame view for statistics
        var statisticsFrame = new StatisticsFrameView(statisticsViewModel)
        {
            X = 0,
            Y = Pos.Bottom(searchForm) + 1,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        
        Add(searchForm, statisticsFrame);
    }
}