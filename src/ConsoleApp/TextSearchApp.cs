using Core;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;

namespace ConsoleApp;

[Command(Name = "Text Search App", Description = "Search for text in files")]
public class TextSearchApp
{
    [Argument(0, Name = "Input folder path")] 
    public required string InputFolderPath { get; set; }
    
    [Argument(1, Name = "Search term")] 
    public required string SearchTerm { get; set; }
    
    private void OnExecute(
        MainLogic mainLogic)
    {
        mainLogic.Search(InputFolderPath, SearchTerm);
    }
}