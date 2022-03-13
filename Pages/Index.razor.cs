namespace GeneticCars.Pages;

using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Models;
using Newtonsoft.Json;
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

  private List<string> _trackList { get; set; } = new ();
  private string _selTrack { get; set; }

  private Canvas2DContext ctx;
  private BECanvasComponent _canvas;
  private string _image64 { get; set; }
  private ElementReference _trackImgRef { get; set; }

  private int _count;
  private bool _run;
  private string _debug { get; set; }

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
    _trackList = JsonConvert.DeserializeObject<List<string>>(trackListJson);
    _selTrack = _trackList.First();

    var trackStrm = await _client.GetByteArrayAsync($"tracks/{_selTrack}");
    var trackImg = Image.Load<Rgba32>(trackStrm);
    var track = new Track(trackImg);
    _track = new TrackDrawer(track, _trackImgRef);
    _debug += $"Loaded:  {_selTrack}" + Environment.NewLine;
    _debug += $"Start:  [{track.Start.X}, {track.Start.Y}]" + Environment.NewLine;

    using var outStream = new MemoryStream();
    trackImg.SaveAsPng(outStream);
    _image64 = "data:image/png;base64," + Convert.ToBase64String(outStream.ToArray());

    await base.OnInitializedAsync();
  }

  protected override async Task OnAfterRenderAsync(bool firstRender)
  {
    ctx = await _canvas.CreateCanvas2DAsync();
    await JsRuntime.InvokeAsync<object>("initRenderJS", DotNetObjectReference.Create(this));
    await base.OnAfterRenderAsync(firstRender);
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

    var car = _cars.OfType<CarDrawer>().SingleOrDefault()?.Car;
    car?.Move(1, 1);
    car?.Rotate(1.5);
    _count++;
    if (car?.Position.X > CanvasWidth ||
      car?.Position.Y > CanvasHeight)
    {
      _run = false;
    }

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
    var car = _cars.OfType<CarDrawer>().SingleOrDefault()?.Car;
    var pos = car?.Position;
    car?.Move(-pos?.X ?? 0, -pos?.Y ?? 0);
    var heading = car?.Heading;
    car?.Rotate(heading ?? 0);
  }
}
