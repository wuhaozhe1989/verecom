using System;
using System.Collections;
using UnityEngine;
public static class FrameDelayCall
{
    public delegate void DelayCall();

//	public delegate void DelayCall <T>(T arg1);
    private static bool inited;

    private static int counter;
    private static BetterList<CallObj> calls = new BetterList<CallObj>();
    private static BetterList<CallTimeObj> calltimes = new BetterList<CallTimeObj>();
	/// <summary>
	/// 删除延迟执行.
	/// </summary>
	/// <param name='call'>
	/// Call.
	/// </param>
    public static void remove(CallObj call)
    {
        if (call != null) calls.Remove(call);
    }
	public static void removeTime(CallTimeObj call){
		if(call!=null)calltimes.Remove(call);	
	}
    /// <summary>
    /// 添加延迟执行按照帧数 call, delayframes, mn and isUnique.
    /// </summary>
    /// <param name='call'>
    /// 方法.
    /// </param>
    /// <param name='delayframes'>
    /// 延迟帧数.
    /// </param>
    /// <param name='mn'>
    /// 承载回掉函数的实例是否存在的判断.
    /// </param>
    /// <param name='isUnique'>
    /// 是否是唯一的方法.
    /// </param>
    public static CallObj Add(DelayCall call, int delayframes, MonoBehaviour mn = null, bool isUnique = false)
    {
        if (!inited)
        {
            inited = true;
            UpdateManager.AddCoroutine(null, 0, OnUpdate);
        }
        if (isUnique)
        {
            for (int i = 0; i < calls.size; i++)
            {
                CallObj call2 = calls[i];
                if (call2.mn == mn && call2.call == call)
                {
                    return call2;
                }
            }
        }

        var callobj = new CallObj();
        callobj.call = call;
        callobj.isMN = (mn != null);
        callobj.mn = mn;
        callobj.frame = counter + delayframes;
        calls.Add(callobj);

        return callobj;
        //calls.Add(Time.frameCount+delayframes,call);
        //calls[Time.frameCount+delayframes]+=call;
    }
    public static CallTimeObj AddTime(DelayCall call, float delayTime, MonoBehaviour mn = null, bool isUnique = false)
    {
        if (!inited)
        {
            inited = true;
            UpdateManager.AddCoroutine(null, 0, OnUpdate);
        }
        if (isUnique)
        {
            for (int i = 0; i < calltimes.size; i++)
            {
                CallTimeObj call2 = calltimes[i];
                if (call2.mn == mn && call2.call == call)
                {
                    return call2;
                }
            }
        }

        var callobj = new CallTimeObj();
        callobj.call = call;
        callobj.isMN = (mn != null);
        callobj.mn = mn;
        callobj.time = Time.realtimeSinceStartup+delayTime;
        calltimes.Add(callobj);

        return callobj; 
    }


    private static void OnUpdate(float delta)
    {
        
        ++counter;
        if(calls.size!=0)for (int i = 0; i < calls.size; ++i)
        {
            CallObj call = calls[i];
            if (call.frame <= counter)
            {
                calls.RemoveAt(i);
                if (call.isMN && call.mn == null )//|| !call.mn.enabled))
                {
                }
                else
                {
					try{
                    	call.call();
					}catch(Exception e){
						Debug.LogException(e);
					}
                }


                --i;
            }
        }

        //time call
        if(calltimes.size!=0)for (int i = 0; i < calltimes.size; ++i)
        {
            CallTimeObj call = calltimes[i];
            if (call.time <= Time.realtimeSinceStartup)
            {
                calltimes.RemoveAt(i);
                if (call.isMN && call.mn == null )//|| !call.mn.enabled))
                {
                }
                else
                {
					try{
                   		call.call();
					}catch(Exception e){
						Debug.LogException(e);
					}
				}


                --i;
            }
        }
        
    }
    public static IEnumerator waitForSeconds(float time)
    {
       
        yield return new WaitForSeconds(time);

    }
    public class CallTimeObj{
        public DelayCall call;
        public float time;
        public bool isMN;
        public MonoBehaviour mn;
    }
    public class CallObj
    {
        public DelayCall call;
        public int frame;
        public bool isMN;
        public MonoBehaviour mn;
    }
}