#pragma once

using namespace System;

namespace NXDO {
	namespace JBridge{

		interface class IObject
		{
			void setJClassPtr(IntPtr ptr);
			void setJObjectPtr(IntPtr ptr);

			property IntPtr JObjectPtr {
				IntPtr get();
			}

			property IntPtr JClassPtr{
				IntPtr get();
			};

			String^ ToString(); 

			int GetHashCode();

			bool Equals(Object^ obj);
		};

		//interface class IJObject
		//{
		//};


		//interface class IJReturn
		//{
		//	property Type^ ReturnType
		//	{
		//		Type^ get();
		//	}

		//	property bool IsArray
		//	{
		//		bool get();
		//	}

		//	property bool IsSubJObject
		//	{
		//		bool get();
		//	}

		//	property bool IsJObject
		//	{
		//		bool get();
		//	}
		//	

		//	//返回值为 JObject 类型时，java端返回后获取结果的真实类型名称
		//	property String^ ReturnJObjectOfJClassName
		//	{
		//		String^ get();
		//		void set(String^);
		//	}
		//};
	}
}