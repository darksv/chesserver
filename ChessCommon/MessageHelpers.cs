using Chess.Common.Messages;
using Newtonsoft.Json;

namespace Chess.Common
{
    public static class MessageHelpers
    {
        public static string ExtractType(string message)
        {
            try
            {
                return JsonConvert.DeserializeObject<Message>(message)?.Type;
            }
            catch
            {
                return null;
            }
        }

        public static string SerializeMessage<TMessage>(TMessage message)
            where TMessage : Message
        {
            return JsonConvert.SerializeObject(message, Formatting.None) + "\n";
        }
    }
}
