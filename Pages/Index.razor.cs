namespace GeneticCars.Pages;

using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Models;

public partial class Index
{
  private const int CanvasWidth = 800;
  private const int CanvasHeight = 800;
  
  [Inject]
  private IJSRuntime JsRuntime { get; set; }

  private Canvas2DContext ctx;
  private BECanvasComponent _canvas;
  private int _count;
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
  }

  protected override async Task OnAfterRenderAsync(bool firstRender)
  {
    this.ctx = await _canvas.CreateCanvas2DAsync();
    await JsRuntime.InvokeAsync<object>("initRenderJS", DotNetObjectReference.Create(this));
    await base.OnInitializedAsync();
  }

  [JSInvokable]
  public async ValueTask RenderInBlazor(float timeStamp)
  {
    await this.ctx.BeginBatchAsync();

    await this.ctx.ClearRectAsync(0, 0, CanvasWidth, CanvasHeight);
    await this.ctx.SetFillStyleAsync("white");
    await this.ctx.FillRectAsync(0, 0, CanvasWidth, CanvasHeight);

    await ctx.SetFontAsync("48px solid");
    await ctx.FillTextAsync(_count.ToString(), 650, 650);

    var carDraw = _cars.OfType<CarDrawer>().SingleOrDefault();
    carDraw?.Car.Move(10, 10);
    carDraw?.Car.Rotate(15);

    var cars = _cars.Select(d => d.Draw(ctx));
    await Task.WhenAll(cars);
    var checkpts = _checkpoints.Select(d => d.Draw(ctx));
    await Task.WhenAll(checkpts);

    await this.ctx.EndBatchAsync();
  }

  private void OnStartClick()
  {
    _run = !_run;
  }
}
