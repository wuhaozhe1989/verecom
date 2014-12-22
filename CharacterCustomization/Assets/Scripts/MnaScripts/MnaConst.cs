using UnityEngine;
using System.Collections;

public class MnaConst {


	
	//手机运营商
	public enum ENUM_MNA_MOBILE_ISP
	{
		E_MNA_MOBILE_UNKNOWN			= 0,  //未知
		E_MNA_MOBILE_TELCOM				= 1,  //中国电信
		E_MNA_MOBILE_UNICOM				= 2,  //中国联通
		E_MNA_MOBILE_CHINAMOBILE		= 4,  //中国移动
	};

	//手机操作系统
	public enum ENUM_MNA_MOBILE_OS
	{
	    E_MNA_OS_U3D_ANDRIOD            = 3,  // U3D for android
        E_MNA_OS_U3D_IOS                = 4,  // U3D for iOS
	};
	
	//网络类型
	public enum ENUM_MNA_NETTYPE
	{
		E_MNA_NETTYPE_UNKNOWN			= 0,  // 未知
		E_MNA_NETTYPE_WIFI				= 1,  // WIFI
		E_MNA_NETTYPE_2G				= 2,  // 2G
		E_MNA_NETTYPE_3G				= 3,  // 3G
		E_MNA_NETTYPE_4G				= 4,  // 4G
	};
}
