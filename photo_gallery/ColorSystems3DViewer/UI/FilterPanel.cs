namespace PixelLab;

public partial class FilterPanel : UserControl
{
    private static readonly Color BgDark = Color.FromArgb(50, 50, 50);
    private static readonly Color BgMid = Color.FromArgb(70, 70, 70);
    private static readonly Color Accent = Color.FromArgb(85, 79, 179);
    private static readonly Color TextLight = Color.FromArgb(230, 230, 230);
    private static readonly Color BorderGray = Color.FromArgb(130, 130, 130);

    private readonly List<(TrackBar min, TrackBar max)> _clipTracks = new();
    private readonly List<TrackBar> _pointTracks = new();
    private Button _btnShowInSpace;

    private Panel _clipPanel;
    private ColorFilter _filter;
    private TrackBar _peelTrack;
    private Panel _pointPanel;
    private RadioButton _rbFreeClip;
    private RadioButton _rbPickPoint;
    private CheckBox _subtractCheck;

    public FilterPanel()
    {
        InitializeComponent();

        AutoScroll = true;
    }

    public event Action FilterChanged;

    private void ApplyTheme()
    {
        BackColor = BgDark;

        foreach (Control c in Controls)
            ThemeControl(c);

        // recurse into panels
        foreach (Control c in new[] { _clipPanel, _pointPanel })
        {
            if (c == null) continue;
            c.BackColor = BgDark;
            foreach (Control child in c.Controls)
                ThemeControl(child);
        }
    }

    private void ThemeControl(Control c)
    {
        switch (c)
        {
            case Label lbl:
                lbl.ForeColor = TextLight;
                lbl.BackColor = Color.Transparent;
                lbl.Font = new Font("Segoe UI", 9f);
                break;

            case RadioButton rb:
                rb.ForeColor = TextLight;
                rb.BackColor = Color.Transparent;
                rb.FlatStyle = FlatStyle.Flat;
                rb.Font = new Font("Segoe UI", 9f);
                break;

            case CheckBox cb:
                cb.ForeColor = TextLight;
                cb.BackColor = Color.Transparent;
                cb.FlatStyle = FlatStyle.Flat;
                cb.Font = new Font("Segoe UI", 9f);
                break;

            case Button btn:
                btn.BackColor = Color.FromArgb(31, 31, 31);
                btn.ForeColor = Color.FromArgb(85, 79, 179);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = Color.FromArgb(85, 79, 179);
                btn.FlatAppearance.BorderSize = 2;
                btn.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
                break;

            case TrackBar tb:
                tb.BackColor = BgDark;
                break;
        }
    }

    public void LoadFilter(ColorFilter filter)
    {
        _filter = filter;
        _clipTracks.Clear();
        _pointTracks.Clear();
        Controls.Clear();

        BuildModeToggle();
        BuildClipPanel();
        BuildPointPanel();

        Controls.Add(_clipPanel);
        Controls.Add(_pointPanel);

        SetMode(true);
        ApplyTheme();
    }

    private void BuildModeToggle()
    {
        _rbFreeClip = new RadioButton
        {
            Text = "Free Clip",
            Location = new Point(4, 4),
            AutoSize = true,
            Checked = true
        };
        _rbPickPoint = new RadioButton
        {
            Text = "Pick Point",
            Location = new Point(100, 4),
            AutoSize = true
        };

        _rbFreeClip.CheckedChanged += (_, _) =>
        {
            if (_rbFreeClip.Checked) SetMode(true);
        };
        _rbPickPoint.CheckedChanged += (_, _) =>
        {
            if (_rbPickPoint.Checked) SetMode(false);
        };

        Controls.Add(_rbFreeClip);
        Controls.Add(_rbPickPoint);
    }

