using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.ServiceBus
{
    public interface IMessageBus
    {
        Task publishMessage(object message, string topic_queue_name);
    }
}
