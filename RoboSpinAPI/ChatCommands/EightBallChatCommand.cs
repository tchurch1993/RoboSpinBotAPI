using RoboSpinAPI.Interfaces;

namespace RoboSpinAPI.ChatCommands;

public class EightBallChatCommand : IChatCommand
{
    public void Execute(IEnumerable<string> args, string channel, string user)
    {
        Random random = new Random();
        string[] answers = new[]
        {
            "It is certain",
            "It is decidedly so",
            "Without a doubt",
            "Yes definitely",
            "You may rely on it",
            "As I see it, yes",
            "Most likely",
            "Outlook good",
            "Yes",
            "Signs point to yes",
            "Reply hazy try again",
            "Ask again later",
            "Better not tell you now",
            "Cannot predict now",
            "Concentrate and ask again",
            "Don't count on it",
            "My reply is no",
            "My sources say no",
            "Outlook not so good",
            "Very doubtful"
        };

        string answer = answers[random.Next(answers.Length)];
        string message = $":8ball: {user}: {answer}";
        //TwitchBot.SendMessage(channel, message);
    }

    public string CommandName => "8ball";
    public string Description => "Ask the magic 8 ball a question";
}