    private void BuildClipPanel()
    {
        _clipPanel = new Panel
        {
            Location = new Point(0, 30),

            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        var header = new Label
        {
            Text = "— CLIP RANGES —",
            Location = new Point(4, 0),
            AutoSize = true,
            ForeColor = Color.FromArgb(85, 79, 179),
            Font = new Font("Segoe UI", 8f, FontStyle.Bold)
        };
        _clipPanel.Controls.Add(header);
        var y = 20; // start y after header


        for (var i = 0; i < _filter.ChannelNames.Length; i++)
        {
            var idx = i;

            var lbl = new Label { Text = _filter.ChannelNames[i], Location = new Point(4, y), AutoSize = true };

            var minTrack = new TrackBar
            {
                Minimum = 0, Maximum = 100, Value = 0,
                Location = new Point(30, y - 4),
                Width = 120, Height = 30, TickFrequency = 10
            };
            var maxTrack = new TrackBar
            {
                Minimum = 0, Maximum = 100, Value = 100,
                Location = new Point(155, y - 4),
                Width = 120, Height = 30, TickFrequency = 10
            };

            minTrack.ValueChanged += (_, _) =>
            {
                _filter.Ranges[idx] = (minTrack.Value / 100f, _filter.Ranges[idx].Max);
                FilterChanged?.Invoke();
            };
            maxTrack.ValueChanged += (_, _) =>
            {
                _filter.Ranges[idx] = (_filter.Ranges[idx].Min, maxTrack.Value / 100f);
                FilterChanged?.Invoke();
            };

            _clipPanel.Controls.AddRange(lbl, minTrack, maxTrack);
            _clipTracks.Add((minTrack, maxTrack));
            y += 60;
        }

        // peel
        var peelLbl = new Label { Text = "Peel", Location = new Point(4, y), AutoSize = true };
        _peelTrack = new TrackBar
        {
            Minimum = 0, Maximum = 100, Value = 100,
            Location = new Point(30, y - 4),
            Width = 250, Height = 30, TickFrequency = 10
        };
        _peelTrack.ValueChanged += (_, _) =>
        {
            _filter.Peel = _peelTrack.Value / 100f;
            FilterChanged?.Invoke();
        };

        _subtractCheck = new CheckBox
        {
            Text = "Subtract inner",
            Location = new Point(4, y + 60),
            AutoSize = true
        };
        _subtractCheck.CheckedChanged += (_, _) =>
        {
            _filter.SubtractInner = _subtractCheck.Checked;
            FilterChanged?.Invoke();
        };
        _clipPanel.Height = y + 20;
        _clipPanel.Controls.AddRange(peelLbl, _peelTrack, _subtractCheck);
    }

    private void BuildPointPanel()
    {
        _pointPanel = new Panel
        {
            Location = new Point(0, 30),

            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        var header = new Label
        {
            Text = "— PICK POINT —",

            Location = new Point(4, 0),
            AutoSize = true,
            ForeColor = Color.FromArgb(85, 79, 179),
            Font = new Font("Segoe UI", 8f, FontStyle.Bold)
        };
        _clipPanel.Controls.Add(header);
        var y = 20; // start y after header


        for (var i = 0; i < _filter.ChannelNames.Length; i++)
        {
            var idx = i;

            var lbl = new Label { Text = _filter.ChannelNames[i], Location = new Point(4, y), AutoSize = true };

            var track = new TrackBar
            {
                Minimum = 0, Maximum = 100, Value = 50,
                Location = new Point(30, y - 4),
                Width = 250, Height = 30, TickFrequency = 10
            };

            track.ValueChanged += (_, _) =>
            {
                if (_filter.PointChannels == null) return;
                _filter.PointChannels[idx] = track.Value / 100f;
                UpdateColorPreview();
                FilterChanged?.Invoke();
            };

            _pointPanel.Controls.AddRange(lbl, track);
            _pointTracks.Add(track);
            y += 60;
        }

        _btnShowInSpace = new Button
        {
            Text = "Show in Space",
            Location = new Point(4, y + 4),
            Width = 220,
            Height = 80
        };
        _btnShowInSpace.Click += (_, _) =>
        {
            _filter.PeelToPoint();
            // sync peel track UI
            _peelTrack.Value = (int)(_filter.Peel * 100);
            FilterChanged?.Invoke();
        };
        _pointPanel.Height = y + 20;
        _pointPanel.Controls.Add(_btnShowInSpace);
    }

    private void SetMode(bool freeClip)
    {
        if (freeClip)
        {
            _filter.PointChannels = null;
            _clipPanel.Visible = true;
            _pointPanel.Visible = false;
        }
        else
        {
            // init point to center
            _filter.PointChannels = Enumerable.Repeat(0.5f, _filter.ChannelNames.Length).ToArray();
            _clipPanel.Visible = false;
            _pointPanel.Visible = true;
        }

        FilterChanged?.Invoke();
    }

    private void UpdateColorPreview()
    {
        // FilterPanel doesn't own the preview — raise event and let Form handle it
        FilterChanged?.Invoke();
    }
}