#include "jni.h"

#pragma once
using namespace System;

class lReflection
{
private:
	static jobject jrObject;
	static jclass jrClass;
	static JNIEnv* jenv;
	static jmethodID jm_GetClass;
	static jmethodID jm_GetClassName;
	static jmethodID jm_GetEnumOrdinal;
	static jmethodID jm_CheckIsPrimitive;

	static jmethodID jm_CreateDate;
	static jmethodID jm_CreateCalendar;
	static jmethodID jm_GetDateString;

	static jmethodID jm_NewInstance;
	static jmethodID jm_InvokeMethod;
	static jmethodID jm_InvokeField;

	static jmethodID jm_ToString;
	static jmethodID jm_HashCode;
public:
	lReflection(void);
	~lReflection(void);

	static void init(jobject jb, jclass jc);

	static jclass GetClass(String^ javaClassName);
	static String^ GetClassName(jobject jr);
	static long GetEnumOrdinal(jobject jr);
	static bool CheckIsPrimitive(jobject jr);

	static jobject CreateDate(String^ date, String^ dFormat);
	static jobject CreateCalendar(String^ date, String^ dFormat);
	static String^ GetDateString(jobject jr);

	static jobject JNewInstance(jclass jclass, jobjectArray jclasses, jobjectArray Values);
	static jobject JInvokeMethod(jobject obj,String^ methodName,jobjectArray jclasses, jobjectArray Values);
	static jobject JInvokeField(jobject obj,String^ fieldName,jobject value, bool isSet);

	static jstring JGetToString(jobject obj);
	static jint JGetHashCode(jobject obj);
};

