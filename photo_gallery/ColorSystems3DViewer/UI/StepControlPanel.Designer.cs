using System.ComponentModel;

namespace PixelLab;

partial class StepControlPanel
{
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
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

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        header = new System.Windows.Forms.Label();
        _stepTrack = new System.Windows.Forms.TrackBar();
        _stepValueLabel = new System.Windows.Forms.Label();
        _applyBtn = new System.Windows.Forms.Button();
        ((System.ComponentModel.ISupportInitialize)_stepTrack).BeginInit();
        SuspendLayout();
        // 
        // header
        // 
        header.Dock = System.Windows.Forms.DockStyle.Top;
        header.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)85)), ((int)((byte)79)), ((int)((byte)179)));
        header.Location = new System.Drawing.Point(0, 0);
        header.Name = "header";
        header.Size = new System.Drawing.Size(314, 33);
        header.TabIndex = 0;
        header.Text = "Points Density";
        header.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        // 
        // _stepTrack
        // 
        _stepTrack.BackColor = System.Drawing.Color.FromArgb(((int)((byte)50)), ((int)((byte)50)), ((int)((byte)50)));
        _stepTrack.Location = new System.Drawing.Point(3, 36);
        _stepTrack.Minimum = 1;
        _stepTrack.Name = "_stepTrack";
        _stepTrack.Size = new System.Drawing.Size(214, 56);
        _stepTrack.TabIndex = 1;
        _stepTrack.Value = 1;
        _stepTrack.ValueChanged += _stepTrack_ValueChanged;
        // 
        // _stepValueLabel
        // 
        _stepValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)85)), ((int)((byte)79)), ((int)((byte)179)));
        _stepValueLabel.Location = new System.Drawing.Point(223, 33);
        _stepValueLabel.Name = "_stepValueLabel";
        _stepValueLabel.Size = new System.Drawing.Size(88, 40);
        _stepValueLabel.TabIndex = 2;
        _stepValueLabel.Text = "Steps : 3";
        _stepValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        // 
        // _applyBtn
        // 
        _applyBtn.BackColor = System.Drawing.Color.FromArgb(((int)((byte)50)), ((int)((byte)50)), ((int)((byte)50)));
        _applyBtn.Dock = System.Windows.Forms.DockStyle.Bottom;
        _applyBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)((byte)85)), ((int)((byte)79)), ((int)((byte)179)));
        _applyBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        _applyBtn.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)85)), ((int)((byte)79)), ((int)((byte)179)));
        _applyBtn.Location = new System.Drawing.Point(0, 94);
        _applyBtn.Name = "_applyBtn";
        _applyBtn.Size = new System.Drawing.Size(314, 35);
        _applyBtn.TabIndex = 3;
        _applyBtn.Text = "Apply";
        _applyBtn.UseVisualStyleBackColor = false;
        _applyBtn.MouseClick += _applyBtn_MouseClick;
        // 
        // StepControlPanel
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(((int)((byte)50)), ((int)((byte)50)), ((int)((byte)50)));
        Controls.Add(_applyBtn);
        Controls.Add(_stepValueLabel);
        Controls.Add(_stepTrack);
        Controls.Add(header);
        Size = new System.Drawing.Size(314, 129);
        ((System.ComponentModel.ISupportInitialize)_stepTrack).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.Button _applyBtn;

    private System.Windows.Forms.Label _stepValueLabel;


    private System.Windows.Forms.TrackBar _stepTrack;
   

    private System.Windows.Forms.Label header;

    #endregion
}