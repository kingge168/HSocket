using System.Collections.Concurrent;
using Spider.Data;
using System.Collections.Generic;
using Spider.Util;
using System;

namespace Spider.Network
{
    public interface IActivtor
    {
         CommandBase CreateInstance(Type commandType,params object[] args);
    }

    class DefalutActivtor : IActivtor
    {
        public CommandBase CreateInstance(Type commandType,params object[] args)
        {
            return Activator.CreateInstance(commandType, args) as CommandBase;
        }
    }

     class  CommandFactory
    {
        private static ConcurrentDictionary<string, Type> Commands { get; set; }

        private static IActivtor Activtor { get; set; }

        static CommandFactory()
        {
            Activtor = new DefalutActivtor();
            Commands = new ConcurrentDictionary<string, Type>();
            IEnumerable<Type> types = TypeFinder.FindFromAssemblies<CommandBase>(AppDomain.CurrentDomain.GetAssemblies());
            foreach (var type in types)
            {
                //ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                if (type.Name.EndsWith("Command"))
                {
                    Commands.TryAdd(type.Name.Substring(0, type.Name.Length-7), type);
                }
                #if DEBUG
                else
                {
                    Logger.ErrorFormat("{0} is a invalid commad.", type.FullName);
                }
                #endif
            }
        }

        public static byte[] Process(SmartObject message)
        {
            if (message.IsDefined("Command"))
            {
                dynamic d = message;
                Type commandType;
                if (Commands.TryGetValue(d.Command.ToString(), out commandType))
                {
                    var command = Activtor.CreateInstance(commandType);
                    return command.Process(message);
                }
            }
            return null;
        }
    }
}
