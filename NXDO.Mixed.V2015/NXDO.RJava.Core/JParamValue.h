#include "jvmrun.h";
#include "lRunCore.h"

#pragma once
using namespace System;

namespace NXDO {
	namespace RJava{
		/// <summary>
		/// Java 参数包装器的接口。
		/// </summary>
		interface class IParamValue
		{
			/// <summary>
			/// jobject 的指针, 对应 int,Integer,Object ...
			/// </summary>
			property IntPtr JValue;

			/// <summary>
			/// jclass 的指针，对应 java.lang.Class&lt;?&gt;
			/// </summary>
			property IntPtr JClass{
				IntPtr get();
			};

			void ChangeJavaClass(IntPtr javaClassPtr);
		};

		/// <summary>
		/// Java 参数包装器。
		/// </summary>
		ref class JParamValue abstract : IParamValue
		{
		protected:
			/// <summary>
			/// 初始化 Java 参数值包装器。
			/// </summary>
			/// <param name="paramJavaClassName">java方法的参数类型名称。</param>
			JParamValue(String^ paramJavaClassName);
			
		private:
			IntPtr _jClass;
			IntPtr _jValue;

			//jobject ConvertJValue(Type^ jcType, Object^ v);
			//jobjectArray ConvertJValueArray(Type^ jcType, Object^ v, jclass jAryElemClass);
			//String^ GetArrayElemClassName(String^ javaArrayClassName);

		public:
			/// <summary>
			/// java参数的指针值
			/// </summary>
			virtual property IntPtr JValue{
				IntPtr get(){return _jValue;};
				void set(IntPtr ptr){_jValue = ptr;};
			}

			/// <summary>
			/// java参数类型的指针值
			/// </summary>
			virtual property IntPtr JClass{
				IntPtr get(){return _jClass;};
			}

			virtual void ChangeJavaClass(IntPtr javaClassPtr);
		
		internal:
			static array<IParamValue^>^ GetParams(... array<IParamValue^>^ values);

		//internal:
		//	/// <summary>	
		//	/// java方法调用时需要的参数值
		//	/// </summary>
		//	property jobject Value{
		//		jobject get(){
		//			return (jobject)this->JValue.ToPointer();
		//		};
		//	}

		//	/// <summary>
		//	/// java方法调用时需要匹配的参数类型
		//	/// </summary>
		//	property jclass Class{
		//		jclass get(){
		//			return (jclass)this->JClass.ToPointer();
		//		};
		//	}

			
		};
	}
}

