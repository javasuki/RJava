#include "jni.h"
#include "jvmrun.h"

#pragma once
using namespace System;
using namespace System::Reflection;

namespace NXDO {
	namespace RJava{
		ref class JReturnValueHelper
		{
		private:
			static MethodInfo^ toArrayMethod;
			static void addArrayElemToList(jobjectArray ary, System::Collections::IList^ gList, Type^ aryElemType,bool isPrimitiveObject,  bool isSubJObject);

		public:
			generic <typename T>
			static T GetObjectValue(IntPtr ptr, bool isSubJObject);

			generic <typename T>
			static T GetObjectArray(IntPtr ptr, bool isSubJObject);

			static int GetArraySize(IntPtr ptr);
			static IntPtr GetArrayElem(IntPtr ptr, long index);

			//static String^ GetDateTime(IntPtr ptr);
			static String^ GetClassName(IntPtr ptr);
			static bool CheckIsPrimitive(IntPtr ptr);
		};
	}
}

