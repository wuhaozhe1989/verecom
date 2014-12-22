using System.Collections;
using UnityEngine;
using hs.framework.utils;

namespace hs.framework.data.binding
{
    public enum BindingMode
    {
        /**
		 *一次 
		 */
        ONE_TIME = 0,
        /**
		 *单向 
		 */
        ONE_WAY = 1,
        /**
		 *双向 
		 */
        TWO_WAY = 2
    }


    public class BindingRelationship
    {
        public object[] args;
        public Hashtable map;
        public BindingMode mode;
        public DependencyObject source;
        public object target;

        public override string ToString()
        {
            return "[BindingRelationship:from:" + source.GetType() + ",to:" + target.GetType()
				+ ",mode:" + mode.ToString() + "]";
                  // + Json.jsonEncode(map);
        }
    }

    public interface IBindingMethod
    {
        bool bind(DependencyObject orgData, object objData, Hashtable objMap, BindingMode m, params object[] args);
        void unbind(DependencyObject sourceData, object target);
        void unbindFromSource(DependencyObject sourceData);
        void unbindFromTarget(object target);
        void clear();
    }

    public class DataPropertyBinding : IBindingMethod
    {
        protected BetterList<BindingRelationship> list = new BetterList<BindingRelationship>();

        public bool bind(DependencyObject source, object target, Hashtable map, BindingMode mode, params object[] args)
        {
            if (map == null)
            {
                map = ObjectUtils.getMapList(source);
            }
            if ((((((source == null)) || ((target == null)))) || ((map == null))))
            {
                return (false);
            }
            ;

            var relationship = new BindingRelationship();

            relationship.source = source;

            relationship.target = target;

            relationship.map = map;

            relationship.mode = mode;

            relationship.args = args;

            bool result = add(relationship);

            ObjectUtils.copyValues(source, target, map);

            return (result);
        }

        public void unbind(DependencyObject source, object target)
        {
            BindingRelationship relationship;
            int i = (list.size - 1);
			remove(source, target);
            while (i >= 0)
            {
                relationship = list[i];

                if (relationship.source == source && relationship.target == target)
                {
                    list.RemoveAt(i);
                }

                i--;
            }

            
        }

        public void unbindFromSource(DependencyObject source)
        {
            BindingRelationship relationship;
            int i = (list.size - 1);
			remove(source, null);
            while (i >= 0)
            {
                relationship = list[i];

                if (source == relationship.source)
                {
                    list.RemoveAt(i);
                }

                i--;
            }

            
        }

        public void unbindFromTarget(object target)
        {
            BindingRelationship relationship;
            int i = (list.size - 1);
			
			remove(null, target);
            while (i >= 0)
            {
                relationship = list[i];

                if (target == relationship.target)
                {
                    list.RemoveAt(i);
                }
                ;

                i--;
            }

        }

        public void clear()
        {
            var removeList = new ArrayList();

            for (int i = 0; i < list.size; i++)
            {
                BindingRelationship br = list[i];
                if (!removeList.Contains(br.source))
                {
                    removeList.Add(br.source);
                }
                ;

                if (br.mode == BindingMode.TWO_WAY)
                {
                    if (!removeList.Contains(br.target))
                    {
                        removeList.Add(br.target);
                    }
                    ;
                }
                ;
            }
            ;

            foreach (DependencyObject o in removeList)
            {
                o.removeEventListener(DependencyPropertyChangeEvent.CHANGE, onobjectPropertyChange);
                //Messenger<DependencyPropertyChangeEvent>.RemoveListener(DependencyPropertyChangeEvent.CHANGE, this.onobjectPropertyChange);
            }
            ;
            list.Clear();
        }

        protected bool contain(BindingRelationship relationship)
        {
            for (int i = 0; i < list.size; i++)
            {
                BindingRelationship br = list[i];
                if (br.source == relationship.source && br.target == relationship.target)
                {
                    return (true);
                }
                ;
            }
            ;

            return (false);
        }

        protected bool add(BindingRelationship relationship)
        {
            if (relationship.mode == BindingMode.ONE_TIME)
            {
                return (false);
            }
            ;

            if (contain(relationship))
            {
                return (false);
            }
            ;

            if ((((relationship.mode == BindingMode.ONE_WAY)) || ((relationship.mode == BindingMode.TWO_WAY))))
            {
                //Messenger<DependencyPropertyChangeEvent>.AddListener(DependencyPropertyChangeEvent.CHANGE,onobjectPropertyChange);
                relationship.source.addEventListener(DependencyPropertyChangeEvent.CHANGE, onobjectPropertyChange);
            }
            ;

            if (relationship.mode == BindingMode.TWO_WAY)
            {
                //Messenger<DependencyPropertyChangeEvent>.AddListener(DependencyPropertyChangeEvent.CHANGE,onobjectPropertyChange);
                (relationship.target as DependencyObject).addEventListener(DependencyPropertyChangeEvent.CHANGE,
                                                                           onobjectPropertyChange);
            }
            ;

            list.Add(relationship);

            return (true);
        }

