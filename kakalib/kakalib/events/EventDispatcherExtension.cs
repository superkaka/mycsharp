using System;
using System.Collections.Generic;
using System.Text;

namespace KLib.events
{
    public delegate void ExtensionEventHandler(Event evt);
    static public class EventDispatcherExtension
    {

        static private Dictionary<Object, Dictionary<string, List<ExtensionEventHandler>>> dic_eventDispatcher = new Dictionary<object, Dictionary<string, List<ExtensionEventHandler>>>();

        static public void DispatchEvent(this Object target, Event evt)
        {
            var list = getHandlerList(target, evt.Type);
            if (null == list)
                return;

            evt.Sender = target;
            for (int i = 0; i < list.Count; i++)
            {
                list[i](evt);
            }
        }

        static public void AddEventListener(this Object target, string type, ExtensionEventHandler handler)
        {
            RemoveEventListener(target, type, handler);
            var list = getHandlerList(target, type, true);
            list.Add(handler);
        }

        static public void RemoveEventListener(this Object target, string type, ExtensionEventHandler handler)
        {
            var list = getHandlerList(target, type);
            if (null == list)
                return;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == handler)
                {
                    list.Remove(handler);
                    break;
                }
            }
        }

        static public void RemoveAllEventListener(this Object target)
        {
            if (!dic_eventDispatcher.ContainsKey(target))
                return;

            var record_obj = dic_eventDispatcher[target];
            record_obj.Clear();
            dic_eventDispatcher.Remove(target);
        }

        static public List<ExtensionEventHandler> getHandlerList(Object target, string type, bool createIfNull = false)
        {
            List<ExtensionEventHandler> list = null;
            Dictionary<string, List<ExtensionEventHandler>> dic = null;

            if (dic_eventDispatcher.ContainsKey(target))
                dic = dic_eventDispatcher[target];
            else if (createIfNull)
                dic = dic_eventDispatcher[target] = new Dictionary<string, List<ExtensionEventHandler>>();

            if (dic != null)
            {
                if (dic.ContainsKey(type))
                    list = dic[type];
                else if (createIfNull)
                    list = dic[type] = new List<ExtensionEventHandler>();
            }

            return list;
        }

    }
}
