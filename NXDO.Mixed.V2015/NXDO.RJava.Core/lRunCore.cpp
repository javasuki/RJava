#include "stdafx.h"
#include "lRunCore.h"
#include "jvmrun.h"
#include "lconvert.h"


jobject lRunCore::jrObject;
jclass lRunCore::jrClass;
JNIEnv* lRunCore::jenv;
jmethodID lRunCore::jm_GetClass;
jmethodID lRunCore::jm_GetClassName;
jmethodID lRunCore::jm_GetEnumOrdinal;
jmethodID lRunCore::jm_CheckIsPrimitive;

jmethodID lRunCore::jm_CreateDate;
jmethodID lRunCore::jm_GetDateString;

jmethodID lRunCore::jm_NewInstance;
jmethodID lRunCore::jm_InvokeMethod;
jmethodID lRunCore::jm_InvokeField;

jmethodID lRunCore::jm_ToString;
jmethodID lRunCore::jm_HashCode;

void lRunCore::init(jobject jb, jclass jc){
	jrObject = jb;
	jrClass = jc;
	jenv = jvmrun::getEnv();

	jm_GetClass = jenv->GetMethodID(jrClass, "getClass", "(Ljava/lang/String;)Ljava/lang/Class;");
	jm_GetClassName = jenv->GetMethodID(jrClass, "getClassName", "(Ljava/lang/Class;)Ljava/lang/String;");
	jm_GetEnumOrdinal = jenv->GetMethodID(jrClass, "getEnumOrdinal", "(Ljava/lang/Object;)I");
	jm_CheckIsPrimitive = jenv->GetMethodID(jrClass, "checkIsPrimitive", "(Ljava/lang/Class;)Z");
	
	jm_CreateDate = jenv->GetMethodID(jrClass, "createDate", "(Ljava/lang/String;Ljava/lang/String;)Ljava/util/Date;");
	jm_GetDateString = jenv->GetMethodID(jrClass, "getDateString", "(Ljava/lang/Object;)Ljava/lang/String;");

	jm_NewInstance = jenv->GetMethodID(jrClass, "newInstance", "(Ljava/lang/Class;[Ljava/lang/Class;[Ljava/lang/Object;)Ljava/lang/Object;");
	jm_InvokeMethod = jenv->GetMethodID(jrClass, "invokeObjectMethodValue", "(Ljava/lang/Object;Ljava/lang/String;[Ljava/lang/Class;[Ljava/lang/Object;)Ljava/lang/Object;");
	jm_InvokeField = jenv->GetMethodID(jrClass,"invokeObjectFieldValue", "(Ljava/lang/Object;Ljava/lang/String;Ljava/lang/Object;Z)Ljava/lang/Object;");

	jm_ToString = jenv->GetMethodID(jrClass, "getObjectToString", "(Ljava/lang/Object;)Ljava/lang/String;");
	jm_HashCode = jenv->GetMethodID(jrClass, "getObjectHashCode", "(Ljava/lang/Object;)I");
}

jclass lRunCore::GetClass(String^ javaClassName){
	jobject jr = jenv->CallObjectMethod(jrObject, jm_GetClass, lconvert::toJString(javaClassName));
	return (jclass)jr;
}

String^ lRunCore::GetClassName(jobject jr){
	jclass jc = jenv->GetObjectClass(jr);
	jstring className = (jstring)jenv->CallObjectMethod(jrObject, jm_GetClassName, jc);
	return lconvert::toNString(className);
}

long lRunCore::GetEnumOrdinal(jobject jr){
	return jenv->CallIntMethod(jrObject, jm_GetEnumOrdinal, jr);
}

bool lRunCore::CheckIsPrimitive(jobject jr){
	jclass jc = jenv->GetObjectClass(jr);
	jboolean b = jenv->CallBooleanMethod(jrObject, jm_CheckIsPrimitive, jc);
	return b == JNI_TRUE ? true : false;
}

jobject lRunCore::CreateDate(String^ date, String^ dFormat)
{
	return jenv->CallObjectMethod(jrObject,jm_CreateDate, lconvert::toJString(date), lconvert::toJString(dFormat));
}

String^ lRunCore::GetDateString(jobject jr){
	jstring sDateTime = (jstring)jenv->CallObjectMethod(jrObject,jm_GetDateString,jr);
	return lconvert::toNString(sDateTime);
}

jobject lRunCore::JNewInstance(jclass jclass, jobjectArray jclasses, jobjectArray Values){
	return jenv->CallObjectMethod(jrObject, jm_NewInstance, jclass, jclasses, Values);
}

jobject lRunCore::JInvokeMethod(jobject obj,String^ methodName,jobjectArray jclasses, jobjectArray Values){
	return jenv->CallObjectMethod(jrObject, jm_InvokeMethod, obj, lconvert::toJString(methodName), jclasses, Values);
}

jobject lRunCore::JInvokeField(jobject obj,String^ fieldName,jobject value, bool isSet){
	return jenv->CallObjectMethod(jrObject, jm_InvokeField, obj, lconvert::toJString(fieldName), value, isSet);
}

jstring lRunCore::JGetToString(jobject obj)
{
	return (jstring)jenv->CallObjectMethod(jrObject, jm_ToString, obj);
}

jint lRunCore::JGetHashCode(jobject obj)
{
	return jenv->CallIntMethod(jrObject, jm_HashCode, obj);
}
