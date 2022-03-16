namespace GeneticCars.UI.Windows;

public sealed partial class Canvas : UserControl
{
  public Canvas()
  {
    InitializeComponent();
    SetStyle(ControlStyles.UserPaint, true);
    DoubleBuffered = true;
  }

  private void Draw(Graphics g)
  {
  }

  protected override void OnPaint(PaintEventArgs e)
  {
    Draw(e.Graphics);
    base.OnPaint(e);
  }
}
