#include "jvmrun.h";
#include "lRunCore.h"

#pragma once
using namespace System;

namespace NXDO {
	namespace RJava{
		/// <summary>
		/// Java ������װ���Ľӿڡ�
		/// </summary>
		interface class IParamValue
		{
			/// <summary>
			/// jobject ��ָ��, ��Ӧ int,Integer,Object ...
			/// </summary>
			property IntPtr JValue;

			/// <summary>
			/// jclass ��ָ�룬��Ӧ java.lang.Class&lt;?&gt;
			/// </summary>
			property IntPtr JClass{
				IntPtr get();
			};

			void ChangeJavaClass(IntPtr javaClassPtr);
		};

		/// <summary>
		/// Java ������װ����
		/// </summary>
		ref class JParamValue abstract : IParamValue
		{
		protected:
			/// <summary>
			/// ��ʼ�� Java ����ֵ��װ����
			/// </summary>
			/// <param name="paramJavaClassName">java�����Ĳ����������ơ�</param>
			JParamValue(String^ paramJavaClassName);
			
		private:
			IntPtr _jClass;
			IntPtr _jValue;

			//jobject ConvertJValue(Type^ jcType, Object^ v);
			//jobjectArray ConvertJValueArray(Type^ jcType, Object^ v, jclass jAryElemClass);
			//String^ GetArrayElemClassName(String^ javaArrayClassName);

		public:
			/// <summary>
			/// java������ָ��ֵ
			/// </summary>
			virtual property IntPtr JValue{
				IntPtr get(){return _jValue;};
				void set(IntPtr ptr){_jValue = ptr;};
			}

			/// <summary>
			/// java�������͵�ָ��ֵ
			/// </summary>
			virtual property IntPtr JClass{
				IntPtr get(){return _jClass;};
			}

			virtual void ChangeJavaClass(IntPtr javaClassPtr);
		
		internal:
			static array<IParamValue^>^ GetParams(... array<IParamValue^>^ values);

		//internal:
		//	/// <summary>	
		//	/// java��������ʱ��Ҫ�Ĳ���ֵ
		//	/// </summary>
		//	property jobject Value{
		//		jobject get(){
		//			return (jobject)this->JValue.ToPointer();
		//		};
		//	}

		//	/// <summary>
		//	/// java��������ʱ��Ҫƥ��Ĳ�������
		//	/// </summary>
		//	property jclass Class{
		//		jclass get(){
		//			return (jclass)this->JClass.ToPointer();
		//		};
		//	}

			
		};
	}
}

