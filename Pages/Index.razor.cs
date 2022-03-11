namespace GeneticCars.Pages;

using System.Timers;
using Excubo.Blazor.Canvas;
using Models;
using Timer = System.Timers.Timer;

public partial class Index
{
  private const int CanvasWidth = 800;
  private const int CanvasHeight = 800;
  
  private Canvas _canvas;
  private int _count;
  private readonly Timer _timer = new(1000){ Enabled = true };
  private bool _run;

  private readonly List<IDrawable> _checkpoints = new();
  private readonly List<IDrawable> _cars = new();

  public Index()
  {
    var car1 = new Car(new(20, 20));
    var carDraw1 = new CarDrawer(car1);
    _cars.Add(carDraw1);

    var chkpt1 = new Checkpoint(new (200, 200));
    var chkptDraw1 = new CheckpointDrawer(chkpt1);
    _checkpoints.Add(chkptDraw1);

    _timer.Elapsed += TimerOnElapsed;
  }

  private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
  {
    if (!_run)
    {
      return;
    }
  
    _count++;
    var carDraw = _cars.OfType<CarDrawer>().SingleOrDefault();
    carDraw?.Car.Move(10, 10);
    carDraw?.Car.Rotate(15);
    InvokeAsync(StateHasChanged);
  }

  protected override async Task OnAfterRenderAsync(bool firstRender)
  {
    await using var ctx = await _canvas.GetContext2DAsync();
    await ctx.ClearRectAsync(0, 0, CanvasWidth, CanvasHeight);
    await ctx.FontAsync("48px solid");
    await ctx.FillTextAsync(_count.ToString(), 650, 650);

    var cars = _cars.Select(d => d.Draw(ctx));
    await Task.WhenAll(cars);
    var checkpts = _checkpoints.Select(d => d.Draw(ctx));
    await Task.WhenAll(checkpts);
  }

  private void OnStartClick()
  {
    _run = !_run;
  }
}
