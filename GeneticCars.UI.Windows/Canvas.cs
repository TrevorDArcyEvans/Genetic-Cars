namespace GeneticCars.UI.Windows;

public sealed partial class Canvas : UserControl
{
  private List<IDrawable> _drawables;

  public Canvas()
  {
    InitializeComponent();
    SetStyle(ControlStyles.UserPaint, true);
    DoubleBuffered = true;
  }

  public void SetDrawables(List<IDrawable> drawables)
  {
    _drawables = drawables;
  }

  protected override void OnPaint(PaintEventArgs e)
  {
    _drawables.ForEach(drawable => drawable.Draw(e.Graphics));
    base.OnPaint(e);
  }
}
