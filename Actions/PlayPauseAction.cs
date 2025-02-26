using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;
using WNPReduxAdapterLibrary;

namespace TheFogIOF.WNPPlugin.Actions
{
    public class PlayPauseAction : PluginAction
    {
        public override string Name => "Play/Pause";

        public override string Description => "Plays or pauses the current track for all supported tabs";

        public override bool CanConfigure => false;

        public override string BindableVariable => "wnp_is_playing";

        public override void Trigger(string clientId, ActionButton actionButton)
        {
            if (WNPRedux.MediaInfo.Controls.SupportsPlayPause)
            {
                WNPRedux.MediaInfo.Controls.TryTogglePlayPause();
            }
        }
    }
}