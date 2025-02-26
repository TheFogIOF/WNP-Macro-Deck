using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;
using WNPReduxAdapterLibrary;

namespace TheFogIOF.WNPPlugin.Actions
{
    public class PreviousAction : PluginAction
    {
        public override string Name => "Previous";

        public override string Description => "Previous track";

        public override bool CanConfigure => false;

        public override void Trigger(string clientId, ActionButton actionButton)
        {
            if (WNPRedux.MediaInfo.Controls.SupportsSkipPrevious)
            {
                WNPRedux.MediaInfo.Controls.TrySkipPrevious();   
            }
        }
    }
}