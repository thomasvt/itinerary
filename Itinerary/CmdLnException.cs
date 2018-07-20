using System;

namespace Itinerary
{
    public class CmdLnException
    : Exception
    {
        public CmdLnException(string message)
        : base(message)
        {
        }
    }
}
