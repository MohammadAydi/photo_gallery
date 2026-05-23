namespace PixelLab;

public partial class ColorValuesPanel : UserControl
{
    private static readonly Color BgDark = Color.FromArgb(50, 50, 50);
    private static readonly Color BgMid = Color.FromArgb(70, 70, 70);
    private static readonly Color Accent = Color.FromArgb(85, 79, 179);
    private static readonly Color TextLight = Color.FromArgb(230, 230, 230);
    private static readonly Color TextDim = Color.FromArgb(140, 140, 140);
    private static readonly Font MonoFont = new("Consolas", 9f);
    private static readonly Font LabelFont = new("Segoe UI", 8f, FontStyle.Bold);
    private FlowLayoutPanel _allValuesFlow;

    private Panel _colorPreview;
    private Label _currentSpaceLabel;
    private Label _currentValueLabel;
    private Panel _divider;

    public ColorValuesPanel()
    {
        BackColor = BgDark;
        Padding = new Padding(8);
        BuildLayout();
    }

    private void BuildLayout()
    {
        _colorPreview = new Panel
        {
            Size = new Size(40, 40),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(80, 80, 80)
        };

        _currentSpaceLabel = new Label
        {
            AutoSize = true,
            ForeColor = Accent,
            Font = LabelFont,
            Text = "—"
        };

        _currentValueLabel = new Label
        {
            AutoSize = true,
            ForeColor = TextLight,
            Font = MonoFont,
            Text = "—"
        };

        _divider = new Panel
        {
            Height = 1,
            Dock = DockStyle.Top,
            BackColor = Color.FromArgb(80, 80, 80),
            Margin = new Padding(0, 6, 0, 6)
        };

        _allValuesFlow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            AutoSize = true,
            WrapContents = false,
            BackColor = BgDark
        };

        // top section: preview + current space
        var topFlow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            WrapContents = false,
            BackColor = BgDark,
            Margin = new Padding(0, 0, 0, 6)
        };

        var currentTextFlow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            AutoSize = true,
            WrapContents = false,
            BackColor = BgDark,
            Padding = new Padding(6, 0, 0, 0)
        };

        currentTextFlow.Controls.Add(_currentSpaceLabel);
        currentTextFlow.Controls.Add(_currentValueLabel);
        topFlow.Controls.Add(_colorPreview);
        topFlow.Controls.Add(currentTextFlow);

        var root = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            Dock = DockStyle.Fill,
            WrapContents = false,
            BackColor = BgDark
        };

        root.Controls.Add(topFlow);
        root.Controls.Add(_divider);
        root.Controls.Add(_allValuesFlow);

        Controls.Add(root);
    }

    // call this when a point is picked or sliders change
    public void UpdateValues(string currentSpace, float[] currentChannels,
        string[] channelNames, Color rgbColor,
        Dictionary<string, (string[] names, float[] values)> allSpaces)
    {
        // color preview
        _colorPreview.BackColor = rgbColor;

        // current space header
        _currentSpaceLabel.Text = currentSpace;

        // current space channel values
        _currentValueLabel.Text = FormatChannels(channelNames, currentChannels);

        // all spaces
        _allValuesFlow.Controls.Clear();

        foreach (var kv in allSpaces)
        {
            var isCurrent = kv.Key == currentSpace;

            var row = new Label
            {
                AutoSize = true,
                Font = MonoFont,
                ForeColor = isCurrent ? Accent : TextDim,
                BackColor = Color.Transparent,
                Text = $"{kv.Key,-7} {FormatChannels(kv.Value.names, kv.Value.values)}",
                Margin = new Padding(0, 2, 0, 2)
            };

            _allValuesFlow.Controls.Add(row);
        }
    }

    private string FormatChannels(string[] names, float[] values)
    {
        var parts = new List<string>();
        for (int i = 0; i < names.Length; i++)
        {
            string name = names[i];
            float  val  = values[i];

            string formatted = name switch
            {
                "H"              => $"{val:0}°",
                "R" or "G" or "B"=> $"{val:0}",       // 0-255
                "L" when val > 2f=> $"{val:0.0}",      // Lab L 0-100
                "a" or "b"       => $"{val:+0.0;-0.0}",// Lab a/b signed
                "Cb" or "Cr"     => $"{val:+0.000;-0.000}",
                "U"  or "V"      => $"{val:+0.000;-0.000}",
                _                => $"{val:0.000}"
            };

            parts.Add($"{name}:{formatted}");
        }
        return string.Join("  ", parts);
    }
}