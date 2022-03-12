namespace GeneticCars;

using Blazor.Extensions.Canvas.Canvas2D;

public interface IDrawable
{
  Task Draw(Canvas2DContext ctx);
}
