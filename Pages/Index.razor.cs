﻿namespace GeneticCars.Pages;

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

  private List<string> _trackList { get; set; } = new();
  private string _selTrack { get; set; }

  private Canvas2DContext ctx;
  private BECanvasComponent _canvas;
  private string _image64 { get; set; }
  private ElementReference _trackImgRef { get; set; }

  private int _count;
  private bool _run;
  private string _debug { get; set; }

  private readonly List<CarDrawer> _cars = new();
  private TrackDrawer _track;

  public Index()
  {
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

    var car1 = new Car(_track.Track.Start, _track.Track.Direction);
    var carDraw1 = new CarDrawer(car1);
    _cars.Add(carDraw1);

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

    await ctx.ClearRectAsync(0, 0, CanvasWidth, CanvasHeight);
    await ctx.SetFillStyleAsync("white");
    await ctx.FillRectAsync(0, 0, CanvasWidth, CanvasHeight);

    await _track.Draw(ctx);

    await ctx.SetFontAsync("48px solid");
    await ctx.SetFillStyleAsync("white");
    await ctx.FillTextAsync(_count.ToString(), 700, 750);

    var car = _cars.OfType<CarDrawer>().SingleOrDefault()?.Car;
    car?.Move(1, 1);
    car?.Rotate(1.5);
    _count++;
    if (car?.Position.X > CanvasWidth ||
        car?.Position.Y > CanvasHeight)
    {
      OnResetClick();
    }

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
    _count = 0;

    _cars.ForEach(car => car.Car.Reset(_track.Track.Start, _track.Track.Direction));
  }

  private async Task OnTrackChangedAsync(ChangeEventArgs e)
  {
    _selTrack = (string)e.Value;
    var trackStrm = await _client.GetByteArrayAsync($"tracks/{_selTrack}");
    var trackImg = Image.Load<Rgba32>(trackStrm);
    var track = new Track(trackImg);
    _track = new TrackDrawer(track, _trackImgRef);
    _debug += $"Track changed:  {_selTrack}" + Environment.NewLine;
    _debug += $"Start:  [{track.Start.X}, {track.Start.Y}]" + Environment.NewLine;

    using var outStream = new MemoryStream();
    trackImg.SaveAsPng(outStream);
    _image64 = "data:image/png;base64," + Convert.ToBase64String(outStream.ToArray());
  }
}
