using System;
using System.Collections.Generic;
using System.Reflection;
using TheFogIOF.WNPPlugin.Actions;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Variables;
using System.Threading.Tasks;
using System.Timers;
using WNPReduxAdapterLibrary;
using SuchByte.MacroDeck.ActionButton;
using System.Drawing;

namespace TheFogIOF.WNPPlugin
{
    public static class PluginInstance
    {
        public static Main Main { get; set; }
    }

    public class Main : MacroDeckPlugin
    {
        public static Main Instance;
        private Timer _Timer;

        public Main()
        {
            PluginInstance.Main = this;
        }

        public override bool CanConfigure => false;

        public override async void Enable()
        {
            try
            {
                Instance ??= this;
                Actions = new List<PluginAction>
                {
                    new PlayPauseAction(),
                    new PreviousAction(),
                    new NextAction(),
                    new ShuffleAction(),
                    new RepeatAction(),
                    new UpdateStatesAction()
                };
                await InitializeStateAsync();
                await Init();

                _Timer = new Timer(500)
                {
                    Enabled = true
                };
                _Timer.Elapsed += OnTimerTick;
                _Timer.Start();
            }
            catch (Exception e)
            {
                MacroDeckLogger.Error(this, $"There is a error.\r\n{e}");
            }
        }

        private async Task Init()
        {
            var assembly = Assembly.GetExecutingAssembly().GetName().Version;
            string version = $"{assembly.Major}.{assembly.Minor}.{assembly.Build}";
            WNPRedux.Start(8698, version, ((type, s) => Logger((int)type, s)));
            WNPReduxNative.Start(8698);
        }

        public static async Task InitializeStateAsync()
        {
            var mediainfo = WNPRedux.MediaInfo;

            VariableManager.SetValue("wnp_title", mediainfo.Title, VariableType.String, Main.Instance, null!);
            VariableManager.SetValue("wnp_album", mediainfo.Album, VariableType.String, Main.Instance, null!);
            VariableManager.SetValue("wnp_artist", mediainfo.Artist, VariableType.String, Main.Instance, null!);
            VariableManager.SetValue("wnp_position", mediainfo.Position, VariableType.String, Main.Instance, null!);
            VariableManager.SetValue("wnp_pos_percent", mediainfo.PositionPercent, VariableType.Float, Main.Instance, null!);
            VariableManager.SetValue("wnp_duration", mediainfo.Duration, VariableType.String, Main.Instance, null!);
            VariableManager.SetValue("wnp_player", mediainfo.PlayerName, VariableType.String, Main.Instance, null!);
            VariableManager.SetValue("wnp_state", mediainfo.State, VariableType.Integer, Main.Instance, null!);
            VariableManager.SetValue("wnp_volume", mediainfo.Volume, VariableType.Integer, Main.Instance, null!);
            VariableManager.SetValue("wnp_shuffle", mediainfo.ShuffleActive, VariableType.Bool, Main.Instance, null!);
            VariableManager.SetValue("wnp_repeatone", mediainfo.RepeatMode == MediaInfo.RepeatModeEnum.ONE, VariableType.Bool, Main.Instance, null!);
            VariableManager.SetValue("wnp_repeatall", mediainfo.RepeatMode == MediaInfo.RepeatModeEnum.ALL, VariableType.Bool, Main.Instance, null!);
            VariableManager.SetValue("wnp_is_playing", mediainfo.State == MediaInfo.StateMode.PLAYING, VariableType.Bool, Main.Instance, null!);
            VariableManager.SetValue("wnp_repeat", mediainfo.RepeatMode != MediaInfo.RepeatModeEnum.NONE, VariableType.Bool, Main.Instance, null!);
            VariableManager.SetValue("wnp_music_bar", MusicBar(), VariableType.String, Main.Instance, null!);
        }

        private async void OnTimerTick(object sender, EventArgs e)
        {
            await InitializeStateAsync();
        }

        public void Logger(int type, string message)
        {
            if (type == 0)
                MacroDeckLogger.Info(PluginInstance.Main, message);
            else if (type == 1)
                MacroDeckLogger.Warning(PluginInstance.Main, message);
            else
                MacroDeckLogger.Error(PluginInstance.Main, message);
        }

        public static string MusicBar()
        {
            var mediainfo = WNPRedux.MediaInfo;
            int percent = Convert.ToInt32(Math.Round(mediainfo.PositionPercent * 0.9));

            Font font = new Font("Segoe UI Symbol", 4, FontStyle.Regular);
            Bitmap bitmap = new Bitmap(1, 1);
            Graphics graphics = Graphics.FromImage(bitmap);

            SizeF size_bit_0 = new SizeF();
            size_bit_0 = graphics.MeasureString(" ", font);
            SizeF size_bit_1 = new SizeF();
            size_bit_1 = graphics.MeasureString("  ", font);

            var size_bit = size_bit_1.Width - size_bit_0.Width;

            string bar_0 = new string('▏', 90);
            SizeF size_full = new SizeF();
            size_full = graphics.MeasureString(bar_0, font);

            string bar_1 = new string('▏', percent);
            SizeF size_current = new SizeF();
            size_current = graphics.MeasureString(bar_1, font);

            var size_empty = size_full.Width - size_current.Width;

            int empty_percent = Convert.ToInt32(Math.Round(size_empty / size_bit));
            string bar = "";
            if (percent >= 0 && empty_percent >= 0) bar = new string('▏', percent) + new string(' ', empty_percent);
            return bar;
        }

        public class UpdateStatesAction : PluginAction
        {
            public override string Name => "Update States";

            public override string Description => "Instant Update All Plugin States";

            public override bool CanConfigure => false;

            public override async void Trigger(string clientId, ActionButton actionButton)
            {
                await InitializeStateAsync();
            }
        }
    }
}