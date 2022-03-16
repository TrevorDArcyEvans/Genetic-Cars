namespace GeneticCars.UI.Windows
{
  partial class MainForm
  {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this._canvas = new GeneticCars.UI.Windows.Canvas();
      this.CmdStart = new System.Windows.Forms.Button();
      this.CmdReset = new System.Windows.Forms.Button();
      this.Tracks = new System.Windows.Forms.ComboBox();
      this.DebugLog = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // _canvas
      // 
      this._canvas.AutoScroll = true;
      this._canvas.AutoScrollMargin = new System.Drawing.Size(15, 15);
      this._canvas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this._canvas.Location = new System.Drawing.Point(19, 16);
      this._canvas.Name = "_canvas";
      this._canvas.Size = new System.Drawing.Size(800, 800);
      this._canvas.TabIndex = 0;
      // 
      // CmdStart
      // 
      this.CmdStart.Location = new System.Drawing.Point(825, 16);
      this.CmdStart.Name = "CmdStart";
      this.CmdStart.Size = new System.Drawing.Size(75, 23);
      this.CmdStart.TabIndex = 1;
      this.CmdStart.Text = "Start";
      this.CmdStart.UseVisualStyleBackColor = true;
      // 
      // CmdReset
      // 
      this.CmdReset.Location = new System.Drawing.Point(825, 45);
      this.CmdReset.Name = "CmdReset";
      this.CmdReset.Size = new System.Drawing.Size(75, 23);
      this.CmdReset.TabIndex = 2;
      this.CmdReset.Text = "Reset";
      this.CmdReset.UseVisualStyleBackColor = true;
      // 
      // Tracks
      // 
      this.Tracks.FormattingEnabled = true;
      this.Tracks.Location = new System.Drawing.Point(825, 74);
      this.Tracks.Name = "Tracks";
      this.Tracks.Size = new System.Drawing.Size(205, 23);
      this.Tracks.TabIndex = 3;
      // 
      // DebugLog
      // 
      this.DebugLog.Location = new System.Drawing.Point(825, 103);
      this.DebugLog.Multiline = true;
      this.DebugLog.Name = "DebugLog";
      this.DebugLog.Size = new System.Drawing.Size(205, 713);
      this.DebugLog.TabIndex = 4;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1042, 836);
      this.Controls.Add(this.DebugLog);
      this.Controls.Add(this.Tracks);
      this.Controls.Add(this.CmdReset);
      this.Controls.Add(this.CmdStart);
      this.Controls.Add(this._canvas);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.Name = "MainForm";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.Text = "Genetic Cars";
      this.Load += new System.EventHandler(this.MainForm_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private Canvas _canvas;
    private Button CmdStart;
    private Button CmdReset;
    private ComboBox Tracks;
    private TextBox DebugLog;
  }
}