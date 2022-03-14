namespace GeneticCars.Models;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public sealed class Track
{
  // Start point is a 5x5 square of green pixels
  // This point is centre of the 5x5 square
  public System.Drawing.Point Start { get; }

  public int Width => _data.GetLength(0);
  public int Height => _data.GetLength(1);
  public double Direction { get; private set; } // degrees

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
  }

  public bool IsTrack(int x, int y)
  {
    return _data[x, y];
  }

  // Convert image to black and white boolean matrix (width x height)
  // black = non-track = false
  // white = track = true
  private static bool[,] ConvertImageToBlackAndWhite(Image<Rgba32> inputImg)
  {
    // allocate gray image
    byte[,] GrayImage = new byte[inputImg.Width, inputImg.Height];
    int[] GrayLevel = new int[256];

    // convert to gray
    inputImg.ProcessPixelRows(acc =>
    {
      for (var y = 0; y < acc.Height; y++)
      {
        var pxRow = acc.GetRowSpan(y);
        for (var x = 0; x < pxRow.Length - 1; x++)
        {
          ref var px = ref pxRow[x];
          int Module = (30 * px.R + 59 * px.G + 11 * px.B) / 100;
          GrayLevel[Module]++;
          GrayImage[x, y] = (byte)Module;
        }
      }
    });

    // gray level cutoff between black and white
    int LevelStart;
    int LevelEnd;
    for (LevelStart = 0; LevelStart < 256 && GrayLevel[LevelStart] == 0; LevelStart++)
    {
      // DO_NOTHING
    }

    for (LevelEnd = 255; LevelEnd >= LevelStart && GrayLevel[LevelEnd] == 0; LevelEnd--)
    {
      // DO_NOTHING
    }

    LevelEnd++;

    int CutoffLevel = (LevelStart + LevelEnd) / 2;

    // create boolean image white = true, black = false
    var BlackWhiteImage = new bool[inputImg.Width, inputImg.Height];
    for (int Row = 0; Row < inputImg.Height; Row++)
    {
      for (int Col = 0; Col < inputImg.Width; Col++)
      {
        BlackWhiteImage[Col, Row] = GrayImage[Row, Col] > CutoffLevel;
      }
    }

    return BlackWhiteImage;
  }

  private static System.Drawing.Point GetStart(Image<Rgba32> inputImg)
  {
    var startPts = new List<Point>();
    inputImg.ProcessPixelRows(acc =>
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

  private static double GetDirection(Image<Rgba32> inputImg)
  {
    var retval = 0d;
    inputImg.ProcessPixelRows(acc =>
    {
      var pxRow = acc.GetRowSpan(0);
      ref var px = ref pxRow[0];
      retval = px.R + px.G + px.B;
    });

    return retval;
  }
}
