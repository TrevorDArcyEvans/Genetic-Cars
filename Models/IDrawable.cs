using Excubo.Blazor.Canvas.Contexts;

namespace GeneticCars.Models;

public interface IDrawable
{
  Task Draw(Context2D ctx);
}
