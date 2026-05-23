namespace PixelLab;

public partial class StepControlPanel : UserControl
{
    private static readonly Color BgDark = Color.FromArgb(50, 50, 50);
    private static readonly Color Accent = Color.FromArgb(85, 79, 179);
    private static readonly Color TextLight = Color.FromArgb(230, 230, 230);
    private Button _applyBtn;

    private TrackBar _stepTrack;
    private Label _stepValueLabel;

    public StepControlPanel()
    {
        BackColor = BgDark;
        Height = 80;
        BuildLayout();
    }

    public int CurrentStep => _stepTrack.Value;

    public event Action<int>? StepChanged;

    private void BuildLayout()
    {
        var header = new Label
        {
            Text = "— POINT DENSITY —",
            AutoSize = true,
            ForeColor = Accent,
            Font = new Font("Segoe UI", 8f, FontStyle.Bold),
            Location = new Point(8, 6)
        };

        _stepTrack = new TrackBar
        {
            Minimum = 1,
            Maximum = 10,
            Value = 3,
            TickFrequency = 1,
            Location = new Point(8, 24),
            Width = 180,
            Height = 30,
            BackColor = BgDark
        };

        _stepValueLabel = new Label
        {
            Text = "Step: 3",
            AutoSize = true,
            ForeColor = TextLight,
            Font = new Font("Consolas", 9f),
            Location = new Point(195, 30)
        };

        _applyBtn = new Button
        {
            Text = "Apply",
            Location = new Point(255, 26),
            Width = 60,
            Height = 28,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(31, 31, 31),
            ForeColor = Accent,
            Font = new Font("Segoe UI", 8f, FontStyle.Bold)
        };
        _applyBtn.FlatAppearance.BorderColor = Accent;
        _applyBtn.FlatAppearance.BorderSize = 1;

        _stepTrack.ValueChanged += (_, _) =>
            _stepValueLabel.Text = $"Step: {_stepTrack.Value}";

        _applyBtn.Click += (_, _) =>
            StepChanged?.Invoke(_stepTrack.Value);

        Controls.AddRange(header, _stepTrack, _stepValueLabel, _applyBtn);
    }

    public void SetStep(int step)
    {
        _stepTrack.Value = Math.Clamp(step, 1, 10);
        _stepValueLabel.Text = $"Step: {step}";
    }
}