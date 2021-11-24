namespace HoveyTech.SearchableComboBox
{
#if WINDOWS_UWP
    internal class WrapPanel : Microsoft.Toolkit.Uwp.UI.Controls.WrapPanel
#else
    internal class WrapPanel : CommunityToolkit.WinUI.UI.Controls.WrapPanel
#endif
    {
    }
}
