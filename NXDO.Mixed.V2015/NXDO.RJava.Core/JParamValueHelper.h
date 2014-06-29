#include "jvmrun.h"

#pragma once
using namespace System;

namespace NXDO {
	namespace RJava{
		ref class JParamValueHelper
		{
		private:
			static property JNIEnv* JEnv{
				JNIEnv* get(){
					return jvmrun::getEnv();
				};
			}

		public:
			static IntPtr GetJavaClass(String^ javaClassName);

			generic <typename T>
			static IntPtr ToPrimitiveValue(T value);
			static IntPtr ToPrimitiveValue(Object^ value, Type^ type);

			static IntPtr NewDate(String^ date, String^ format);						
			
			generic <typename T>
			where T : ValueType
			static IntPtr CreatePrimitiveArray(long size);

			generic <typename T>
			where T : ValueType
			static void SetValuePrimitiveArray(IntPtr aryPtr, long index, T value);

			static IntPtr CreateObjectArray(IntPtr elemClassPtr, long size);
			static void SetValueObjectArray(IntPtr aryPtr, long index, IntPtr value);

			generic <typename T>
			where T : ValueType
			static IntPtr GetPrimitiveArray(array<T>^ tArray);

			generic <typename T>
			static IntPtr GetObjectArray(String^ javaArrayElemClassName, array<T>^ tArray);
		};
	}
}

