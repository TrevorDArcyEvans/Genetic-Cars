namespace GeneticCars.UI.Windows;

using Models;
using Drawables;
using Network;
using SixLabors.ImageSharp.PixelFormats;
using System.Reflection;
using Image = SixLabors.ImageSharp.Image;

public partial class MainForm : Form
{
  private int _count;
  private readonly List<CarDrawer> _cars = new();
  private TrackDrawer _track;
  private EvolutionManager _evMgr;
  private readonly StatusMessage _statusMsg = new ();

  public MainForm()
  {
    InitializeComponent();

    var tracksDir = GetTracksDir();
    var tracks = Directory.EnumerateFiles(tracksDir, "*.png");
    Tracks.DataSource = tracks.Select(trackPath => Path.GetFileName(trackPath)).ToList();
  }

  private void CmdStart_Click(object sender, EventArgs e)
  {
    _timer.Enabled = !_timer.Enabled;
  }

  private void CmdReset_Click(object sender, EventArgs e)
  {
    Reset();
  }

  private void Reset()
  {
    _timer.Enabled = false;
    _count = 0;

    _evMgr.Reset();
  }

  private static string GetTracksDir()
  {
    var currAssyPath = Assembly.GetExecutingAssembly().Location;
    var currAssyDir = Path.GetDirectoryName(currAssyPath);
    var tracksDir = Path.Combine(currAssyDir, "tracks");
    return tracksDir;
  }

  private void Tracks_SelectedIndexChanged(object sender, EventArgs e)
  {
    var trackName = (string)Tracks.SelectedItem;
    var trackPath = Path.Combine(GetTracksDir(), trackName);
    var trackImg = Image.Load<Rgba32>(trackPath);
    var track = new Track(trackImg);
    _track = new TrackDrawer(track, new Bitmap(trackPath));

    DebugLog.Text += $"Track changed:  {trackName}" + Environment.NewLine;
    DebugLog.Text += $"Start:   [{track.Start.X}, {track.Start.Y}]" + Environment.NewLine;
    DebugLog.Text += $"ChkPts:  [{track.Checkpoints.Count()}]" + Environment.NewLine;

    _cars.Clear();
    foreach (var _ in Enumerable.Range(0, 10))
    {
      var car = new Car(_track.Track.Start, _track.Track.Direction);
      var carDraw = new CarDrawer(car);
      _cars.Add(carDraw);
    }

    var cars = _cars.Select(car => car.Car).ToList().AsReadOnly();
    _evMgr = new(_track.Track, cars);

    var statusMsgDrawer = new StatusMessageDrawer(_statusMsg);

    var drawables = new List<IDrawable>();
    drawables.Add(_track);
    drawables.Add(statusMsgDrawer);
    drawables.AddRange(_cars);
    _canvas.SetDrawables(drawables);

    _canvas.Invalidate();

    Reset();
  }

  private void Timer_Tick(object sender, EventArgs e)
  {
    _statusMsg.Text = $"{_evMgr.GenerationCount} / {_evMgr.MaxGenerations}";
    _evMgr.Update();
    _canvas.Invalidate();
  }
}
