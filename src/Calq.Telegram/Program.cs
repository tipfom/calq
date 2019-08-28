using Calq.Core;
using System;
using System.Text;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Calq.Telegram
{
    class Program
    {
        const string HTTP_API_KEY = "814183760:AAHLq3QDHlFGYg5lUuH5Fs4Fl7eNq5Aj3hM";
        static TelegramBotClient bot;

        static void Main(string[] args)
        {
            bot = new TelegramBotClient(HTTP_API_KEY);
            bot.OnMessage += BotClient_OnMessage;
            bot.StartReceiving(Array.Empty<UpdateType>());
            Console.ReadLine();
        }

        private static void BotClient_OnMessage(object? sender, global::Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Text == null) return;
            if (e.Message.Text.StartsWith("/q"))
            {
                try
                {
                    Term t = Term.Parse(e.Message.Text.Substring(2));

                    string expLat = t.ToLaTeX();

                    t = t.MergeBranches();
                    t = t.Evaluate();


                    int i = 0;
                    int lastLength = int.MaxValue;
                    while (t.ToInfix().Length < lastLength)
                    {
                        lastLength = t.ToInfix().Length;
                        t = t.MergeBranches();
                        t = t.Reduce();
                        i++;
                    }

                    string msg = "Ergebnis<a href=\"https://qalq.timpokart.de/print/" + Convert.ToBase64String(Encoding.UTF8.GetBytes(t.ToLaTeX())) + ".png\">&#8205;</a>";
                    bot.SendTextMessageAsync(e.Message.Chat.Id, msg, ParseMode.Html);
                }
                catch
                {
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "Verarbeitungsfehler");
                }
            }
        }
    }
}
