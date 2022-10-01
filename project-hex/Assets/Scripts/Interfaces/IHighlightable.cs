// Everyobject that implements this interface is highlighted when mouse is hovered on top of it.

public interface IHighlightable
{
    // How highlighted the object should be. 0 = not highlighted, 1 = semi-highlighted, 2 = super high
    public void SetHighlightLevel(int levelOfHighlight);
    public bool IsPlayable();
}
