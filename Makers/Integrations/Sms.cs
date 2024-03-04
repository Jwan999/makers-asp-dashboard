namespace Makers.Integrations;

public class Sms
{
    public static void sendOtp(string phoneNumber, string value)
    {
        //TODO: Integrate with messaging service :)
        //MessageResource.Create(
        //    body: $"Your OTP code is {value}",
        //    from: new PhoneNumber("WhatsApp:+xxxxxx
        //    to: new PhoneNumber($"WhatsApp:+{phoneNumber}"));
    }

    public static void SendCredential(string phoneNumber, string password)
    {
        //MessageResource.Create(
        //    body: $"Your new password is [{password}], please do not share it.",
        //    from: new PhoneNumber("WhatsApp:+xxxx
        //    to: new PhoneNumber($"WhatsApp:+{phoneNumber}"));
    }
}