using DiscordSharp;
using DiscordSharp.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KWRBot
{
    class Program
    {
        //readonly string Email = "derpina2013@yandex.ru";
        //readonly string Password = "derpina";
        //readonly string BotToken = "MTkyNjI4NzY5ODMwMjA3NDg5.CkLnBQ.ZvmlN4E2koYhcOOKX5z2wYOrmcE";

        static void Main(string[] args)
        {
            KWRBot bot = new KWRBot();
            bot.Login();
            Console.ReadKey();
            bot.Exit();
        }
    }
}
