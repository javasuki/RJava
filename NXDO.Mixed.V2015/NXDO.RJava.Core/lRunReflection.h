#include "jni.h"

#pragma once
using namespace System;

class lRunReflection
{
private:
	static jclass jrClass;
	static jclass aryElemClass;
	static JNIEnv* jenv;

	static jmethodID jm_GetGenericArguments;
	static jmethodID jm_GetSuperClass;
	static jmethodID jm_GetDeclaringClass;
	static jmethodID jm_GetElementClass;
	static jmethodID jm_GetInterfaces;

	static jmethodID jm_GetIsAssignableFrom;
	static jmethodID jm_GetIsInstance;
	static jmethodID jm_GetAsSubClass;
	static jmethodID jm_GetAsCast;
	static jmethodID jm_GetClassName;
	static jmethodID jm_GetClassFlag;
	
	static jmethodID jm_GetCtor;
	static jmethodID jm_GetCtors;
	static jmethodID jm_GetCtorName;
	static jmethodID jm_InvokeCtor;
	static jmethodID jm_GetCtorDeclaringClass;
	static jmethodID jm_GetCtorModifier;	


	static jmethodID jm_GetMethodDeclaringClass;
	static jmethodID jm_GetMethodName;
	static jmethodID jm_GetAccessModifier;	


	static jmethodID jm_GetMethod;
	static jmethodID jm_GetMethods;
	static jmethodID jm_InvokeMethod;
	static jmethodID jm_CheckArray;

	static jmethodID jm_GetField;
	static jmethodID jm_GetFields;
	static jmethodID jm_GetFieldName;
	static jmethodID jm_GetFieldModifier;
	static jmethodID jm_GetFieldDeclaringClass;

	static jmethodID jm_GetFieldValue;
	static jmethodID jm_SetFieldValue;

	static jmethodID jm_GetMethodParams;

public:
	static void init(jclass jc);

	static IntPtr GetField(IntPtr classPtr, String^ fieldName);
	static array<IntPtr>^ GetFields(IntPtr classPtr);
	static String^ GetFieldName(IntPtr fldPtr);
	static array<Boolean>^ GetFieldModifier(IntPtr fldPtr);
	static IntPtr GetFieldDeclaringClass(IntPtr fldPtr);

	static IntPtr GetFieldValue(IntPtr fldPtr, IntPtr objPtr);
	static void SetFieldValue(IntPtr fldPtr, IntPtr objPtr, jobject value);

	static array<IntPtr>^ GetGenericArguments(IntPtr classPtr);
	static IntPtr GetSuperClass(IntPtr classPtr);
	static IntPtr GetDeclaringClass(IntPtr classPtr);
	static IntPtr GetElementClass(IntPtr classPtr);
	static array<IntPtr>^ GetInterfaces(IntPtr classPtr);

	static bool GetIsAssignableFrom(IntPtr classPtr, IntPtr classPtr2);
	static bool GetIsInstance(IntPtr classPtr, IntPtr objectPtr);
	static IntPtr GetAsSubClass(IntPtr classPtr, IntPtr parentClassPtr);
	static IntPtr GetAsCast(IntPtr classPtr, IntPtr objectPtr);
	static String^ GetClassName(IntPtr classPtr);
	static array<Boolean>^ GetClassFlag(IntPtr classPtr);
	
	
	
	static IntPtr GetConstructor(IntPtr classPtr, array<IntPtr>^ paramTypes);
	static array<IntPtr>^ GetConstructors(IntPtr classPtr);
	static String^ GetConstructorName(IntPtr ctorPtr);
	static array<Boolean>^ GetCtorModifier(IntPtr ctorPtr);
	static jobject InvokeConstructor(IntPtr ctorPtr, jobjectArray paramValues);	
	static IntPtr GetConstructorDeclaringClass(IntPtr ctorPtr);


	static IntPtr GetMethodDeclaringClass(IntPtr methodPtr);
	static String^ GetMethodName(IntPtr methodPtr);
	static array<Boolean>^ GetAccessModifier(IntPtr methodPtr);

	static jobject GetMethod(IntPtr classPtr, String^ methodName, array<IntPtr>^ paramTypes);
	static array<IntPtr>^ GetMethods(IntPtr classPtr);
	static jobject InvokeMethod(IntPtr methodPtr, IntPtr jObjectPtr, jobjectArray paramValues);
	static jboolean CheckIsArray(IntPtr objectPtr);


	static array<IntPtr>^ GetMethodParams(IntPtr methodPtr);
};

