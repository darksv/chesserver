using System;

namespace ChessServer
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