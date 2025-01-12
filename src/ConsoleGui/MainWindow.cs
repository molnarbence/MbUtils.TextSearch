using Core;
using Core.Strategies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Terminal.Gui;

namespace ConsoleGui;

public class MainWindow : Window
{
    private readonly StatisticsFrameView _statisticsFrame;
    
    public static string SearchTerm = string.Empty;
    public MainWindow()
    {
        Title = "Text Search App";
        
        // Create input components and labels
        var searchTermLabel = new Label
        {
            Text = "Search Term:"
        };
        
        var searchTermText = new TextField
        {
            // Position text field adjacent to the label
            X = Pos.Right(searchTermLabel) + 1,

            // Fill remaining horizontal space
            Width = Dim.Fill()
        };
        
        var inputFolderLabel = new Label
        {
            Text = "Input Folder:", 
            X = Pos.Left(searchTermLabel), 
            Y = Pos.Bottom (searchTermLabel) + 1
        };
        var inputFolderText = new TextField
        {
            // align with the text box above
            X = Pos.Left(searchTermText),
            Y = Pos.Top(inputFolderLabel),
            Width = Dim.Fill()
        };
        
        // Create login button
        var btnSearch = new Button
        {
            Text = "Search",
            Y = Pos.Bottom (inputFolderLabel) + 1,

            // center the login button horizontally
            X = Pos.Center (),
            IsDefault = true
        };
        
        // Frame view for statistics
        _statisticsFrame = new StatisticsFrameView(new StatisticsViewModel())
        {
            X = 0,
            Y = Pos.Bottom(btnSearch) + 1,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        
        btnSearch.Accept += (_, _) =>
        {
            OnSearchClicked(searchTermText.Text, inputFolderText.Text);
        };

        Add(searchTermLabel, searchTermText, inputFolderLabel, inputFolderText, btnSearch, _statisticsFrame);
    }

    private void OnSearchClicked(string searchTerm, string inputFolderPath)
    {
        var options = Options.Create(new AppConfig
        {
            OutputFilePath = @"D:\searchtermsout\output.csv"
        });

        var loggerFactory = new LoggerFactory();
        
        var resultRepositoryLogger = new Logger<FileBasedResultRepository>(loggerFactory);
        var mainLogic = new MainLogic(new RegexStrategy(), new FileBasedResultRepository(resultRepositoryLogger, options), options);
        
        var statistics = mainLogic.Search(inputFolderPath, searchTerm);
        _statisticsFrame.UpdateStatistics(statistics);
    }
}