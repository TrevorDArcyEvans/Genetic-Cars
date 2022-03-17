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
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.Label label1;
      this._canvas = new GeneticCars.UI.Windows.Canvas();
      this.CmdStart = new System.Windows.Forms.Button();
      this.CmdReset = new System.Windows.Forms.Button();
      this.Tracks = new System.Windows.Forms.ComboBox();
      this.DebugLog = new System.Windows.Forms.TextBox();
      this._timer = new System.Windows.Forms.Timer(this.components);
      this.MaxGenerations = new System.Windows.Forms.NumericUpDown();
      label1 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.MaxGenerations)).BeginInit();
      this.SuspendLayout();
      // 
      // _canvas
      // 
      this._canvas.AutoScroll = true;
      this._canvas.AutoScrollMargin = new System.Drawing.Size(15, 15);
      this._canvas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this._canvas.Location = new System.Drawing.Point(19, 16);
      this._canvas.Margin = new System.Windows.Forms.Padding(0);
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
      this.CmdStart.Click += new System.EventHandler(this.CmdStart_Click);
      // 
      // CmdReset
      // 
      this.CmdReset.Location = new System.Drawing.Point(825, 45);
      this.CmdReset.Name = "CmdReset";
      this.CmdReset.Size = new System.Drawing.Size(75, 23);
      this.CmdReset.TabIndex = 2;
      this.CmdReset.Text = "Reset";
      this.CmdReset.UseVisualStyleBackColor = true;
      this.CmdReset.Click += new System.EventHandler(this.CmdReset_Click);
      // 
      // Tracks
      // 
      this.Tracks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.Tracks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.Tracks.FormattingEnabled = true;
      this.Tracks.Location = new System.Drawing.Point(825, 74);
      this.Tracks.Name = "Tracks";
      this.Tracks.Size = new System.Drawing.Size(390, 23);
      this.Tracks.TabIndex = 3;
      this.Tracks.SelectedIndexChanged += new System.EventHandler(this.Tracks_SelectedIndexChanged);
      // 
      // DebugLog
      // 
      this.DebugLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.DebugLog.Location = new System.Drawing.Point(825, 103);
      this.DebugLog.Multiline = true;
      this.DebugLog.Name = "DebugLog";
      this.DebugLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.DebugLog.Size = new System.Drawing.Size(390, 713);
      this.DebugLog.TabIndex = 4;
      // 
      // _timer
      // 
      this._timer.Interval = 10;
      this._timer.Tick += new System.EventHandler(this.Timer_Tick);
      // 
      // MaxGenerations
      // 
      this.MaxGenerations.Location = new System.Drawing.Point(1095, 22);
      this.MaxGenerations.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
      this.MaxGenerations.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.MaxGenerations.Name = "MaxGenerations";
      this.MaxGenerations.Size = new System.Drawing.Size(120, 23);
      this.MaxGenerations.TabIndex = 5;
      this.MaxGenerations.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new System.Drawing.Point(990, 24);
      label1.Name = "label1";
      label1.Size = new System.Drawing.Size(99, 15);
      label1.TabIndex = 6;
      label1.Text = "Max Generations:";
      // 
      // MainForm
      // 
      this.AcceptButton = this.CmdStart;
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.CmdReset;
      this.ClientSize = new System.Drawing.Size(1227, 833);
      this.Controls.Add(label1);
      this.Controls.Add(this.MaxGenerations);
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
      ((System.ComponentModel.ISupportInitialize)(this.MaxGenerations)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private Canvas _canvas;
    private Button CmdStart;
    private Button CmdReset;
    private ComboBox Tracks;
    private TextBox DebugLog;
    private System.Windows.Forms.Timer _timer;
    private NumericUpDown MaxGenerations;
  }
}