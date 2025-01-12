// See https://aka.ms/new-console-template for more information

using System.Reactive.Concurrency;
using ConsoleGui;
using ReactiveUI;
using Terminal.Gui;

// ConfigurationManager.RuntimeConfig = """{ "Theme": "Light" }""";
Application.Init();

RxApp.MainThreadScheduler = TerminalScheduler.Default;
RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;

Application.Run<MainWindow>().Dispose();

// Before the application exits, reset Terminal.Gui for clean shutdown
Application.Shutdown ();

// To see this output on the screen it must be done after shutdown,
// which restores the previous screen.
Console.WriteLine ($"Username: {MainWindow.SearchTerm}");