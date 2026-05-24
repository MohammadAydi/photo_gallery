namespace PixelLab;

partial class SpacesViewr
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        renderPanel = new System.Windows.Forms.Panel();
        glControl = new OpenTK.GLControl.GLControl();
        infoPanel = new System.Windows.Forms.Panel();
        _stepPanel = new PixelLab.StepControlPanel();
        filterPanel2 = new PixelLab.FilterPanel();
        filterPanel1 = new PixelLab.FilterPanel();
        _valuesPanel = new PixelLab.ColorValuesPanel();
        lstColorSpaces = new System.Windows.Forms.ListBox();
        filterPanel = new PixelLab.FilterPanel();
        renderPanel.SuspendLayout();
        infoPanel.SuspendLayout();
        SuspendLayout();
        // 
        // renderPanel
        // 
        renderPanel.Controls.Add(glControl);
        renderPanel.Controls.Add(infoPanel);
        renderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        renderPanel.Location = new System.Drawing.Point(0, 0);
        renderPanel.Name = "renderPanel";
        renderPanel.Size = new System.Drawing.Size(1382, 853);
        renderPanel.TabIndex = 1;
        // 
        // glControl
        // 
        glControl.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
        glControl.APIVersion = new System.Version(3, 3, 0, 0);
        glControl.Dock = System.Windows.Forms.DockStyle.Fill;
        glControl.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
        glControl.IsEventDriven = true;
        glControl.Location = new System.Drawing.Point(379, 0);
        glControl.Name = "glControl";
        glControl.Profile = OpenTK.Windowing.Common.ContextProfile.Core;
        glControl.SharedContext = null;
        glControl.Size = new System.Drawing.Size(1003, 853);
        glControl.TabIndex = 1;
        glControl.Load += glControl_Load;
        glControl.Paint += glControl_Paint;
        glControl.MouseDown += glControl_MouseDown;
        glControl.MouseMove += glControl_MouseMove;
        glControl.MouseUp += glControl_MouseUp;
        glControl.Resize += glControl_Resize;
        // 
        // infoPanel
        // 
        infoPanel.BackColor = System.Drawing.Color.FromArgb(((int)((byte)50)), ((int)((byte)50)), ((int)((byte)50)));
        infoPanel.Controls.Add(_stepPanel);
        infoPanel.Controls.Add(filterPanel2);
        infoPanel.Controls.Add(filterPanel1);
        infoPanel.Controls.Add(_valuesPanel);
        infoPanel.Controls.Add(lstColorSpaces);
        infoPanel.Controls.Add(filterPanel);
        infoPanel.Dock = System.Windows.Forms.DockStyle.Left;
        infoPanel.Location = new System.Drawing.Point(0, 0);
        infoPanel.Name = "infoPanel";
        infoPanel.Size = new System.Drawing.Size(379, 853);
        infoPanel.TabIndex = 0;
        // 
        // _stepPanel
        // 
        _stepPanel.BackColor = System.Drawing.Color.FromArgb(((int)((byte)50)), ((int)((byte)50)), ((int)((byte)50)));
        _stepPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
        _stepPanel.Location = new System.Drawing.Point(0, 706);
        _stepPanel.Name = "_stepPanel";
        _stepPanel.Size = new System.Drawing.Size(379, 147);
        _stepPanel.TabIndex = 9;
        // 
        // filterPanel2
        // 
        filterPanel2.AutoScroll = true;
        filterPanel2.AutoSize = true;
        filterPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
        filterPanel2.Location = new System.Drawing.Point(0, 334);
        filterPanel2.Name = "filterPanel2";
        filterPanel2.Size = new System.Drawing.Size(379, 519);
        filterPanel2.TabIndex = 8;
        // 
        // filterPanel1
        // 
        filterPanel1.AutoScroll = true;
        filterPanel1.AutoSize = true;
        filterPanel1.Dock = System.Windows.Forms.DockStyle.Top;
        filterPanel1.Location = new System.Drawing.Point(0, 334);
        filterPanel1.Name = "filterPanel1";
        filterPanel1.Size = new System.Drawing.Size(379, 0);
        filterPanel1.TabIndex = 6;
        // 
        // _valuesPanel
        // 
        _valuesPanel.BackColor = System.Drawing.Color.FromArgb(((int)((byte)50)), ((int)((byte)50)), ((int)((byte)50)));
        _valuesPanel.Dock = System.Windows.Forms.DockStyle.Top;
        _valuesPanel.Location = new System.Drawing.Point(0, 162);
        _valuesPanel.Name = "_valuesPanel";
        _valuesPanel.Padding = new System.Windows.Forms.Padding(8);
        _valuesPanel.Size = new System.Drawing.Size(379, 172);
        _valuesPanel.TabIndex = 5;
        // 
        // lstColorSpaces
        // 
        lstColorSpaces.BackColor = System.Drawing.Color.FromArgb(((int)((byte)50)), ((int)((byte)50)), ((int)((byte)50)));
        lstColorSpaces.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        lstColorSpaces.Dock = System.Windows.Forms.DockStyle.Top;
        lstColorSpaces.ForeColor = System.Drawing.Color.White;
        lstColorSpaces.FormattingEnabled = true;
        lstColorSpaces.Items.AddRange(new object[] { "\"RGB\"" });
        lstColorSpaces.Location = new System.Drawing.Point(0, 0);
        lstColorSpaces.Name = "lstColorSpaces";
        lstColorSpaces.Size = new System.Drawing.Size(379, 162);
        lstColorSpaces.TabIndex = 4;
        lstColorSpaces.SelectedIndexChanged += lstColorSpaces_SelectedIndexChanged;
        // 
        // filterPanel
        // 
        filterPanel.AutoScroll = true;
        filterPanel.AutoSize = true;
        filterPanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
        filterPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        filterPanel.Location = new System.Drawing.Point(0, 851);
        filterPanel.Name = "filterPanel";
        filterPanel.Size = new System.Drawing.Size(353, 500);
        filterPanel.TabIndex = 3;
        // 
        // SpacesViewr
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(((int)((byte)31)), ((int)((byte)31)), ((int)((byte)31)));
        ClientSize = new System.Drawing.Size(1382, 853);
        Controls.Add(renderPanel);
        Location = new System.Drawing.Point(19, 19);
        renderPanel.ResumeLayout(false);
        infoPanel.ResumeLayout(false);
        infoPanel.PerformLayout();
        ResumeLayout(false);
    }

    private PixelLab.StepControlPanel _stepPanel;

    private PixelLab.FilterPanel filterPanel2;

    private PixelLab.FilterPanel filterPanel;

    private System.Windows.Forms.ListBox lstColorSpaces;

    private PixelLab.FilterPanel filterPanel1;

    private PixelLab.ColorValuesPanel _valuesPanel;


    

    private System.Windows.Forms.Panel infoPanel;

    private System.Windows.Forms.Panel renderPanel;

    private OpenTK.GLControl.GLControl glControl;

    #endregion
}