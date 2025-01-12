using System.Reactive.Disposables;
using Core;
using ReactiveUI;
using Terminal.Gui;

namespace ConsoleGui;

public class StatisticsFrameView : FrameView, IViewFor<StatisticsViewModel>
{
    private readonly CompositeDisposable _disposable = [];
    public StatisticsFrameView(StatisticsViewModel viewModel)
    {
        ViewModel = viewModel;
        Title = "Statistics";
        
        var bufferSizeLabel = new Label
        {
            Text = "Buffer Size:",
            Width = 30,
            TextAlignment = Alignment.End
        };
        
        var bufferSizeValue = new Label
        {
            X = Pos.Right(bufferSizeLabel) + 1,
            Width = Dim.Fill()
        };

        ViewModel
            .WhenAnyValue(x => x.BufferSize)
            .BindTo(bufferSizeValue, lbl => lbl.Text);

        Add(bufferSizeLabel, bufferSizeValue);
    }
    
    public void UpdateStatistics(Statistics statistics)
    {
        // Update the buffer size value;
        if (ViewModel != null) ViewModel.BufferSize = statistics.BufferSize;
    }

    public StatisticsViewModel? ViewModel { get; set; }
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (StatisticsViewModel)value!;
    }
}