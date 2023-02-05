using System;
using Glasswall.Kernel.Messaging;

namespace Messaging
{
    [Serializable]
    public class TestMessage : Message
    {
        public TestMessage(Guid id) : base(id)
        {
        }

        public string Name { get; set; }
    }
}
