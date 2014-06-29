#include "stdafx.h"
#include "jvmrun.h"
#include "JReturnValue.h"
#include "lconvert.h"
#include "larray.h"
#include "lReflection.h"

using namespace System;
using namespace System::Reflection;
using namespace NXDO::JBridge;

JReturnValue::JReturnValue(IntPtr ptr, Type^ returnType, Type^ jobjectType)
{
	this->_resultPtr = ptr;
	this->_isArray = returnType->IsArray;
	this->_returnType = !returnType->IsArray ? returnType : returnType->GetElementType();
	this->_jobjectType = jobjectType;
	this->_isSubOf = returnType->IsSubclassOf(jobjectType);
	this->_isRootOf = returnType == jobjectType;
}

Object^ JReturnValue:: CastNValue()
{
	jobject jr = (jobject)this->ReturnPtr.ToPointer();
	return (!this->IsArray) ? 
		 this->ToValue(jr) : 
		 this->ToArray(jr);
}


Object^ JReturnValue::ToValue(jobject jr)
{
	if(jr == NULL) return nullptr;

	Type^ type = this->ReturnType;
	if(this->IsJObject){
		//java返回值类型为object,表示c#方法返回类型为 JObject
		String^ str = lReflection::GetClassName(jr);
		IJReturn^ ijr = (IJReturn^)this;
		ijr->ReturnJObjectOfJClassName = str;

		type = this->GetTypeFromClass(str);

		//不是基本类型时，直接返回指针
		if(type == nullptr)
			return IntPtr(jr);
	}

	return lconvert::convertToDotnetObject(jr, type, this->IsSubJObject);
	
	//if(type == Boolean::typeid || type == Nullable<Boolean>::typeid)
	//	return lconvert::j2boolean(jr);
	//if(type == Byte::typeid || type == Nullable<Byte>::typeid)
	//	return lconvert::j2byte(jr);
	//if(type == Char::typeid || type == Nullable<Char>::typeid)
	//	return gcnew Char(lconvert::j2char(jr));

	//if(type == Int16::typeid || type == Nullable<Int16>::typeid)
	//	return lconvert::j2short(jr);
	//if(type == Int32::typeid || type == Nullable<Int32>::typeid)
	//	return lconvert::j2int(jr);
	//if(type == Int64::typeid || type == Nullable<Int64>::typeid)
	//	return gcnew Int64(lconvert::j2long(jr));

	//if(type == Single::typeid || type == Nullable<Single>::typeid)
	//	return lconvert::j2float(jr);
	//if(type == Double::typeid || type == Nullable<Double>::typeid)
	//	return lconvert::j2double(jr);
	//
	//if(type == String::typeid)
	//	return lconvert::toNString((jstring)jr);

	//return nullptr;
}

Array^ JReturnValue::ToArray(jobject jr){
	if(jr == NULL) return nullptr;

	Type^ type = this->ReturnType;
	if(this->IsSubJObject){
		return larray::toNSubofJObjectArray(jr, type);
	}

	if(this->IsJObject){
		String^ str = lReflection::GetClassName(jr);
		IJReturn^ ijr = (IJReturn^)this;
		ijr->ReturnJObjectOfJClassName = str;

		type = this->GetTypeFromClass(str);
		if(type == nullptr)
			return larray::toNBoxPtrArray(jr);
	}

	if(type == Boolean::typeid || type == Nullable<Boolean>::typeid)
		return larray::toNBoolArray(jr, type == Nullable<Boolean>::typeid);
	if(type == Byte::typeid || type == Nullable<Byte>::typeid)
		return larray::toNByteArray(jr, type == Nullable<Byte>::typeid);
	if(type == Char::typeid || type == Nullable<Char>::typeid)
		return larray::toNCharArray(jr, type == Nullable<Char>::typeid);

	if(type == Int16::typeid || type == Nullable<Int16>::typeid)
		return larray::toNShortArray(jr, type == Nullable<Int16>::typeid);
	if(type == Int32::typeid || type == Nullable<Int32>::typeid)
		return larray::toNIntArray(jr, type == Nullable<Int32>::typeid);
	if(type == Int64::typeid || type == Nullable<Int64>::typeid)
		return larray::toNLongArray(jr, type == Nullable<Int64>::typeid);

	if(type == Single::typeid || type == Nullable<Single>::typeid)
		return larray::toNFloatArray(jr, type == Nullable<Single>::typeid);
	if(type == Double::typeid || type == Nullable<Double>::typeid)
		return larray::toNDoubleArray(jr, type == Nullable<Double>::typeid);

	if(type == String::typeid)
		return larray::toNStringArray(jr);
	return nullptr;
}


Type^ JReturnValue::GetTypeFromClass(String^ javaClassName)
{
	if(javaClassName == "java.lang.Boolean" || javaClassName == "[Ljava.lang.Boolean;")
		return Boolean::typeid;
	if(javaClassName == "java.lang.Byte" || javaClassName == "[Ljava.lang.Byte;")
		return Byte::typeid;
	if(javaClassName == "java.lang.Character" || javaClassName == "[Ljava.lang.Character;")
		return Char::typeid;
	if(javaClassName == "java.lang.Short" || javaClassName == "[Ljava.lang.Short;")
		return Int16::typeid;
	if(javaClassName == "java.lang.Integer" || javaClassName == "[Ljava.lang.Integer;")
		return INT32::typeid;
	if(javaClassName == "java.lang.Long" || javaClassName == "[Ljava.lang.Long;")
		return INT64::typeid;
	if(javaClassName == "java.lang.Float" || javaClassName == "[Ljava.lang.Float;")
		return Single::typeid;
	if(javaClassName == "java.lang.Double" || javaClassName == "[Ljava.lang.Double;")
		return Double::typeid;
	if(javaClassName == "java.lang.String" || javaClassName == "[Ljava.lang.String;")
		return String::typeid;
	return nullptr;
}
