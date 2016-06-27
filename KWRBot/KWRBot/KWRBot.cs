using DiscordSharp;
using DiscordSharp.Commands;
using DiscordSharp.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KWRBot
{
    public class KWRBot
    {
        private CommandsManager CommandsManager;
        private Config _config;
        private DiscordClient _client;
        public KWRBot()
        {
            Initialise();
            SetUpCommands();
            SetUpEvents();
        }

        /// <summary>
        /// Start-up settings
        /// </summary>
        private void Initialise()
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"settings.json")))
                this._config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("settings.json"));
            else this._config = new Config();
            if (this._config.CommandPrefix.ToString().Length == 0)
                this._config.CommandPrefix = '!';
            _client = new DiscordClient();
            this._client.ClientPrivateInformation.Email = this._config.BotEmail;
            this._client.ClientPrivateInformation.Password = this._config.BotPass;
            CommandsManager = new CommandsManager(this._client);
        }

        private void SetUpEvents()
        {
            this._client.Connected += (sender, e) =>
            {
                Console.Title = $"KWRBot - Discord - logged in as {e.User.Username}";
                Console.WriteLine($"Connected! Username: {e.User.Username}");
            };
            this._client.MessageReceived += (sender, e) =>
            {
                Console.WriteLine($"Messag received from {e.Author.Username}: \"{e.MessageText}\"");
                if(e.Message.Content.Length > 0 && e.Message.Content[0] == this._config.CommandPrefix)
                {
                    string rawCommand = e.Message.Content.Substring(1);
                    CommandsManager.ExecuteOnMessageCommand(rawCommand, e.Channel, e.Author);
                }
            };
            this._client.PrivateMessageReceived += (sender, e) =>
            {
                Console.WriteLine($"Private message received from {e.Author.Username}: \"{e.Message}\"");
                if (e.Message.Length > 0 && e.Message[0] == this._config.CommandPrefix)
                {
                    string rawCommand = e.Message.Substring(1);
                    if(e.Author.ID == this._config.OwnerID)
                    {
                        this._client.AcceptInvite(rawCommand.Substring(rawCommand.LastIndexOf('/') + 1));
                    }
                }
            };
            this._client.GuildCreated += (sender, e) =>
            {
                Console.WriteLine($"Joined server: {e.Server.Name} ({e.Server.ID})");
            };
        }

        /// <summary>
        /// Setting up some commands
        /// </summary>
        private void SetUpCommands()
        {
            CommandsManager.AddCommand(new CommandStub("join", "Join a specified server", "", PermissionType.Owner, 1, cmdArgs =>
            {
                string substring = cmdArgs.Args[0].Substring(cmdArgs.Args[0].LastIndexOf('/') + 1);
                _client.AcceptInvite(substring);
                Console.WriteLine("Connecting");
            }));
            CommandsManager.AddCommand(new CommandStub("wakeup", "Mentions user multiple times", "specify victim", PermissionType.User, 1, cmdArgs =>
               {
                   string substring = cmdArgs.Args[0].Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries)[0];
                   var victim = this._client.GetMemberFromChannel(cmdArgs.Channel, substring, false);
                   var channel = cmdArgs.Channel;
                   for (int i = 0; i < int.Parse(cmdArgs.Args[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]); i++)
                   {
                       this._client.SendMessageToChannel($"@{victim.Username} wake up!", cmdArgs.Channel);
                   }
               }));
        }

        public void Login()
        {
            if (this._client.SendLoginRequest() != null) _client.Connect();
        }

        public void Exit()
        {
            _client.Logout();
            _client.Dispose();
            Environment.Exit(0);
        }
    }
}
