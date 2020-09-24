namespace Enigmatry.Blueprint.BuildingBlocks.Email
{
    public interface IEmailClient
    {
        void Send(EmailMessage email);
    }
}
