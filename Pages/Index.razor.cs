namespace GeneticCars.Pages;

using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public partial class Index
{
  private const int CanvasWidth = 800;
  private const int CanvasHeight = 800;
  
  [Inject]
  private IJSRuntime JsRuntime { get; set; }

  [Inject]
  private HttpClient _client { get; set; }

  private string TrackList { get; set; }

  private Canvas2DContext ctx;
  private BECanvasComponent _canvas;
  private string _image64 { get; set; }
  private ElementReference _trackImgRef { get; set; }

  private int _count;
  private bool _run;

  private readonly List<IDrawable> _checkpoints = new();
  private readonly List<IDrawable> _cars = new();
  private IDrawable _track;

  public Index()
  {
    var car1 = new Car(new(20, 20));
    var carDraw1 = new CarDrawer(car1);
    _cars.Add(carDraw1);

    var chkpt1 = new Checkpoint(new (200, 200));
    var chkptDraw1 = new CheckpointDrawer(chkpt1);
    _checkpoints.Add(chkptDraw1);
  }

  protected override async Task OnInitializedAsync()
  {
    var trackListResp = await _client.GetAsync("tracks/tracks.json");
    var trackListJson = await trackListResp.Content.ReadAsStringAsync();
    TrackList = trackListJson;
    var trackStrm = await _client.GetByteArrayAsync("tracks/track001.png");
    var trackImg = Image.Load<Rgba32>(trackStrm);
    var track = new Track(trackImg);
    _track = new TrackDrawer(track, _trackImgRef);
    TrackList += Environment.NewLine + "Loaded:  tracks/track001.png";

    using var outStream = new MemoryStream();
    trackImg.SaveAsPng(outStream);
    _image64 = "data:image/png;base64," + Convert.ToBase64String(outStream.ToArray());


    await base.OnInitializedAsync();
  }

  protected override async Task OnAfterRenderAsync(bool firstRender)
  {
    ctx = await _canvas.CreateCanvas2DAsync();
    await JsRuntime.InvokeAsync<object>("initRenderJS", DotNetObjectReference.Create(this));
    await base.OnInitializedAsync();
  }

  [JSInvokable]
  public async ValueTask RenderInBlazor(float timeStamp)
  {
    if (!_run)
    {
      return;
    }

    await ctx.BeginBatchAsync();

    await ctx.ClearRectAsync(0, 0, CanvasWidth, CanvasHeight);
    await ctx.SetFillStyleAsync("white");
    await ctx.FillRectAsync(0, 0, CanvasWidth, CanvasHeight);

    await _track.Draw(ctx);

    await ctx.SetFontAsync("48px solid");
    await ctx.SetFillStyleAsync("white");
    await ctx.FillTextAsync(_count.ToString(), 650, 650);

    var carDraw = _cars.OfType<CarDrawer>().SingleOrDefault();
    carDraw?.Car.Move(1, 1);
    carDraw?.Car.Rotate(1.5);
    _count++;

    var cars = _cars.Select(d => d.Draw(ctx));
    await Task.WhenAll(cars);
    var checkpts = _checkpoints.Select(d => d.Draw(ctx));
    await Task.WhenAll(checkpts);

    await ctx.EndBatchAsync();
  }

  private void OnStartClick()
  {
    _run = !_run;
  }

  private void OnResetClickAsync()
  {
    _run = false;
    _count = 0;
    var carDraw = _cars.OfType<CarDrawer>().SingleOrDefault();
    var pos = carDraw?.Car.Position;
    carDraw?.Car.Move(-pos?.X ?? 0, -pos?.Y ?? 0);
    var heading = carDraw?.Car.Heading;
    carDraw?.Car.Rotate(heading ?? 0);
  }
}
