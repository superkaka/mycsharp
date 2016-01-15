using System;
using System.Collections.Generic;
using System.Text;

namespace KLib.events
{
    public class Event
    {

        private string type;
        public string Type
        {
            get { return type; }
        }

        private Object data;
        public Object Data
        {
            get { return data; }
        }

        private Object sender;
        public Object Sender
        {
            set
            {
                if (sender != null)
                    throw new Exception("The property \"Sender\" can not change");
                sender = value;
            }
            get { return sender; }
        }

        public Event(string type, Object data = null)
        {
            this.type = type;
            this.data = data;
        }

    }
}
