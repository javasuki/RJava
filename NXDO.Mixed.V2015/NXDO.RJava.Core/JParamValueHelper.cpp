#include "jni.h"
#include "stdafx.h"
#include "JParamValueHelper.h"
#include "lconvert.h"
#include "larray.h"
#include "ExtOper.h"

using namespace System;
using namespace NXDO::RJava;

IntPtr JParamValueHelper::GetJavaClass(String^ javaClassName)
{
	jclass jc = lRunCore::GetClass(javaClassName);
	return IntPtr(jc);
}

generic <typename T>
IntPtr JParamValueHelper::ToPrimitiveValue(T value)
{
	return lconvert::convertToJavaObject(value, T::typeid);
}

IntPtr JParamValueHelper::ToPrimitiveValue(Object^ value, Type^ type)
{
	return lconvert::convertToJavaObject(value, type);
}


IntPtr JParamValueHelper::NewDate(String^ date, String^ format)
{
	jobject jo = lRunCore::CreateDate(date, format);
	return IntPtr(jo);
}
			
generic <typename T>
IntPtr JParamValueHelper::CreatePrimitiveArray(long size)
{
	//System::Collections::Generic::List<int>^ lst = nullptr;
	jobjectArray jobjAry = NULL;
	if(T::typeid == Boolean::typeid)
		jobjAry = (jobjectArray)JEnv->NewBooleanArray(size);
	else if(T::typeid == Byte::typeid)
		jobjAry = (jobjectArray)JEnv->NewByteArray(size);
	else if(T::typeid == Char::typeid)
		jobjAry = (jobjectArray)JEnv->NewCharArray(size);
	else if(T::typeid == Int16::typeid)
		jobjAry = (jobjectArray)JEnv->NewShortArray(size);
	else if(T::typeid == Int32::typeid)
		jobjAry = (jobjectArray)JEnv->NewIntArray(size);
	else if(T::typeid == Int64::typeid)
		jobjAry = (jobjectArray)JEnv->NewLongArray(size);
	else if(T::typeid == Single::typeid)
		jobjAry = (jobjectArray)JEnv->NewFloatArray(size);
	else if(T::typeid == Double::typeid)
		jobjAry = (jobjectArray)JEnv->NewDoubleArray(size);

	return IntPtr(jobjAry);
}

generic <typename T>
void JParamValueHelper::SetValuePrimitiveArray(IntPtr aryPtr, long index, T value)
{
	//Object^ value = t;
	jobjectArray jobjAry = (jobjectArray)(aryPtr.ToPointer());
	if(T::typeid == Boolean::typeid){
		jboolean val = safe_cast<bool>(value);
		JEnv->SetBooleanArrayRegion((jbooleanArray)jobjAry, index, 1, &val);
	}
	else if(T::typeid == Byte::typeid){
		jbyte val = (byte)value;
		JEnv->SetByteArrayRegion((jbyteArray)jobjAry, index, 1, &val);
	}
	else if(T::typeid == Char::typeid){
		jchar val = Convert::ToChar(value);
		JEnv->SetCharArrayRegion((jcharArray)jobjAry, index, 1, &val);
	}
	else if(T::typeid == Int16::typeid){
		jshort val = (short)value;
		JEnv->SetShortArrayRegion((jshortArray)jobjAry, index, 1, &val);
	}
	else if(T::typeid == Int32::typeid){
		jint val = (long)value;
		JEnv->SetIntArrayRegion((jintArray)jobjAry, index, 1, &val);
	}
	else if(T::typeid == Int64::typeid){
		jlong val = Convert::ToInt64(value);
		JEnv->SetLongArrayRegion((jlongArray)jobjAry, index, 1, &val);
	}
	else if(T::typeid == Single::typeid){
		jfloat val = (float)value;
		JEnv->SetFloatArrayRegion((jfloatArray)jobjAry, index, 1, &val);
	}
	else if(T::typeid == Double::typeid){
		jdouble val = (double)value;
		JEnv->SetDoubleArrayRegion((jdoubleArray)jobjAry, index, 1, &val);
	}
}

IntPtr JParamValueHelper::CreateObjectArray(IntPtr elemClassPtr, long size)
{
	jclass aryElemClass = (jclass)elemClassPtr.ToPointer();
	jobjectArray jAry = JEnv->NewObjectArray(size, aryElemClass, 0);
	return IntPtr(jAry);
}

void JParamValueHelper::SetValueObjectArray(IntPtr aryPtr, long index, IntPtr value)
{
	jobjectArray jAry = (jobjectArray)aryPtr.ToPointer();
	jobject jVal = (jobject)value.ToPointer();
	JEnv->SetObjectArrayElement(jAry, index, jVal);
}


generic <typename T>
IntPtr JParamValueHelper::GetPrimitiveArray(array<T>^ tArray)
{
	long size = tArray == nullptr ? 0 : tArray->Length;
	IntPtr ptr = JParamValueHelper::CreatePrimitiveArray<T>(size);
	if(size == 0) return ptr;

	for(long i=0;i<size;i++){
		T t = (T)tArray->GetValue(i);
		JParamValueHelper::SetValuePrimitiveArray<T>(ptr,i, t);
	}
	return ptr;
}

generic <typename T>
IntPtr JParamValueHelper::GetObjectArray(String^ javaArrayElemClassName, array<T>^ tArray){
	long size = tArray == nullptr ? 0 : tArray->Length;
	IntPtr clsPtr = JParamValueHelper::GetJavaClass(javaArrayElemClassName);
	IntPtr aryPtr = JParamValueHelper::CreateObjectArray(clsPtr, size);
	if(size == 0) return aryPtr;

	for(long i=0;i<size;i++){
		T t = (T)tArray->GetValue(i);
		bool bIsPtr = IS<IntPtr>((Object^)t);
		
		IntPtr valPtr = bIsPtr ? (IntPtr)t : JParamValueHelper::ToPrimitiveValue(t);
		JParamValueHelper::SetValueObjectArray(aryPtr,i, valPtr);
	}
	return aryPtr;
}