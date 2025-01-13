using System.ComponentModel;
using System.Runtime.Serialization;
using Core;
using Core.Strategies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace ConsoleGui;

[DataContract]
public partial class SearchFormViewModel : ReactiveObject
{
    [DataMember, Reactive] private string _searchTerm = string.Empty;
    [DataMember, Reactive] private string _inputFolderPath = string.Empty;
    
    [IgnoreDataMember]
    public ReactiveCommand<HandledEventArgs, Statistics> Search { get; }
    
    public SearchFormViewModel()
    {
        var options = Options.Create(new AppConfig
        {
            OutputFilePath = @"D:\searchtermsout\output.csv"
        });

        var loggerFactory = new LoggerFactory();
        
        var resultRepositoryLogger = new Logger<FileBasedResultRepository>(loggerFactory);
        var mainLogic = new MainLogic(new RegexStrategy(), new FileBasedResultRepository(resultRepositoryLogger, options), options);

        Search = ReactiveCommand.CreateRunInBackground<HandledEventArgs, Statistics>
        (
            e => mainLogic.Search(InputFolderPath, SearchTerm)
        );
    }
}