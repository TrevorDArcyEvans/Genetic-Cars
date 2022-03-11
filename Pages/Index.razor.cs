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
  private readonly Timer _timer = new(1000);

  private List<IDrawable> _drawables = new();

  public Index()
  {
    var car1 = new Car();
    var carDraw1 = new CarDrawer(car1);
    _drawables.Add(carDraw1);
    
    _timer.Elapsed += TimerOnElapsed;
  }

  private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
  {
    _count++;
    var carDraw = _drawables.OfType<CarDrawer>().SingleOrDefault();
    carDraw?.Car.Move(10, 10);
    carDraw?.Car.Rotate(15);
    InvokeAsync(StateHasChanged);
  }

  protected override async Task OnAfterRenderAsync(bool firstRender)
  {
    await using var ctx = await _canvas.GetContext2DAsync();
    await ctx.ClearRectAsync(0, 0, CanvasWidth, CanvasHeight);
    await ctx.FontAsync("48px solid");
    await ctx.FillTextAsync(_count.ToString(), 0, 150);
    var draws = _drawables.Select(d => d.Draw(ctx));
    await Task.WhenAll(draws);
  }

  private void OnStartClick()
  {
    _timer.Enabled = !_timer.Enabled;
  }
}
