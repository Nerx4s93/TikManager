using Terminal.Gui;

internal abstract class Page
{
    public abstract int Index { get; }
    public abstract string Title { get; }

    public abstract void Init(FrameView content);
    public abstract void Activate();
    public abstract void Deactivate();
}