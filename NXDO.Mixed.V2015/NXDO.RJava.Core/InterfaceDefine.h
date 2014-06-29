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

		//	//����ֵΪ JObject ����ʱ��java�˷��غ��ȡ�������ʵ��������
		//	property String^ ReturnJObjectOfJClassName
		//	{
		//		String^ get();
		//		void set(String^);
		//	}
		//};
	}
}