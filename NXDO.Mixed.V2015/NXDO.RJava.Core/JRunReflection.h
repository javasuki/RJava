#include "jvmrun.h"
#include "lRunReflection.h"
#include "JParamValue.h"
#include "JException.h"

#pragma once
namespace NXDO {
	namespace RJava{
		ref class JRunReflection
		{
		private:
			jclass tReflectionClass;
			JNIEnv* jenv;

		internal:
			JRunReflection(jclass jReflectionClass);

			array<Boolean>^ GetClassFlag(IntPtr classPtr);
			array<IntPtr>^ GetGenericArguments(IntPtr classPtr);
			IntPtr GetSuperClass(IntPtr classPtr);
			IntPtr GetDeclaringClass(IntPtr classPtr);
			IntPtr GetElementClass(IntPtr classPtr);
			array<IntPtr>^ GetInterfaces(IntPtr classPtr);
			bool GetIsAssignableFrom(IntPtr classPtr, IntPtr classPtr2);
			bool GetIsInstance(IntPtr classPtr, IntPtr objectPtr);
			IntPtr GetAsSubClass(IntPtr classPtr, IntPtr parentClassPtr);
			IntPtr GetAsCast(IntPtr classPtr, IntPtr objectPtr);
			String^ GetClassName(IntPtr classPtr);

			IntPtr GetMethodDeclaringClass(IntPtr methodPtr);
			

			String^ GetConstructorName(IntPtr ctorPtr);
			array<Boolean>^ GetCtorModifier(IntPtr ctorPtr);
			IntPtr GetConstructor(IntPtr classPtr, array<IntPtr>^ paramTypes);
			array<IntPtr>^ GetConstructors(IntPtr classPtr);
			IntPtr InvokeConstructor(IntPtr ctorPtr, array<IParamValue^>^ values);
			IntPtr GetConstructorDeclaringClass(IntPtr ctorPtr);

			String^ GetMethodName(IntPtr methodPtr);
			array<Boolean>^ GetAccessModifier(IntPtr methodPtr);
			IntPtr GetMethod(IntPtr classPtr, String^ methodName, array<IntPtr>^ paramTypes);
			array<IntPtr>^ GetMethods(IntPtr classPtr);
			IntPtr InvokeMethod(IntPtr jObjectClassPtr, IntPtr methodPtr, array<IParamValue^>^ values);
			Boolean CheckIsArray(IntPtr jObjectPtr);

			IntPtr GetField(IntPtr classPtr, String^ fieldName);
			array<IntPtr>^ GetFields(IntPtr classPtr);
			String^ GetFieldName(IntPtr fldPtr);
			array<Boolean>^ GetFieldModifier(IntPtr fldPtr);
			IntPtr GetFieldDeclaringClass(IntPtr fldPtr);

			IntPtr GetFieldValue(IntPtr fldPtr, IntPtr objPtr);
			void SetFieldValue(IntPtr fldPtr, IntPtr objPtr, IParamValue^ value);


			array<IntPtr>^ GetMethodParams(IntPtr methodPtr);

			void DisposeSelf();
		};
	}
}

