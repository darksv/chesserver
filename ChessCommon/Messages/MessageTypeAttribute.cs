using System;

namespace Chess.Common.Messages
{
    public class MessageTypeAttribute : Attribute
    {
        public string Type { get; }

        public MessageTypeAttribute(string type)
        {
            Type = type;
        }
    }
}