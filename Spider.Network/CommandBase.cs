using Spider.Data;

namespace Spider.Network
{
    public abstract class CommandBase
    {
        public CommandBase()
        {

        }

        public abstract string Name
        {
            get;
        }

        public abstract byte[] Process(SmartObject data);
    }
}
