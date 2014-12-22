using UnityEngine;

public static class NumberUtil
{
    public static int randomInt(int min, int max)
    {
        return
            Random.Range(min, max);
    }
	static string cutS(string s,int lengthLimit){
		
		if(lengthLimit!=0&&s.Length>lengthLimit){
				if(s.IndexOf(".")>0){
					var np=s.Split(new char[]{'.'});
					string ss=np[1];
					int needreduce=s.Length-lengthLimit-1;
					if(needreduce>=ss.Length){
						return np[0];
					}
					ss=ss.Substring(0,ss.Length-needreduce);
                    s=np[0]+"."+ss;
				}
				
			}	
		return s;
	}

    //格式化数字，进行缩减 如果要保证出来的字符长度则lengthLimit要付值，最小不要小于3
    //
    public static string formatLongNumber(string longnumber, int lenghLimit = 0)
    {
		 string s = "";
		/*if(Localization.isChinese){
			 if (longnumber.Length > 12)
	        {
	            s = longnumber.Substring(0, longnumber.Length - 8);
				
				
				s=((float)(int.Parse(s)/10)/1000).ToString();
				s=cutS(s,lenghLimit);
	            s += Localization.Localize("num12","万亿");
				
	        }else   if (longnumber.Length > 8)
	        {
	            s = longnumber.Substring(0, longnumber.Length - 4);
				s=((float)(int.Parse(s)/10)/1000).ToString();
				s=cutS(s,lenghLimit);
	            s += Localization.Localize("num8","亿");
	        }else if (longnumber.Length > 4)
	        {
	            //s = longnumber.Substring(0, longnumber.Length - 4);
				s=((float)(int.Parse(longnumber)/10)/1000).ToString();
				s=cutS(s,lenghLimit);
	            s += Localization.Localize("num4","万");
                //暂时屏蔽千这个变量
	        }else if (longnumber.Length > 3)
            {
                //s = longnumber.Substring(0, longnumber.Length - 3);
				s=((float)(int.Parse(longnumber)/10)/100).ToString();
				s=cutS(s,lenghLimit);
                s += Localization.Localize("num3","千");
            }else
	        {
	            s += longnumber;
	        }
			
			return s;
		}*/
        //billion
       
        if (longnumber.Length > 12)
        {
            s = formatAddDou(longnumber.Substring(0, longnumber.Length - 9));
            s += "B";
        }else   if (longnumber.Length > 9)
        {
            s = formatAddDou(longnumber.Substring(0, longnumber.Length - 6));
            s += "M";
        }
        else if (longnumber.Length > 6)
        {
            s = formatAddDou(longnumber.Substring(0, longnumber.Length - 3));
            s += "K";
        }
        else
        {
            s += formatAddDou(longnumber);
        }
        if (lenghLimit >= 3)
        {
            if (s.Length > lenghLimit+1)
            {
                longnumber = longnumber.Substring(0, longnumber.Length - 3);
                s = formatAddDou(longnumber) + "K";
            }
            if (s.Length > lenghLimit+1)
            {
				//Debug.Log(longnumber);
                longnumber = longnumber.Substring(0, longnumber.Length - 3);
                s = formatAddDou(longnumber) + "M";
            }
			 if (s.Length > lenghLimit+1)
            {
                longnumber = longnumber.Substring(0, longnumber.Length - 3);
                s = formatAddDou(longnumber) + "B";
            }
        }
        return s;
    }

    public static string formatAddDou(string num)
    {
		if(num.Length<1)return num;
        string s = "";
        string addsign = "";

        if (num.Substring(0, 1) == "-")
        {
            addsign = "-";
            num = num.Substring(1);
        }

        char[] strs = num.ToCharArray();
        int n = 0;
        for (int i = strs.Length - 1; i >= 0; i--)
        {
            s = strs[i] + s;
            if (n%3 == 2 && n != strs.Length - 1)
            {
                s = "," + s;
            }
            n++;
        }
        return addsign + s;
    }
}