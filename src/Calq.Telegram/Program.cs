using Calq.Core;
using System;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Calq.Telegram
{
    class Program
    {
        static TelegramBotClient bot;
        static void Main(string[] args)
        {
            bot = new TelegramBotClient("846107457:AAG1xHj6JAcWtTVkT-Qai23Xxws_PCpNHI0");
            var me = bot.GetMeAsync().Result;
            Console.WriteLine(
              $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );
            bot.OnMessage += BotClient_OnMessage;
            bot.StartReceiving(Array.Empty<UpdateType>());
            Console.ReadLine();
        }

        private static void BotClient_OnMessage(object? sender, global::Telegram.Bot.Args.MessageEventArgs e)
        {
            Term t = Term.Parse(e.Message.Text);
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

            string ret = t.ToLaTeX();
            bot.SendTextMessageAsync(e.Message.Chat.Id, ret);
        }
    }
}
