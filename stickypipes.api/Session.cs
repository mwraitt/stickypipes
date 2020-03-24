using System;

namespace stickypipes.api
{
    public class Session
    {
        public string User { get; set; }

        public Guid Id { get; set; }

        public int Value { get; set; } = 0;
    }
}
