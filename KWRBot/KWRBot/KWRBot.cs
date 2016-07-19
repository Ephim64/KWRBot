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
        private DiscordMember owner;
        private bool newConfig = false;
        private AudioPlayer ap;
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

        private string[] Ball8 = new string[]
        {
            "Бесспорно",
            "Предрешено",
            "Никаких сомнений",
            "Определённо да",
            "Можешь быть уверен в этом",
            "Мне кажется — «да»",
            "Вероятнее всего",
            "Хорошие перспективы",
            "Знаки говорят — «да»",
            "Да",
            "Пока не ясно, попробуй снова",
            "Спроси позже",
            "Лучше не рассказывать",
            "Сейчас нельзя предсказать",
            "Сконцентрируйся и спроси опять",
            "Даже не думай",
            "Мой ответ — «нет»",
            "По моим данным — «нет»",
            "Перспективы не очень хорошие",
            "Весьма сомнительно"
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
            if (File.Exists(Path.GetFullPath("settings.json")) && !this.newConfig)
                this._config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.GetFullPath("settings.json")));
            else
            {
                this._config = new Config();
                this.newConfig = true;
            }
            if (this._config.CommandPrefix.ToString() == string.Empty)
            {
                Console.WriteLine("Warning! No command prefix was found! Please, enter enter the prefix or it will be set to default (\"!\"");
                Console.Write("Command prefix: ");
                var prefix = Console.ReadLine();
                this._config.CommandPrefix = (prefix.Length == 0) ? '!' : prefix.ToCharArray()[0];
                this.newConfig = true;
            }
            if (this._config.BotEmail.Length == 0)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write("Error! No password were found!\nEnter bot's email: ");
                this._config.BotEmail = Console.ReadLine();
                Console.BackgroundColor = ConsoleColor.Black;
                this.newConfig = true;
            }
            if (this._config.BotPass.Length == 0)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write("Error! No password were found!\nEnter bot's password: ");
                this._config.BotPass = Console.ReadLine();
                Console.BackgroundColor = ConsoleColor.Black;
                this.newConfig = true;
            }
            if (this.newConfig)
                File.WriteAllText(Path.GetFullPath("settings.json"), JsonConvert.SerializeObject(this._config));
            _client = new DiscordClient();
            this._client.ClientPrivateInformation.Email = this._config.BotEmail;
            this._client.ClientPrivateInformation.Password = this._config.BotPass;
            CommandsManager = new CommandsManager(this._client);
            if(owner == null)
            {
                //var t = this._client.GetServerChannelIsIn().GetMemberByKey(this._config.OwnerID);
                //owner = this._client.GetServersList().Find(x => x.GetMemberByKey(this._config.OwnerID) != null).GetMemberByKey(this._config.OwnerID);
            }
            if (File.Exists(Path.GetFullPath("permissions.json")))
            {
                var permissionDictionary = JsonConvert.DeserializeObject<Dictionary<string, PermissionType>>(File.ReadAllText("permissions.json"));
                if(permissionDictionary == null)
                {
                    permissionDictionary = new Dictionary<string, PermissionType>();
                    if (owner != null) permissionDictionary.Add(owner.ID, PermissionType.Owner);
                }
                this.CommandsManager.OverridePermissionsDictionary(permissionDictionary);
            }
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
                if (owner == null)
                {
                    owner = this._client.GetServerChannelIsIn(e.Channel).GetMemberByKey(this._config.OwnerID);
                }
                if (e.Author != owner && e.Author.Username != this._client.ClientPrivateInformation.Username)
                { Console.WriteLine($"Messag received from {e.Author.Username}: \"{e.MessageText}\""); }
                if (e.Message.Content.Length > 0 && e.Message.Content[0] == this._config.CommandPrefix && e.Author.ID != this._config.BotID)
                {
                    string rawCommand = e.Message.Content.Substring(1);
                    try
                    {
                        CommandsManager.ExecuteOnMessageCommand(rawCommand, e.Channel, e.Author);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        this._client.SendMessageToChannel("You don't have permission to execute this command. Contact administrator for additional information", e.Channel);
                        Console.WriteLine($"{e.Author.Username} attempted to execute command with low access level");
                    }
                }
            };
            this._client.PrivateMessageReceived += (sender, e) =>
            {
                if (e.Author.ID != this._config.OwnerID)
                    Console.WriteLine($"Private message received from {e.Author.Username}: \"{e.Message}\"");
                if (e.Message.Length > 0 && e.Message[0] == this._config.CommandPrefix)
                {
                    string rawCommand = e.Message.Substring(1);
                    if (e.Author.ID == this._config.OwnerID)
                    {
                        this._client.AcceptInvite(rawCommand.Substring(rawCommand.LastIndexOf('/') + 1));
                    }
                }
                if (e.Message.StartsWith("?authenticate") && owner == null)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write("Kerrang!\nIn order to become an owner type in bot's password: ");
                    if (Console.ReadLine() == this._config.BotPass)
                    {
                        CommandsManager.AddPermission(e.Author, PermissionType.Owner);
                        this._config.OwnerID = e.Author.ID;
                        owner = this._client.GetServersList().Find(x => x.GetMemberByKey(e.Author.ID) != null).GetMemberByKey(e.Author.ID);
                        Console.WriteLine($"I'm welcoming you, my current owner {e.Author.Username}");
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    owner.SlideIntoDMs("Welcome long lost father!");
                    File.WriteAllText(Path.GetFullPath("settings.json"), JsonConvert.SerializeObject(this._config));
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
                       help += String.Format("{0,-15} {1,-100}\n", this._config.CommandPrefix + command.CommandName, "-" + command.Description + " Level: " + command.MinimumPermission);
                   }
                   cmdArgs.Channel.SendMessage(help);
               }));
            CommandsManager.AddCommand(new CommandStub("join", "Join a specified server. Ownder only.", "", PermissionType.Owner, 1, cmdArgs =>
            {
                string substring = cmdArgs.Args[0].Substring(cmdArgs.Args[0].LastIndexOf('/') + 1);
                this._client.AcceptInvite(substring);
                Console.WriteLine($"Joined new server.");
                cmdArgs.Author.SlideIntoDMs("Joined server you requested!");
            }));
            CommandsManager.AddCommand(new CommandStub("wakeup", $"Mentions user multiple times. Example: {this._config.CommandPrefix}wakeup Username n-times", "specify victim", PermissionType.User, 2, cmdArgs =>
               {
                   string substring = cmdArgs.Args[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
                   var victim = this._client.GetMemberFromChannel(cmdArgs.Channel, cmdArgs.Args[0], false);
                   int times = 0;
                   if (!int.TryParse(cmdArgs.Args[1], out times))
                   {
                       cmdArgs.Channel.SendMessage("Numbers, not random letters, dummy!");
                   }
                   for (int i = 0; i < times; i++)
                   {
                       cmdArgs.Channel.SendMessage($"@{victim.Username}, wake up!");
                   }
               }));
            CommandsManager.AddCommand(new CommandStub("deletemsg", $"Delete last n messages. Example: {this._config.CommandPrefix}deletemsg n", "specify amount of messages", PermissionType.Mod, 1, cmdArgs =>
                 {
                     int amount = 0;
                     int.TryParse(cmdArgs.Args[0], out amount);
                     this._client.DeleteMultipleMessagesInChannel(cmdArgs.Channel, amount + 1);
                 }));
            CommandsManager.AddCommand(new CommandStub("ban", $"Bans user for specified amount of days. Example: {this._config.CommandPrefix}ban user days", "", PermissionType.Mod, 2, cmdArgs =>
                 {
                     int days=1;
                     int.TryParse(cmdArgs.Args[1], out days);
                     _client.BanMember(_client.GetMemberFromChannel(cmdArgs.Channel, cmdArgs.Args[0]));
                 }));
            CommandsManager.AddCommand(new CommandStub("kick", $"Kicks user. Example: {this._config.CommandPrefix}kick user", "", PermissionType.Mod, 1, cmdArgs =>
             {
                 this._client.KickMember(this._client.GetMemberFromChannel(cmdArgs.Channel, cmdArgs.Args[0]));
             }));
            CommandsManager.AddCommand(new CommandStub("permission", $"Grants user specified permission: Admin, Mod or User. Example: {this._config.CommandPrefix}permission user level", "", PermissionType.Owner, 2, cmdArgs =>
            {
                var member = this._client.GetMemberFromChannel(cmdArgs.Channel, cmdArgs.Args[0], false);
                PermissionType type = PermissionType.User;
                switch (cmdArgs.Args[1].ToLower())
                {
                    case "admin":
                        type = PermissionType.Admin;
                        break;
                    case "mod":
                        type = PermissionType.Mod;
                        break;
                    case "user":
                        type = PermissionType.User;
                        break;
                    case "none":
                        type = PermissionType.None;
                        break;
                    default:
                        break;
                }
                cmdArgs.Channel.SendMessage($"User {member.Username} become {type.ToString()}");
                Console.WriteLine($"New {type.ToString()} - {member.Username}({member.ID})");
                CommandsManager.AddPermission(member, type);
            }));
            CommandsManager.AddCommand(new CommandStub("q", $"Posts random quote. Example: {this._config.CommandPrefix}q", "", PermissionType.None, 1, cmdArgs =>
                {
                    cmdArgs.Channel.SendMessage("\"" + ConfuQuote[CommandsManager.rng.Next(0,ConfuQuote.Length)] + "\"" + " - Конфуций");
                }));
            CommandsManager.AddCommand(new CommandStub("ball", $"If you seek for advice, just ask. Example: {this._config.CommandPrefix}ball", "", PermissionType.None, cmdArgs =>
                {
                    cmdArgs.Channel.SendMessage(Ball8[CommandsManager.rng.Next(0, Ball8.Length)]);
                }));
            CommandsManager.AddCommand(new CommandStub("joinvoice", "Join voice channel", "", PermissionType.Owner, 1, cmdArgs =>
            {
                DiscordChannel channelToJoin = cmdArgs.Channel.Parent.Channels.Find(x => x.Name.ToLower() == cmdArgs.Args[0].ToLower() && x.Type == ChannelType.Voice);
                if (channelToJoin != null)
                {
                    DiscordVoiceConfig config = new DiscordVoiceConfig
                    {
                        FrameLengthMs = 60,
                        Channels = 1,
                        OpusMode = DiscordSharp.Voice.OpusApplication.MusicOrMixed,
                        SendOnly = true
                    };
                    this.CommandsManager.Client.ConnectToVoiceChannel(channelToJoin, config);
                    this.ap = new AudioPlayer(this.CommandsManager.Client, config, channelToJoin);
                }
            }));
            CommandsManager.AddCommand(new CommandStub("disconnect", "Disconnects from voice channel.", "", PermissionType.Owner, 0, cmdArgs =>
                 {
                     this._client.DisconnectFromVoice();
                 }));
            CommandsManager.AddCommand(new CommandStub("player", "Granting access to audio player. Currently avaliable commands: play", "", PermissionType.User, 1, cmdArgs =>
                 {
                     switch (cmdArgs.Args[0])
                     {
                         case "play":
                             this.ap.Play();
                             break;
                         case "stop":
                             if(this._client.GetVoiceClient() != null)
                             {
                                 this._client.GetVoiceClient().ClearVoiceQueue();
                             }
                             break;
                         default:
                             break;
                     }
                 }));
            CommandsManager.AddCommand(new CommandStub("roles", "Shows permission levels for users.", "", PermissionType.Owner, 0, cmdArgs =>
                 {
                     string msg = $"Roles for \"{cmdArgs.Channel.Parent.Name}\"\n";
                     foreach (var role in cmdArgs.Channel.Parent.Roles)
                     {
                         msg += $"{role.Position} - {role.ID} - {role.Permissions.GetRawPermissions()}\n";
                     }
                     owner.SlideIntoDMs(msg);
                 }));
            CommandsManager.AddCommand(new CommandStub("shutdown", "Shutdown bot. Owner only.", "", PermissionType.Owner, 0, cmdArgs =>
               {
                   Exit();
               }));

        }

        public void Login()
        {
            if (this._client.SendLoginRequest() != null) _client.Connect();
        }

        public void Exit()
        {
            _client.Logout();
            if(CommandsManager.UserRoles != null && CommandsManager.UserRoles.Count > 0)
            {
                File.WriteAllText("permissions.json", JsonConvert.SerializeObject(CommandsManager.UserRoles));
            }
            _client.Dispose();
            Environment.Exit(0);
        }
    }
}
