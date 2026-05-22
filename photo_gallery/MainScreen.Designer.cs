namespace photo_gallery;

partial class MainScreen {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
        if (disposing && (components != null)) {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
        panel6 = new System.Windows.Forms.Panel();
        radioButton2 = new System.Windows.Forms.RadioButton();
        radioButton3 = new System.Windows.Forms.RadioButton();
        radioButton4 = new System.Windows.Forms.RadioButton();
        radioButton5 = new System.Windows.Forms.RadioButton();
        radioButton6 = new System.Windows.Forms.RadioButton();
        TopBar = new System.Windows.Forms.Panel();
        TobBarFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
        TitlePanel = new System.Windows.Forms.Panel();
        Title = new System.Windows.Forms.Label();
        ActionButtonsPanel = new System.Windows.Forms.Panel();
        ActionButtonsFlow = new System.Windows.Forms.FlowLayoutPanel();
        LoadButton = new System.Windows.Forms.Button();
        ResetButton = new System.Windows.Forms.Button();
        SaveButton = new System.Windows.Forms.Button();
        Open3DSpaceButton = new System.Windows.Forms.Button();
        ColorsNumberLabel = new System.Windows.Forms.Label();
        ColorsNumberFlow = new System.Windows.Forms.FlowLayoutPanel();
        ColorNumberSlider = new System.Windows.Forms.TrackBar();
        UpDownColorsNumber = new System.Windows.Forms.NumericUpDown();
        TopBar.SuspendLayout();
        TobBarFlowLayout.SuspendLayout();
        TitlePanel.SuspendLayout();
        ActionButtonsPanel.SuspendLayout();
        ActionButtonsFlow.SuspendLayout();
        ColorsNumberFlow.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)ColorNumberSlider).BeginInit();
        ((System.ComponentModel.ISupportInitialize)UpDownColorsNumber).BeginInit();
        SuspendLayout();
        // 
        // panel6
        // 
        panel6.BackColor = System.Drawing.Color.Aqua;
        panel6.Location = new System.Drawing.Point(3, 39);
        panel6.Name = "panel6";
        panel6.Size = new System.Drawing.Size(187, 48);
        panel6.TabIndex = 1;
        // 
        // radioButton2
        // 
        radioButton2.Location = new System.Drawing.Point(0, 0);
        radioButton2.Name = "radioButton2";
        radioButton2.Size = new System.Drawing.Size(104, 24);
        radioButton2.TabIndex = 0;
        // 
        // radioButton3
        // 
        radioButton3.Location = new System.Drawing.Point(0, 0);
        radioButton3.Name = "radioButton3";
        radioButton3.Size = new System.Drawing.Size(104, 24);
        radioButton3.TabIndex = 0;
        // 
        // radioButton4
        // 
        radioButton4.Location = new System.Drawing.Point(0, 0);
        radioButton4.Name = "radioButton4";
        radioButton4.Size = new System.Drawing.Size(104, 24);
        radioButton4.TabIndex = 0;
        // 
        // radioButton5
        // 
        radioButton5.Location = new System.Drawing.Point(0, 0);
        radioButton5.Name = "radioButton5";
        radioButton5.Size = new System.Drawing.Size(104, 24);
        radioButton5.TabIndex = 0;
        // 
        // radioButton6
        // 
        radioButton6.Location = new System.Drawing.Point(0, 0);
        radioButton6.Name = "radioButton6";
        radioButton6.Size = new System.Drawing.Size(104, 24);
        radioButton6.TabIndex = 0;
        // 
        // TopBar
        // 
        TopBar.AutoSize = true;
        TopBar.Controls.Add(TobBarFlowLayout);
        TopBar.Dock = System.Windows.Forms.DockStyle.Top;
        TopBar.Location = new System.Drawing.Point(0, 0);
        TopBar.Name = "TopBar";
        TopBar.Size = new System.Drawing.Size(1000, 48);
        TopBar.TabIndex = 0;
        // 
        // TobBarFlowLayout
        // 
        TobBarFlowLayout.AutoSize = true;
        TobBarFlowLayout.BackColor = System.Drawing.SystemColors.Highlight;
        TobBarFlowLayout.Controls.Add(TitlePanel);
        TobBarFlowLayout.Controls.Add(ActionButtonsPanel);
        TobBarFlowLayout.Controls.Add(ColorsNumberFlow);
        TobBarFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
        TobBarFlowLayout.Location = new System.Drawing.Point(0, 0);
        TobBarFlowLayout.Name = "TobBarFlowLayout";
        TobBarFlowLayout.Size = new System.Drawing.Size(1000, 48);
        TobBarFlowLayout.TabIndex = 0;
        // 
        // TitlePanel
        // 
        TitlePanel.BackColor = System.Drawing.Color.Transparent;
        TitlePanel.Controls.Add(Title);
        TitlePanel.Location = new System.Drawing.Point(847, 3);
        TitlePanel.Name = "TitlePanel";
        TitlePanel.Size = new System.Drawing.Size(150, 42);
        TitlePanel.TabIndex = 0;
        // 
        // Title
        // 
        Title.Dock = System.Windows.Forms.DockStyle.Fill;
        Title.Location = new System.Drawing.Point(0, 0);
        Title.Name = "Title";
        Title.Size = new System.Drawing.Size(150, 42);
        Title.TabIndex = 0;
        Title.Text = "مخبر الصور - PixelLab";
        Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        // 
        // ActionButtonsPanel
        // 
        ActionButtonsPanel.AutoSize = true;
        ActionButtonsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        ActionButtonsPanel.Controls.Add(ActionButtonsFlow);
        ActionButtonsPanel.Location = new System.Drawing.Point(378, 3);
        ActionButtonsPanel.Name = "ActionButtonsPanel";
        ActionButtonsPanel.Size = new System.Drawing.Size(463, 42);
        ActionButtonsPanel.TabIndex = 1;
        // 
        // ActionButtonsFlow
        // 
        ActionButtonsFlow.AutoSize = true;
        ActionButtonsFlow.Controls.Add(LoadButton);
        ActionButtonsFlow.Controls.Add(ResetButton);
        ActionButtonsFlow.Controls.Add(SaveButton);
        ActionButtonsFlow.Controls.Add(Open3DSpaceButton);
        ActionButtonsFlow.Location = new System.Drawing.Point(3, 3);
        ActionButtonsFlow.Name = "ActionButtonsFlow";
        ActionButtonsFlow.Size = new System.Drawing.Size(457, 36);
        ActionButtonsFlow.TabIndex = 0;
        ActionButtonsFlow.WrapContents = false;
        // 
        // LoadButton
        // 
        LoadButton.AutoSize = true;
        LoadButton.Location = new System.Drawing.Point(357, 3);
        LoadButton.Name = "LoadButton";
        LoadButton.Size = new System.Drawing.Size(97, 30);
        LoadButton.TabIndex = 3;
        LoadButton.Text = "تحميل صورة";
        LoadButton.UseVisualStyleBackColor = true;
        // 
        // ResetButton
        // 
        ResetButton.AutoSize = true;
        ResetButton.Location = new System.Drawing.Point(261, 3);
        ResetButton.Name = "ResetButton";
        ResetButton.Size = new System.Drawing.Size(90, 30);
        ResetButton.TabIndex = 2;
        ResetButton.Text = "إعادة تعيين";
        ResetButton.UseVisualStyleBackColor = true;
        // 
        // SaveButton
        // 
        SaveButton.AutoSize = true;
        SaveButton.Location = new System.Drawing.Point(184, 3);
        SaveButton.Name = "SaveButton";
        SaveButton.Size = new System.Drawing.Size(71, 30);
        SaveButton.TabIndex = 1;
        SaveButton.Text = "حفظ";
        SaveButton.UseVisualStyleBackColor = true;
        // 
        // Open3DSpaceButton
        // 
        Open3DSpaceButton.AutoSize = true;
        Open3DSpaceButton.Location = new System.Drawing.Point(3, 3);
        Open3DSpaceButton.Name = "Open3DSpaceButton";
        Open3DSpaceButton.Size = new System.Drawing.Size(175, 30);
        Open3DSpaceButton.TabIndex = 4;
        Open3DSpaceButton.Text = "فضاء الألوان ثلاثي الأبعاد";
        Open3DSpaceButton.UseVisualStyleBackColor = true;
        // 
        // ColorsNumberLabel
        // 
        ColorsNumberLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom));
        ColorsNumberLabel.AutoSize = true;
        ColorsNumberLabel.Location = new System.Drawing.Point(3, 0);
        ColorsNumberLabel.Name = "ColorsNumberLabel";
        ColorsNumberLabel.Size = new System.Drawing.Size(78, 33);
        ColorsNumberLabel.TabIndex = 0;
        ColorsNumberLabel.Text = "عدد الألوان";
        ColorsNumberLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        // 
        // ColorsNumberFlow
        // 
        ColorsNumberFlow.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom));
        ColorsNumberFlow.AutoSize = true;
        ColorsNumberFlow.Controls.Add(UpDownColorsNumber);
        ColorsNumberFlow.Controls.Add(ColorNumberSlider);
        ColorsNumberFlow.Controls.Add(ColorsNumberLabel);
        ColorsNumberFlow.Location = new System.Drawing.Point(65, 3);
        ColorsNumberFlow.Name = "ColorsNumberFlow";
        ColorsNumberFlow.Size = new System.Drawing.Size(307, 42);
        ColorsNumberFlow.TabIndex = 3;
        // 
        // ColorNumberSlider
        // 
        ColorNumberSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom));
        ColorNumberSlider.Location = new System.Drawing.Point(87, 3);
        ColorNumberSlider.Name = "ColorNumberSlider";
        ColorNumberSlider.Size = new System.Drawing.Size(158, 27);
        ColorNumberSlider.TabIndex = 1;
        // 
        // UpDownColorsNumber
        // 
        UpDownColorsNumber.AutoSize = true;
        UpDownColorsNumber.Dock = System.Windows.Forms.DockStyle.Fill;
        UpDownColorsNumber.Location = new System.Drawing.Point(251, 3);
        UpDownColorsNumber.Name = "UpDownColorsNumber";
        UpDownColorsNumber.Size = new System.Drawing.Size(53, 27);
        UpDownColorsNumber.TabIndex = 2;
        UpDownColorsNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
        // 
        // MainScreen
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        BackColor = System.Drawing.SystemColors.Control;
        ClientSize = new System.Drawing.Size(1000, 543);
        Controls.Add(TopBar);
        Location = new System.Drawing.Point(19, 19);
        Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        RightToLeftLayout = true;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        Load += MainScreen_Load;
        TopBar.ResumeLayout(false);
        TopBar.PerformLayout();
        TobBarFlowLayout.ResumeLayout(false);
        TobBarFlowLayout.PerformLayout();
        TitlePanel.ResumeLayout(false);
        ActionButtonsPanel.ResumeLayout(false);
        ActionButtonsPanel.PerformLayout();
        ActionButtonsFlow.ResumeLayout(false);
        ActionButtonsFlow.PerformLayout();
        ColorsNumberFlow.ResumeLayout(false);
        ColorsNumberFlow.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)ColorNumberSlider).EndInit();
        ((System.ComponentModel.ISupportInitialize)UpDownColorsNumber).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.TrackBar ColorNumberSlider;
    private System.Windows.Forms.NumericUpDown UpDownColorsNumber;

    private System.Windows.Forms.FlowLayoutPanel ColorsNumberFlow;

    private System.Windows.Forms.Button LoadButton;

    private System.Windows.Forms.Button SaveButton;

    private System.Windows.Forms.Button Open3DSpaceButton;
    private System.Windows.Forms.Button ResetButton;

    private System.Windows.Forms.FlowLayoutPanel ActionButtonsFlow;

    private System.Windows.Forms.Label Title;
    private System.Windows.Forms.Panel TitlePanel;

    private System.Windows.Forms.Label ColorsNumberLabel;

    private System.Windows.Forms.Panel ActionButtonsPanel;

    private System.Windows.Forms.FlowLayoutPanel TobBarFlowLayout;

    private System.Windows.Forms.Panel TopBar;

    private System.Windows.Forms.RadioButton radioButton2;
    private System.Windows.Forms.RadioButton radioButton3;
    private System.Windows.Forms.Panel panel6;
    private System.Windows.Forms.RadioButton radioButton4;
    private System.Windows.Forms.RadioButton radioButton5;
    private System.Windows.Forms.RadioButton radioButton6;

    #endregion
}