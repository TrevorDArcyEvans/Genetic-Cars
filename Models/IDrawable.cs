namespace GeneticCars.Models;

using Excubo.Blazor.Canvas.Contexts;

public interface IDrawable
{
  Task Draw(Context2D ctx);
}
