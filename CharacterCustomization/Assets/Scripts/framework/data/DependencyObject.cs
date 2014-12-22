using System;
using System.Collections;
using System.Collections.Generic;
using hs.framework.utils;

namespace hs.framework.data
{
    public delegate void EventCallback(DependencyPropertyChangeEvent e);

    public class DependencyObject
    {
        protected Dictionary<string, Delegate> listeners = new Dictionary<string, Delegate>();
        protected Hashtable prop = new Hashtable();
        protected int step = 0;
        public void Clear(){
			prop.Clear();
        }
		public DependencyObject fromObject(object o)
        {
            ObjectUtils.copyValues(o, this, null);
			return this;
        }

        /**
			 *取得数据
			 * @param name
			 * @param value
			 * @return 
			 * 
			 */

        protected object GET(string key, object v = null)
        {
            if (prop.ContainsKey(key))
            {
                return prop[key];
            }
            return (v);
        }

        /**
			 *设置数据 
			 * @param key
			 * @param value
			 * @return 
			 * 
			*/

        protected Boolean SET(string key, object v)
        {
            object oldValue;
            if (prop.ContainsKey(key))
            {
                oldValue = prop[key];
                prop[key] = v;
            }
            else
            {
                oldValue = null;
                prop.Add(key, v);
            }

            if (v != oldValue)
            {
                var e = new DependencyPropertyChangeEvent(key, oldValue, v);
                e.currentTarget = this;
                var values = new Delegate[listeners.Count];
                listeners.Values.CopyTo(values, 0);
                for (int i = 0; i < values.Length; i++)
                {
                    var callback = values[i] as EventCallback;
                    callback(e);
                }
                //Messenger<DependencyPropertyChangeEvent>.Broadcast(DependencyPropertyChangeEvent.CHANGE,e,MessengerMode.DONT_REQUIRE_LISTENER);
                /*	foreach(KeyValuePair<string,Delegate> de in listeners){
					 
						EventCallback callback=de.Value as EventCallback;
						callback(e);
					}
					*/
                return true;
            }

            return false;
        }

        public override string ToString()
        {
			return GetType()+ ":" ;  // Json.jsonEncode(prop);
        }
		public Hashtable ToHashTable(){
			return prop;
		}
        public void addEventListener(string EventName, EventCallback callback)
        {
            if (!listeners.ContainsValue(callback))
            {
                listeners.Add(EventName + step, callback);
                step++;
            }
        }

        public void removeEventListener(string EventName, EventCallback callback)
        {
            foreach (var de in listeners)
            {
                if (de.Value == callback)
                {
                    listeners.Remove(de.Key);
                    break;
                }
            }
        }
    }
}