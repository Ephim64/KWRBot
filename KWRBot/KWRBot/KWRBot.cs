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
        private bool newConfig = false;
        private string[] ConfuQuote = new string[]
        {
            "Три пути ведут к знанию: путь размышления — это путь самый благородный, путь подражания — это путь самый легкий и путь опыта — это путь самый горький.",
            "Если ты ненавидишь – значит тебя победили.",
            "В стране, где есть порядок, будь смел и в действиях, и в речах. В стране, где нет порядка, будь смел в действиях, но осмотрителен в речах.",
            "Перед тем как мстить, вырой две могилы.",
            "Давай наставления только тому, кто ищет знаний, обнаружив свое невежество.",
            "Счастье — это когда тебя понимают, большое счастье — это когда тебя любят, настоящее счастье — это когда любишь ты.",
            "На самом деле, жизнь проста, но мы настойчиво её усложняем.",
            "Несдержанность в мелочах погубит великое дело.",
            "Лишь когда приходят холода, становится ясно, что сосны и кипарисы последними теряют свой убор.",
            "Люди в древности не любили много говорить. Они считали позором для себя не поспеть за собственными словами.",
            "Советы мы принимаем каплями, зато раздаём ведрами.",
            "Драгоценный камень нельзя отполировать без трения. Также и человек не может стать успешным без достаточного количества трудных попыток.",
            "Благородный человек предъявляет требования к себе, низкий человек предъявляет требования к другим.",
            "Побороть дурные привычки можно только сегодня, а не завтра.",
            "Три вещи никогда не возвращаются обратно – время, слово, возможность. Поэтому: не теряй времени, выбирай слова, не упускай возможность.",
            "Выберите себе работу по душе, и вам не придется работать ни одного дня в своей жизни.",
            "Я не огорчаюсь, если люди меня не понимают, — огорчаюсь, если я не понимаю людей.",
            "Попытайтесь быть хотя бы немного добрее, и вы увидите, что будете не в состоянии совершить дурной поступок.",
            "В древности люди учились для того, чтобы совершенствовать себя. Нынче учатся для того, чтобы удивить других.",
            "Можно всю жизнь проклинать темноту, а можно зажечь маленькую свечку.",
            "Пришло несчастье – человек породил его, пришло счастье – человек его вырастил.",
            "Красота есть во всем, но не всем дано это видеть.",
            "Благородный в душе безмятежен. Низкий человек всегда озабочен.",
            "Если тебе плюют в спину, значит ты впереди.",
            "Не тот велик, кто никогда не падал, а тот велик – кто падал и вставал."
        };

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
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json")) && !this.newConfig)
                this._config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("settings.json"));
            else
            {
                this._config = new Config();
                this.newConfig = true;
            }
            if (this._config.CommandPrefix.ToString().Length == 0 && this.newConfig)
                this._config.CommandPrefix = '!';
            if(this._config.BotEmail.Length == 0)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write("Error! No password were found!\nEnter bot's email: ");
                this._config.BotEmail = Console.ReadLine();
                Console.BackgroundColor = ConsoleColor.Black;
            }
            if (this._config.BotPass.Length == 0 && this.newConfig)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write("Error! No password were found!\nEnter bot's password: ");
                this._config.BotPass = Console.ReadLine();
                Console.BackgroundColor = ConsoleColor.Black;
            }
            if(this.newConfig)
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json"), JsonConvert.SerializeObject(this._config));
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
                if (e.Author.ID != _config.OwnerID && e.Author.ID != this._config.BotID)
                { Console.WriteLine($"Messag received from {e.Author.Username}: \"{e.MessageText}\" id:{e.Author.ID}"); }
                if (e.Message.Content.Length > 0 && e.Message.Content[0] == this._config.CommandPrefix && e.Author.ID != this._config.BotID)
                {
                    string rawCommand = e.Message.Content.Substring(1);
                    CommandsManager.ExecuteOnMessageCommand(rawCommand, e.Channel, e.Author);
                }
            };
            this._client.PrivateMessageReceived += (sender, e) =>
            {
                if(e.Author.ID != _config.OwnerID)
                    Console.WriteLine($"Private message received from {e.Author.Username}: \"{e.Message}\"");
                if (e.Message.Length > 0 && e.Message[0] == this._config.CommandPrefix)
                {
                    string rawCommand = e.Message.Substring(1);
                    if (e.Author.ID == this._config.OwnerID)
                    {
                        this._client.AcceptInvite(rawCommand.Substring(rawCommand.LastIndexOf('/') + 1));
                    }
                }
                if (e.Message.StartsWith("?authenticate"))
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write("Kerrang!\nIn order to become a owner type in bot's password: ");
                    if (Console.ReadLine() == this._config.BotPass)
                    {
                        CommandsManager.AddPermission(e.Author, PermissionType.Owner);
                        this._config.OwnerID = e.Author.ID;
                        Console.WriteLine($"I'm welcoming you, my current owner {e.Author.Username}");
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    e.Author.SlideIntoDMs("Whelcome long lost father!");
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json"), JsonConvert.SerializeObject(this._config));
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
            CommandsManager.AddCommand(new CommandStub("about", "Tells about bot.", "", PermissionType.User, 1, cmdArgs =>
                {
                    this._client.SendMessageToChannel("Currently being in development this bot will assists moderators and administrators in managing server and channels.\nAuthor: Reverendo\nCourtesy: Big thanks to Luigifan", cmdArgs.Channel);
                }));
            CommandsManager.AddCommand(new CommandStub("help", "Provide discriptions to all avaliable commands", "", PermissionType.User, cmdArgs =>
               {
                   string help = String.Format("inComplete set of commands:\n");
                   foreach (var command in CommandsManager.Commands)
                   {
                       help += String.Format("{0,-15} {1,-100}\n", this._config.CommandPrefix + command.CommandName,"-" + command.Description);
                   }
               this._client.SendMessageToChannel(help, cmdArgs.Channel);
               }));
            CommandsManager.AddCommand(new CommandStub("join", "Join a specified server. Ownder only.", "", PermissionType.Owner, 1, cmdArgs =>
            {
                string substring = cmdArgs.Args[0].Substring(cmdArgs.Args[0].LastIndexOf('/') + 1);
                this._client.AcceptInvite(substring);
                Console.WriteLine($"Joined new server.");
                cmdArgs.Author.SlideIntoDMs("Joined server you requested!");
            }));
            CommandsManager.AddCommand(new CommandStub("wakeup", $"Mentions user multiple times. Example: {this._config.CommandPrefix}wakeup Username n-times", "specify victim", PermissionType.User, 1, cmdArgs =>
               {
                   string substring = cmdArgs.Args[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
                   var victim = this._client.GetMemberFromChannel(cmdArgs.Channel, substring, false);
                   var channel = cmdArgs.Channel;
                   for (int i = 0; i < int.Parse(cmdArgs.Args[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]); i++)
                   {
                       this._client.SendMessageToChannel($"@{victim.Username} wake up!", cmdArgs.Channel);
                   }
               }));
            CommandsManager.AddCommand(new CommandStub("deletemsg", $"Delete last n messages. Example: {this._config.CommandPrefix}deletemsg n", "specify amount of messages", PermissionType.User, 1, cmdArgs =>
                 {
                     int amount = 0;
                     int.TryParse(cmdArgs.Args[0], out amount);
                     this._client.DeleteMultipleMessagesInChannel(cmdArgs.Channel, amount + 1);
                 }));
            CommandsManager.AddCommand(new CommandStub("q", "Posts random quote", "", PermissionType.User, 1, cmdArgs =>
                {
                    this._client.SendMessageToChannel("\"" + ConfuQuote[new Random().Next(ConfuQuote.Length)] + "\"" + " - Конфуций", cmdArgs.Channel);
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
