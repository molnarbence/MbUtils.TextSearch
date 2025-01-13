using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using Terminal.Gui;

namespace ConsoleGui;

public class SearchFormFrameView : FrameView, IViewFor<SearchFormViewModel>
{
    private readonly CompositeDisposable _disposable = [];
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (SearchFormViewModel)value!;
    }

    public SearchFormViewModel? ViewModel { get; set; }

    public SearchFormFrameView(SearchFormViewModel viewModel)
    {
        ViewModel = viewModel;
        Title = "Search";
        
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

        ViewModel
            .WhenAnyValue(x => x.SearchTerm)
            .BindTo(searchTermText, txt => txt.Text)
            .DisposeWith(_disposable);

        searchTermText
            .Events()
            .TextChanged
            .Select(_ => searchTermText.Text)
            .DistinctUntilChanged()
            .BindTo(ViewModel, vm => vm.SearchTerm)
            .DisposeWith(_disposable);
        
        inputFolderText
            .Events()
            .TextChanged
            .Select(_ => inputFolderText.Text)
            .DistinctUntilChanged()
            .BindTo(ViewModel, vm => vm.InputFolderPath)
            .DisposeWith(_disposable);

        btnSearch
            .Events()
            .Accept
            .InvokeCommand(ViewModel.Search)
            .DisposeWith(_disposable);
        
        Add(searchTermLabel, searchTermText, inputFolderLabel, inputFolderText, btnSearch);
    }
    
    protected override void Dispose (bool disposing)
    {
        _disposable.Dispose();
        base.Dispose(disposing);
    }
}