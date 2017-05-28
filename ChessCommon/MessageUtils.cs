using System.Linq;
using Chess.Common.Messages;
using Newtonsoft.Json;

namespace Chess.Common
{
    public static class MessageUtils
    {
        public static string MakeMessage<T>(T value)
            where T : Message
        {
            if (string.IsNullOrEmpty(value.Type))
            {
                var messageTypeAttribute = typeof(T)
                    .GetCustomAttributes(false)
                    .OfType<MessageTypeAttribute>()
                    .FirstOrDefault();
                value.Type = messageTypeAttribute?.Type ?? string.Empty;
            }

            var json = JsonConvert.SerializeObject(value, Formatting.None);
            return $"{json}\n";
        }
    }
}
