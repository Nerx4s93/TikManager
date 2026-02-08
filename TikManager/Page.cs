using Terminal.Gui;

internal abstract class Page
{
    public abstract string Title { get; }

    public abstract void Init(FrameView content);
    public abstract void Activate();
    public virtual void Deactivate() { }
}