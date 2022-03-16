namespace GeneticCars.Models;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public sealed class Track
{
  // Start point is a 5x5 square of green pixels
  // This point is centre of the 5x5 square
  public System.Drawing.Point Start { get; }
  
  // Checkpoint is a 5x5 square of red pixels
  // This point is centre of the 5x5 square
  public IEnumerable<Checkpoint> Checkpoints { get; }

  public int MapWidth => _data.GetLength(0);
  public int MapHeight => _data.GetLength(1);
  public double Direction { get; } // degrees

  // width x height
  // [x, y] track points
  // true --> track
  // false --> non-track
  private readonly bool[,] _data;

  public Track(Image<Rgba32> img)
  {
    _data = ConvertImageToBlackAndWhite(img);
    Start = GetStart(img);
    Direction = GetDirection(img);
    Checkpoints = GetCheckpoints(img);
  }

  public bool IsTrack(int x, int y)
  {
    return _data[x, y];
  }

  // Convert image to black and white boolean matrix (width x height)
  // black = non-track = false
  // white = track = true
  private static bool[,] ConvertImageToBlackAndWhite(Image<Rgba32> img)
  {
    // allocate gray image
    var grayImage = new byte[img.Width, img.Height];
    var grayLevel = new int[256];

    // convert to gray
    img.ProcessPixelRows(acc =>
    {
      for (var y = 0; y < acc.Height; y++)
      {
        var pxRow = acc.GetRowSpan(y);
        for (var x = 0; x < pxRow.Length - 1; x++)
        {
          ref var px = ref pxRow[x];
          var module = (30 * px.R + 59 * px.G + 11 * px.B) / 100;
          grayLevel[module]++;
          grayImage[x, y] = (byte)module;
        }
      }
    });

    // create boolean image white = true, black = false
    var blackWhiteImage = new bool[img.Width, img.Height];
    for (var row = 0; row < img.Height; row++)
    {
      for (var col = 0; col < img.Width; col++)
      {
        // Only colours we expect in track are:
        //  track         white     RGB(255, 255, 255)    gray(100)
        //  off track     black     RGB(0, 0, 0)          gray(0)
        //  direction     reddish   RGB(120, 120, 120)    gray(120)
        //  start         green     RGB(0, 255, 0)        gray(150)
        //  checkpoint    red       RGB(255, 0, 0)        gray(76)
        // so we set the threshold to be just below a checkpoint
        //
        // NOTE:  direction pixel will probably be detected as off track
        //        but car will have to go across many off track areas
        //        to get there
        blackWhiteImage[row, col] = grayImage[row, col] > 70;
      }
    }

    return blackWhiteImage;
  }

  private static System.Drawing.Point GetStart(Image<Rgba32> img)
  {
    var startPts = new List<Point>();
    img.ProcessPixelRows(acc =>
    {
      for (var y = 0; y < acc.Height; y++)
      {
        var pxRow = acc.GetRowSpan(y);
        for (var x = 0; x < pxRow.Length - 1; x++)
        {
          ref var px = ref pxRow[x];
          if (px.G > 245 &&
              px.R < 10 &&
              px.B < 10)
          {
            startPts.Add(new(x, y));
          }
        }
      }
    });

    var avgX = startPts.Sum(pt => pt.X) / startPts.Count;
    var avgY = startPts.Sum(pt => pt.Y) / startPts.Count;
    return new(avgX, avgY);
  }

  private static double GetDirection(Image<Rgba32> img)
  {
    var retval = 0d;
    img.ProcessPixelRows(acc =>
    {
      var pxRow = acc.GetRowSpan(0);
      ref var px = ref pxRow[0];
      retval = px.R + px.G + px.B;
    });

    return retval;
  }

  private static IEnumerable<Checkpoint> GetCheckpoints(Image<Rgba32> img)
  {
    string GetKey(int x, int y)
    {
      return $"[{x},{y}]";
    }

    // points we have already checked
    // key = [px.X,px.Y]
    var consideredPts = new HashSet<string>();
    
    var chkPts = new List<Point>();
    img.ProcessPixelRows(acc =>
    {
      for (var y = 0; y < acc.Height; y++)
      {
        var pxRow = acc.GetRowSpan(y);
        for (var x = 0; x < pxRow.Length - 1; x++)
        {
          ref var px = ref pxRow[x];
          var key = GetKey(x, y);
          if (px.R > 245 &&
              px.G < 10 &&
              px.B < 10 &&
              !consideredPts.Contains(key))
          {
            // if we get to here, we have found the top left hand corner of a checkpoint,
            // so we add the centre of the 5x5 square
            chkPts.Add(new(x + 2, y + 2));

            // then add all the 25 pixels in the checkpoint so we do not check them again
            for (var i = 0; i < 5; i++)
            {
              for (var j = 0; j < 5; j++)
              {
                consideredPts.Add(GetKey(x + i, y + j));
              }
            }
          }
          else
          {
            // one of:  direction, start, track or non-track
            consideredPts.Add(key);
          }
        }
      }
    });

    var retval = chkPts.Select(pt => new Checkpoint(new(pt.X, pt.Y)));
    return retval;
  }
}
