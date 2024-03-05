using Lagrange.Core.Common.Interface.Api;

namespace Lagrange.Core.Test.Tests;

// ReSharper disable once InconsistentNaming

public class NTLoginTest
{
    public async Task LoginByPassword(BotContext bot)
    {
        bot.Invoker.OnBotCaptchaEvent += (_, @event) =>
        {
            Console.WriteLine(@event.ToString());
            var captcha = Console.ReadLine();
            var randStr = Console.ReadLine();
            if (captcha != null && randStr != null) bot.SubmitCaptcha(captcha, randStr);
        };
        
        bot.Invoker.OnGroupInvitationReceived += (_, @event) =>
        {
            Console.WriteLine(@event.ToString());
        };

        await bot.LoginByPassword();

        var friendChain = MessageBuilder.Group(411240674)
                .Text("This is the friend message sent by Lagrange.Core")
                .Mention(1925648680);
        await bot.SendMessage(friendChain.Build());

        await Task.Delay(1000);
    }
}