using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MechaMariBot
{
    public partial class MechaMariBot : ServiceBase
    {
        EventLog eventLog;
        DiscordSocketClient client;
        /*
         * TODO:
         * Dak, zeke, lamb, diggs, parsons, ring are other keywords
         * 
         * Reply with emote as well
         * 
         * combat skyrim? <:thunk:764195150045511720>
         */
        String[] triggerWords = { "cowboy", "cowboys" };
        ulong[] triggerIds = { 388872773109284876, 607723716977098753};
        ulong testServer = 410597263363276801;

        public MechaMariBot()
        {
            InitializeComponent();
            eventLog = new EventLog("Application");
            eventLog.Source = "MechaMariBot";
        }

        protected override void OnStart(string[] args)
        {
            RunDiscordClient();
        }

        protected override void OnStop()
        {
            if (client != null)
            {
                client.Dispose();
            }
        }

        private async Task RunDiscordClient()
        {
            try
            {
                client = new DiscordSocketClient();
                Dictionary<String, String> configuration = JsonConvert.DeserializeObject<Dictionary<String, String>>(File.ReadAllText("C:\\MechaMariBot\\config.json"));
                client.Log += Log;
                client.MessageReceived += MessageReceived;
                await client.LoginAsync(TokenType.Bot, configuration["token"]);
                await client.StartAsync();
                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                LogError("MechaMariBot encountered an error: \n\n" + ex.Message + "\n" + ex.StackTrace);
                this.ExitCode = -1;
                Environment.Exit(-1);
            }
        }


        private async Task MessageReceived(SocketMessage socketMessage)
        {
            var socketGuildChannel = (SocketGuildChannel)socketMessage.Channel;
            bool isTestServer = socketGuildChannel.Guild.Id == testServer;
            if (triggerIds.Contains(socketMessage.Author.Id) || isTestServer)
            {
                string[] words = socketMessage.Content.ToLower().Split(' ');
                if (words.Intersect(triggerWords).Count() > 0)
                {
                    var emote = Emote.Parse("<:cowboypium:885917545738174505>");
                    await socketMessage.AddReactionAsync(emote);
                }
            }
        }

        private Task Log(LogMessage msg)
        {
            switch (msg.Severity) {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    if (msg.Exception != null)
                    {
                        LogError(msg.Message + "\n\n" + msg.Exception.Message + "\n" + msg.Exception.StackTrace);
                    } else
                    {
                        LogError(msg.Message);
                    }
                    break;
                case LogSeverity.Warning:
                    if (msg.Exception != null)
                    {
                        LogWarn(msg.Message + "\n\n" + msg.Exception.Message + "\n" + msg.Exception.StackTrace);
                    }
                    else
                    {
                        LogWarn(msg.Message);
                    }
                    break;
                case LogSeverity.Info:
                    LogInfo(msg.Message);
                    break;
            }
            return Task.CompletedTask;
        }


        private void LogInfo(String log)
        {
            eventLog.WriteEntry(log, EventLogEntryType.Information);
        }

        private void LogWarn(String log)
        {
            eventLog.WriteEntry(log, EventLogEntryType.Warning);
        }

        private void LogError(String log)
        {
            eventLog.WriteEntry(log, EventLogEntryType.Error);
        }
    }
}