        protected void onobjectPropertyChange(DependencyPropertyChangeEvent e)
        {
            //	if(e.propertyName=="onlineNum")
            ////MonoBehaviour.print(e.ToString());
            for (int i = 0; i < list.size; i++)
            {
                BindingRelationship br = list[i];

                if (br.mode == BindingMode.ONE_WAY)
                {
                    if (br.source != e.currentTarget)
                    {
                        continue;
                    }
                }
                ;

                if (br.mode == BindingMode.TWO_WAY)
                {
                    if (br.source != e.currentTarget)
                    {
                        continue;
                    }
                }
                ;
                //   if(e.propertyName=="onlineNum")MonoBehaviour.print(br);
                //if (br.source == e.currentTarget){

                //br.target[e.propertyName]=br.source[br.map[e.propertyName]];
                if (br.map.ContainsKey(e.propertyName))
                {
                    if (br.source != null && br.target != null)
                        ObjectUtils.copyValue(br.source, br.target, e.propertyName, br.map[e.propertyName] as string);
                    else Binding.unbind(br.source, br.target);
                }
                /*} else {
                    
                    if (br.target == e.currentTarget){
//                    	br.source[br.map[e.propertyName]]=br.target[e.propertyName];
						if(br.map.ContainsValue(e.propertyName))
						ObjectUtils.copyValue( br.target as DependencyObject,br.source,e.propertyName,map[e.propertyName] as string,true); 
                    };
                };*/
            }
            ;
        }

        protected void remove(DependencyObject source, object target)
        {
            bool needRemoveSource = true;

            bool needRemoveTarget = true;

            for (int i = 0; i < list.size; i++)
            {
                BindingRelationship br = list[i];
                if (br.source == source)
                {
                    needRemoveSource = false;
                }
                ;

                if ((((br.target == target)) && ((br.mode == BindingMode.TWO_WAY))))
                {
                    needRemoveTarget = false;
                }
                ;

                if ((((needRemoveSource == false)) && ((needRemoveTarget == false)))) break;
            }
            ;

            if (needRemoveSource)
            {
                if (source != null)
                    source.removeEventListener(DependencyPropertyChangeEvent.CHANGE, onobjectPropertyChange);
                //Messenger<DependencyPropertyChangeEvent>.RemoveListener(DependencyPropertyChangeEvent.CHANGE, this.onobjectPropertyChange);
            }
            ;

            if (((needRemoveTarget) && ((target is DependencyObject))))
            {
                (target as DependencyObject).removeEventListener(DependencyPropertyChangeEvent.CHANGE,
                                                                 onobjectPropertyChange);
                //Messenger<DependencyPropertyChangeEvent>.RemoveListener(DependencyPropertyChangeEvent.CHANGE, this.onobjectPropertyChange);
            }
            ;
        }
    }

    /**
	 *绑定类 范例
	 * <br/>
	 * <br/>	Binding.bind(someDepencyObject, this, {
       <br/>         	rolename:"rolename",
       <br/>         	headImageUrl:"headImageUrl",
       <br/>         	sex:"sex",
       <br/>         	coins:"coins",vip:"vip"
       <br/>     }, BindingMode.ONE_WAY); 
	 * @author Gao.D.X
	 * 
	 */

    public static class Binding
    {
        //    private static Binding _instance;

        private static ArrayList _methods;


        /*
        public static bool bind(DependencyObject source, object target, Hashtable map, uint mode, params object[] args){
      
            args=args.CopyTo(new object[] {source,target,map,mode},4);
           
                 
				
             return _instance.GetType().GetMethod("bind").Invoke(method,args);
                    
			
        }

        public static void unbind(DependencyObject source, object target){
            
            instance.unbind(source, target);
            
        }

        public static void unbindFromSource(DependencyObject source){
            instance.unbindFromSource(source);
            
        }

        public static void unbindFromTarget(Object target){
            instance.unbindFromTarget(target);
            
        }

        public static void clear(){
            
            instance.clear();
            
        }*/

        private static bool inited;

        public static ArrayList methods()
        {
            return (_methods);
        }

        private static void init()
        {
            if (inited) return;
            inited = true;
            _methods = new ArrayList();

            _methods.Add(new DataPropertyBinding());
        }

        public static bool bind(DependencyObject source, object target, Hashtable map, BindingMode mode,
                                params object[] args)
        {
            init();
            bool result = false;

            var args2 = new[] {source, target, map, mode};
            args.CopyTo(args2, 4);

            foreach (IBindingMethod method in _methods)
            {
                //if ((bool)method.GetType().GetMethod("bind").Invoke(method,args2)){
                if (method.bind(source, target, map, mode, args))
                {
                    result = true;

                    break;
                }
                ;
            }
            ;

            return result;
        }

        public static void unbind(DependencyObject source, object target)
        {
            init();
            foreach (IBindingMethod method in _methods)
            {
                method.unbind(source, target);
            }
            ;
        }

        public static void unbindFromSource(DependencyObject source)
        {
            init();
            foreach (IBindingMethod method in _methods)
            {
                method.unbindFromSource(source);
            }
            ;
        }

        public static void unbindFromTarget(Object target)
        {
            init();
            foreach (IBindingMethod method in _methods)
            {
                method.unbindFromTarget(target);
            }
            ;
        }

        public static void clear()
        {
            init();
            foreach (IBindingMethod method in _methods)
            {
                method.clear();
            }
            ;
        }
    }
}