// See https://aka.ms/new-console-template for more information

using System.Reactive.Concurrency;
using ConsoleGui;
using ReactiveUI;
using Terminal.Gui;

// ConfigurationManager.RuntimeConfig = """{ "Theme": "Light" }""";
Application.Init();

RxApp.MainThreadScheduler = TerminalScheduler.Default;
RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;

var searchFormViewModel = new SearchFormViewModel();
var statisticsViewModel = new StatisticsViewModel();

searchFormViewModel.Search.Subscribe(statisticsViewModel.UpdateStatistics);

var mainWindow = new MainWindow(searchFormViewModel, statisticsViewModel);

Application.Run(mainWindow);

mainWindow.Dispose();

// Before the application exits, reset Terminal.Gui for clean shutdown
Application.Shutdown();
