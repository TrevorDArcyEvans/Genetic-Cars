using GeneticCars.UI.Windows.Drawables;

namespace GeneticCars.Pages;

using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Drawables;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Models;
using Network;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public partial class Index
{
  [Inject]
  private IJSRuntime JsRuntime { get; set; }

  [Inject]
  private HttpClient _client { get; set; }

  private List<string> _trackList { get; set; } = new();
  private string _selTrack { get; set; }

  private Canvas2DContext ctx;
  private BECanvasComponent _canvas;
  private string _image64 { get; set; }
  private ElementReference _trackImgRef { get; set; }

  private bool _run;
  private string _debug { get; set; }

  private readonly List<CarDrawer> _cars = new();
  private TrackDrawer _track;

  private EvolutionManager _evMgr;
  private readonly StatusMessage _statusMsg = new ();
  private readonly StatusMessageDrawer _status;

  public Index()
  {
    _status = new (_statusMsg);
  }

  protected override async Task OnInitializedAsync()
  {
    var trackListResp = await _client.GetAsync("tracks/tracks.json");
    var trackListJson = await trackListResp.Content.ReadAsStringAsync();
    _trackList = JsonConvert.DeserializeObject<List<string>>(trackListJson);
    var trackChangeEvt = new ChangeEventArgs
    {
      Value = _trackList.First()
    };
    await OnTrackChangedAsync(trackChangeEvt);

    OnResetClick();

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

    await ctx.ClearRectAsync(0, 0, _track.Track.MapWidth, _track.Track.MapHeight);
    await ctx.SetFillStyleAsync("white");
    await ctx.FillRectAsync(0, 0, _track.Track.MapWidth, _track.Track.MapHeight);

    await _track.Draw(ctx);

    _statusMsg.Text = $"{_evMgr.GenerationCount} / {EvolutionManager.MaxGenerations}";
    await _status.Draw(ctx);

    _evMgr.Update();

    var cars = _cars.Select(d => d.Draw(ctx));
    await Task.WhenAll(cars);

    await ctx.EndBatchAsync();
  }

  private void OnStartClick()
  {
    _run = !_run;
  }

  private void OnResetClick()
  {
    _run = false;

    _evMgr.Reset();
  }

  private async Task OnTrackChangedAsync(ChangeEventArgs e)
  {
    _selTrack = (string)e.Value;
    var trackStrm = await _client.GetByteArrayAsync($"tracks/{_selTrack}");
    var trackImg = Image.Load<Rgba32>(trackStrm);
    var track = new Track(trackImg);
    _track = new TrackDrawer(track, _trackImgRef);
    _debug += $"Track changed:  {_selTrack}" + Environment.NewLine;
    _debug += $"Start:   [{track.Start.X}, {track.Start.Y}]" + Environment.NewLine;
    _debug += $"ChkPts:  [{track.Checkpoints.Count()}]" + Environment.NewLine;

    using var outStream = new MemoryStream();
    trackImg.SaveAsPng(outStream);
    _image64 = "data:image/png;base64," + Convert.ToBase64String(outStream.ToArray());

    _cars.Clear();
    foreach (var _ in Enumerable.Range(0, 10))
    {
      var car = new Car(_track.Track.Start, _track.Track.Direction);
      var carDraw = new CarDrawer(car);
      _cars.Add(carDraw);
    }

    var cars = _cars.Select(car => car.Car).ToList().AsReadOnly();
    _evMgr = new(_track.Track, cars);
  }
}
