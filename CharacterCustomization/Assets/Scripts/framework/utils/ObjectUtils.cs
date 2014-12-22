using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using hs.framework.data;
using hs.framework.data.binding;

namespace hs.framework.utils
{
    public static class ObjectUtils
    {
         
        /**
		 *将ArrayList中的数据以type的实例替换 
		 * @param ArrayList 数据列表
		 * @param type 类型
		 * @param map 对应copy字段列表
		 * 
		 */

        public static ArrayList replaceArrayItems(ArrayList array, Type type, Hashtable map)
        {
            object source;
            object target;


            for (int i = 0; i < array.Count; i++)
            {
                source = array[i];

                target = Activator.CreateInstance(type);


                copyValues(source, target, map);


                array[i] = target;
            }
            ;
            return array;
        }

        public static BetterList<T> replaceArrayItems<T>(ArrayList array, Hashtable map)
        {
            object source;
            object target;


            var newlist = new BetterList<T>();


            for (int i = 0; i < array.Count; i++)
            {
                source = array[i];

                target = Activator.CreateInstance(typeof (T));


                copyValues(source, target, map);


                newlist.Add((T) target);
            }
            ;
            return newlist;
        }


        public static Hashtable getMapList(object source)
        {
            if (source.GetType() == typeof (Hashtable))
            {
                Hashtable s=source as Hashtable;
				Hashtable map=new Hashtable();
				foreach(string k in 	s.Keys){
						
					map.Add(k,k);
				}
                return map;
            }


            var result = new Hashtable();

            Type type = source.GetType();
            /*
			MemberInfo[] vars=type.GetMembers();
			foreach(MemberInfo varinfo in vars){
				if(result.ContainsKey(varinfo.Name))continue;
				result.Add(varinfo.Name,varinfo.Name);
			}*/
            FieldInfo[] files = type.GetFields();
            foreach (FieldInfo fie in files)
            {
                if (!result.ContainsKey(fie.Name)) result.Add(fie.Name, fie.Name);
            }
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo propInfo in props)
            {
                if (result.ContainsKey(propInfo.Name)) continue;
                result.Add(propInfo.Name, propInfo.Name);
            }


            return result;
        }

        /**
		 *复制结构数据
		 * @param source 来源数据
		 * @param target 目标数据
		 * @param map 复制对应名如 source的key1对应target的key2
		 * @param reversal 是否反向复制
		 * 
		 */

        public static void copyValues(object source, object target, Hashtable map = null, bool reversal = false)
        {
            if (source == null || target == null)
            {
                return;
            }
            ;
            if (map == null)
            {
                map = getMapList(target);
            }
            ;
            if (map == null)
            {
                return;
            }
            ;


            foreach (DictionaryEntry de  in map)
            {
                copyValue(source, target, de.Key as string, de.Value as string, reversal);
            }
            ;
        }

        /**
		 *复制指定值 
		 * @param source
		 * @param target
		 * @param sourceMember
		 * @param targetMember
		 * @param reversal
		 * 
		 */

        public static void copyValue(object source, object target, string sourceMember, string targetMember,
                                     bool reversal = false)
        {
            if (target is MonoBehaviour)
            {
                var mn = target as MonoBehaviour;
                if (mn == null || mn.gameObject == null || !mn.enabled)
                {
                    //waring!!!!!
                    Binding.unbind(source as DependencyObject, target);
                    return;
                }
            }


            try
            {
                if (!reversal)
                {
//					MonoBehaviour.print(string.Format ("CopyValue source:{0},target:{1},sourcemember:{2},targetmember:{3}",source,target,sourceMember,targetMember));
                    PropertyInfo val = source.GetType().GetProperty(sourceMember);
                    FieldInfo val2 = source.GetType().GetField(sourceMember);
                    FieldInfo key = target.GetType().GetField(targetMember);
                    PropertyInfo key2 = target.GetType().GetProperty(targetMember);


                   if (source is IDictionary)
                    {
                        var ids = source as IDictionary;
                        //no member value
                        if (!ids.Contains(sourceMember) || ids[sourceMember] == null) return;
                        string v = ids[sourceMember].ToString();
						//Debug.Log(sourceMember+"="+v);
                        if (key != null)
                        {
                            if (key.FieldType.Name == "Int64")
                            {
                                key.SetValue(target, long.Parse(v));
                            }
                            else if (key.FieldType.Name == "Boolean")
                            {
                                key.SetValue(target, bool.Parse(v));
                            }
                            else if (key.FieldType.Name == "Int32")
                            {
                                key.SetValue(target, int.Parse(v));
                            }
                            else if (key.FieldType.Name == "Int16")
                            {
                                key.SetValue(target, int.Parse(v));
                            }
                            else if (key.FieldType.Name == "Single")
                            {
                                key.SetValue(target, float.Parse(v));
                            }
                            else if (key.FieldType.Name == "Double")
                            {
                                key.SetValue(target, double.Parse(v));
                            }
                            else
                            {
                                key.SetValue(target, v);
                            }
                        }
                        else if (key2 != null)
                        {
                            if (key2.PropertyType.Name == "Int64")
                            {
                                key2.SetValue(target, long.Parse(v), null);
                            }
                            else if (key2.PropertyType.Name == "Boolean")
                            {
                                key2.SetValue(target, bool.Parse(v), null);
                            }
                            else if (key2.PropertyType.Name == "Int32")
                            {
                                key2.SetValue(target, int.Parse(v), null);
                            }
                            else if (key2.PropertyType.Name == "Int16")
                            {
                                key2.SetValue(target, int.Parse(v), null);
                            }
                            else if (key2.PropertyType.Name == "Single")
                            {
                                key2.SetValue(target, float.Parse(v), null);
                            }
                            else if (key2.PropertyType.Name == "Double")
                            {
                                key2.SetValue(target, double.Parse(v), null);
                            }
                            else
                            {
                                key2.SetValue(target, v, null);
                            }
                        }
                    }
                    else
                    {
                        if (val != null)
                        {
                            if (key != null)
                            {
                                key.SetValue(target, val.GetValue(source, null));
                            }
                            else
                            {
                                key2.SetValue(target, val.GetValue(source, null), null);
                            }
                        }
                        else
                        {
                            if (key != null)
                            {
                                key.SetValue(target, val2.GetValue(source));
                            }
                            else
                            {
                                key2.SetValue(target, val2.GetValue(source), null);
                            }
                        }
                    }
                }
                else
                {
                    PropertyInfo val = target.GetType().GetProperty(targetMember);
                    FieldInfo val2 = target.GetType().GetField(targetMember);
                    FieldInfo key = source.GetType().GetField(sourceMember);
                    PropertyInfo key2 = source.GetType().GetProperty(sourceMember);
                    if (val != null)
                    {
                        if (key != null)
                        {
                            key.SetValue(target, val.GetValue(source, null));
                        }
                        else
                            key2.SetValue(target, val.GetValue(source, null), null);
                    }
                    else
                    {
                        if (key != null)
                        {
                            key.SetValue(target, val2.GetValue(source));
                        }
                        else
                            key2.SetValue(target, val2.GetValue(source), null);
                    }
                }
            }
            catch (Exception e){

				Debug.LogWarning("Copy Error___sourceMember:"+sourceMember+",targetMember:"+targetMember );
                Debug.LogException(e);
            }
        }
    }
}