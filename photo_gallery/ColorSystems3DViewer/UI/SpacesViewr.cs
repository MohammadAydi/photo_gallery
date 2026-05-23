namespace PixelLab;

public partial class SpacesViewr : Form
{
    private readonly HashSet<string> _initializedSpaces = new();
    private readonly Dictionary<string, IColorSpace> _spaces = new();
    private readonly Dictionary<string, int> _spaceSteps = new();
    private IColorSpace _activeSpace;
    private ColorFilter _filter;

    private bool _isDragging;
    private Point _lastMousePos;
    
    private OpenGLRenderer _renderer;
    private bool _wasDragging;

    public SpacesViewr()
    {
        InitializeComponent();
    }

    private void glControl_Load(object sender, EventArgs e)
    {
        _renderer = new OpenGLRenderer(glControl);
        _renderer.Setup();

        // pre-register all spaces — NOT initialized yet
        _spaces["RGB"] = new RgbColorSpace();
        _spaces["CMY"] = new CmyColorSpace();
        _spaces["HSV"] = new HsvColorSpace();
        _spaces["HSL"] = new HslColorSpace();
        _spaces["L*a*b"] = new LabColorSpace();
        _spaces["YCbCr"] = new YCbCrColorSpace();
        _spaces["YUV"] = new YuvColorSpace();

        foreach (var key in _spaces.Keys)
            _spaceSteps[key] = 3;

        _stepPanel.StepChanged += step =>
        {
            var key = lstColorSpaces.SelectedItem?.ToString() ?? "RGB";
            _spaceSteps[key] = step;
            _activeSpace.Rebuild(step);
            glControl.Invalidate();
        };
        glControl.MouseWheel += glControl_MouseWheel;
        filterPanel2.FilterChanged += OnFilterChanged;
        _renderer = new OpenGLRenderer(glControl);
        _renderer.Setup();

        lstColorSpaces.Items.Clear();
        lstColorSpaces.Items.AddRange(_spaces.Keys.ToArray<object>());
        lstColorSpaces.SelectedIndex = 0;
    }

    private void lstColorSpaces_SelectedIndexChanged(object sender, EventArgs e)
    {
        var key = lstColorSpaces.SelectedItem?.ToString() ?? "RGB";
        var space = _spaces[key];

        if (!_initializedSpaces.Contains(key))
        {
            space.Init();
            _initializedSpaces.Add(key);
        }

        // restore this space's step
        var savedStep = _spaceSteps.GetValueOrDefault(key, 3);
        _stepPanel.SetStep(savedStep);

        SetActiveSpace(space);
    }

    private void SetActiveSpace(IColorSpace space)
    {
        _activeSpace = space;
        _filter = space.CreateDefaultFilter();
        filterPanel2.LoadFilter(_filter);
        ShowPickedColor();
        glControl.Invalidate();
    }

    private (float r, float g, float b) _lastPickedRgb = (0.5f, 0.5f, 0.5f);

    private void OnFilterChanged()
    {
        if (_filter.PointChannels != null)
        {
            // convert current space's point channels back to RGB
            var (r, g, b) = _activeSpace.ChannelsToRgb(_filter.PointChannels);
            _lastPickedRgb = (r, g, b);
            ShowPickedColor();
        }

        glControl.Invalidate();
    }

    private void ShowPickedColor()
    {
        var (r, g, b) = _lastPickedRgb;

        var all = ColorMath.AllFromRgb(r, g, b);

        string key     = GetActiveSpaceKey();
        var    current = all[key];

        _valuesPanel.UpdateValues(
            key,
            current.values,
            current.names,
            Color.FromArgb(
                Math.Clamp((int)(r * 255), 0, 255),
                Math.Clamp((int)(g * 255), 0, 255),
                Math.Clamp((int)(b * 255), 0, 255)),
            all);
    }

    private string GetActiveSpaceKey()
    {
        return lstColorSpaces.SelectedItem?.ToString() ?? "RGB";
    }

    private void glControl_Paint(object sender, PaintEventArgs e)
    {
        if (_renderer == null || _activeSpace == null || _filter == null)
            return;

        _renderer.BeginFrame();
        var mvp = _renderer.Camera.GetMVP(_renderer.Aspect);
        _activeSpace.Draw(_filter, mvp); // core path
        _renderer.EndFrame();
    }

    private void glControl_Resize(object sender, EventArgs e)
    {
        glControl.Invalidate();
    }

    private void glControl_MouseDown(object sender, MouseEventArgs e)
    {
        _isDragging = true;
        _wasDragging = false;
        _lastMousePos = e.Location;
    }

    private void glControl_MouseUp(object sender, MouseEventArgs e)
    {
        _isDragging = false;
    }

    private void glControl_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDragging) return;

        float dx = e.X - _lastMousePos.X;
        float dy = e.Y - _lastMousePos.Y;

        if (Math.Abs(dx) > 2 || Math.Abs(dy) > 2)
            _wasDragging = true;

        _renderer.Camera.Pan(dx, dy);
        _lastMousePos = e.Location;
        glControl.Invalidate();
    }

    private void glControl_MouseWheel(object sender, MouseEventArgs e)
    {
        _renderer.Camera.Scroll(e.Delta);
        glControl.Invalidate();
    }
}