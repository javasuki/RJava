#include "stdafx.h"
#include "lReflection.h"
#include "jvmrun.h"
#include "lconvert.h"

lReflection::lReflection(void)
{
}


lReflection::~lReflection(void)
{
}

jobject lReflection::jrObject;
jclass lReflection::jrClass;
JNIEnv* lReflection::jenv;
jmethodID lReflection::jm_GetClass;
jmethodID lReflection::jm_GetClassName;
jmethodID lReflection::jm_GetEnumOrdinal;
jmethodID lReflection::jm_CheckIsPrimitive;

jmethodID lReflection::jm_CreateDate;
jmethodID lReflection::jm_CreateCalendar;
jmethodID lReflection::jm_GetDateString;

jmethodID lReflection::jm_NewInstance;
jmethodID lReflection::jm_InvokeMethod;
jmethodID lReflection::jm_InvokeField;

jmethodID lReflection::jm_ToString;
jmethodID lReflection::jm_HashCode;

void lReflection::init(jobject jb, jclass jc){
	jrObject = jb;
	jrClass = jc;
	jenv = jvmrun::getEnv();

	jm_GetClass = jenv->GetMethodID(jrClass, "getClass", "(Ljava/lang/String;)Ljava/lang/Class;");
	jm_GetClassName = jenv->GetMethodID(jrClass, "getClassName", "(Ljava/lang/Class;)Ljava/lang/String;");
	jm_GetEnumOrdinal = jenv->GetMethodID(jrClass, "getEnumOrdinal", "(Ljava/lang/Object;)I");
	jm_CheckIsPrimitive = jenv->GetMethodID(jrClass, "checkIsPrimitive", "(Ljava/lang/Class;)Z");
	
	jm_CreateDate = jenv->GetMethodID(jrClass, "createDate", "(Ljava/lang/String;Ljava/lang/String;)Ljava/util/Date;");
	jm_CreateCalendar = jenv->GetMethodID(jrClass, "createCalendar", "(Ljava/lang/String;Ljava/lang/String;)Ljava/util/Calendar;");
	jm_GetDateString = jenv->GetMethodID(jrClass, "getDateString", "(Ljava/lang/Object;)Ljava/lang/String;");

	jm_NewInstance = jenv->GetMethodID(jrClass, "newInstance", "(Ljava/lang/Class;[Ljava/lang/Class;[Ljava/lang/Object;)Ljava/lang/Object;");
	jm_InvokeMethod = jenv->GetMethodID(jrClass, "invokeObjectMethodValue", "(Ljava/lang/Object;Ljava/lang/String;[Ljava/lang/Class;[Ljava/lang/Object;)Ljava/lang/Object;");
	jm_InvokeField = jenv->GetMethodID(jrClass,"invokeObjectFieldValue", "(Ljava/lang/Object;Ljava/lang/String;Ljava/lang/Object;Z)Ljava/lang/Object;");

	jm_ToString = jenv->GetMethodID(jrClass, "getObjectToString", "(Ljava/lang/Object;)Ljava/lang/String;");
	jm_HashCode = jenv->GetMethodID(jrClass, "getObjectHashCode", "(Ljava/lang/Object;)I");
}

jclass lReflection::GetClass(String^ javaClassName){
	jobject jr = jenv->CallObjectMethod(jrObject, jm_GetClass, lconvert::toJString(javaClassName));
	return (jclass)jr;
}

String^ lReflection::GetClassName(jobject jr){
	jclass jc = jenv->GetObjectClass(jr);
	jstring className = (jstring)jenv->CallObjectMethod(jrObject, jm_GetClassName, jc);
	return lconvert::toNString(className);
}

long lReflection::GetEnumOrdinal(jobject jr){
	return jenv->CallIntMethod(jrObject, jm_GetEnumOrdinal, jr);
}

bool lReflection::CheckIsPrimitive(jobject jr){
	jclass jc = jenv->GetObjectClass(jr);
	jboolean b = jenv->CallBooleanMethod(jrObject, jm_CheckIsPrimitive, jc);
	return b == JNI_TRUE ? true : false;
}

jobject lReflection::CreateDate(String^ date, String^ dFormat)
{
	return jenv->CallObjectMethod(jrObject,jm_CreateDate, lconvert::toJString(date), lconvert::toJString(dFormat));
}

jobject lReflection::CreateCalendar(String^ date, String^ dFormat){
	return jenv->CallObjectMethod(jrObject,jm_CreateCalendar,lconvert::toJString(date), lconvert::toJString(dFormat));
}

String^ lReflection::GetDateString(jobject jr){
	jstring sDateTime = (jstring)jenv->CallObjectMethod(jrObject,jm_GetDateString,jr);
	return lconvert::toNString(sDateTime);
}

jobject lReflection::JNewInstance(jclass jclass, jobjectArray jclasses, jobjectArray Values){
	return jenv->CallObjectMethod(jrObject, jm_NewInstance, jclass, jclasses, Values);
}

jobject lReflection::JInvokeMethod(jobject obj,String^ methodName,jobjectArray jclasses, jobjectArray Values){
	return jenv->CallObjectMethod(jrObject, jm_InvokeMethod, obj, lconvert::toJString(methodName), jclasses, Values);
}

jobject lReflection::JInvokeField(jobject obj,String^ fieldName,jobject value, bool isSet){
	return jenv->CallObjectMethod(jrObject, jm_InvokeField, obj, lconvert::toJString(fieldName), value, isSet);
}

jstring lReflection::JGetToString(jobject obj)
{
	return (jstring)jenv->CallObjectMethod(jrObject, jm_ToString, obj);
}

jint lReflection::JGetHashCode(jobject obj)
{
	return jenv->CallIntMethod(jrObject, jm_HashCode, obj);
}
