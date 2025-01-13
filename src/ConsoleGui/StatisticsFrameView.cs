using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
        
        var previous = AddRow(null,"Buffer Size:", vm => vm.BufferSize);
        previous = AddRow(previous, "Parallel Tasks:", vm => vm.ParallelTasks);
        previous = AddRow(previous, "Strategy:", vm => vm.Strategy);
        previous = AddRow(previous, "Total elapsed time:", vm => vm.TotalMilliseconds, "ms");
        previous = AddRow(previous, "Total Bytes Read:", vm => vm.TotalBytesRead, "B");
        previous = AddRow(previous, "Total Megabytes Read:", vm => vm.TotalMegabytesRead, "MB");
        previous = AddRow(previous, "Read Rate:", vm => vm.ReadRateInBytesPerSecond, "B/s");
        AddRow(previous, "Read Rate:", vm => vm.ReadRateInMegabytesPerSecond, "MB/s");
    }

    public StatisticsViewModel? ViewModel { get; set; }
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (StatisticsViewModel)value!;
    }
    
    protected override void Dispose (bool disposing)
    {
        _disposable.Dispose();
        base.Dispose(disposing);
    }

    private Pos AddRow<TProp>(Pos? previous, string labelText, Expression<Func<StatisticsViewModel, TProp>> propSelector, string measurementUnit = "")
    {   
        var nextY = previous is null ? 0 : previous + 1;
        
        var labelLabel = new Label
        {
            Y = nextY,
            Text = labelText,
            Width = 25,
            TextAlignment = Alignment.End
        };
        
        var valueLabel = new Label
        {
            X = Pos.Right(labelLabel) + 1,
            Y = nextY,
            Width = 25,
            TextAlignment = Alignment.End
        };
        
        ViewModel
            .WhenAnyValue(propSelector)
            .BindTo(valueLabel, lbl => lbl.Text)
            .DisposeWith(_disposable);

        Add(labelLabel, valueLabel);

        if (!string.IsNullOrEmpty(measurementUnit))
        {
            var measurementUnitLabel = new Label
            {
                X = Pos.Right(valueLabel) + 1,
                Y = nextY,
                Width = 5,
                Text = measurementUnit
            };
            Add(measurementUnitLabel);
        }

        return Pos.Bottom(labelLabel);
    }
}