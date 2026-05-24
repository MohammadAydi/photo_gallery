namespace PixelLab;

public partial class StepControlPanel : UserControl
{
    private static readonly Color BgDark = Color.FromArgb(50, 50, 50);
    private static readonly Color Accent = Color.FromArgb(85, 79, 179);
    private static readonly Color TextLight = Color.FromArgb(230, 230, 230);



    public event Action<int>? StepChanged;

    public StepControlPanel()
    {
        InitializeComponent();
    }

    public void SetStep(int step)
    {
        _stepTrack.Value = Math.Clamp(step, 1, 10);
        _stepValueLabel.Text = $"Step: {step}";
    }


    private void _stepTrack_ValueChanged(object sender, EventArgs e)
    {
        _stepValueLabel.Text = $"Step: {_stepTrack.Value}";
    }


    private void _applyBtn_MouseClick(object sender, MouseEventArgs e)
    {
        StepChanged?.Invoke(_stepTrack.Value);
    }
}