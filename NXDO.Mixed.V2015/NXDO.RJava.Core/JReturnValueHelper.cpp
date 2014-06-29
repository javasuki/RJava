#include "jni.h"
#include "stdafx.h"
#include "JReturnValueHelper.h"
#include "lconvert.h"
#include "larray.h"
#include "ExtOper.h"

using namespace System;
using namespace System::Reflection;
using namespace System::Collections::Generic;
using namespace NXDO::RJava;

generic <typename T>
T JReturnValueHelper::GetObjectValue(IntPtr ptr, bool isSubJObject)
{
	if(ptr == IntPtr::Zero)return (T)(Object^)nullptr;
	jobject jr = (jobject)ptr.ToPointer();
	Object^ o = lconvert::convertToDotnetObject(jr, T::typeid, isSubJObject);
	//Object^ o = nullptr;
	return (T)o;
}

generic <typename T>
T JReturnValueHelper::GetObjectArray(IntPtr ptr, bool isSubJObject)
{
	Type^ aryElemType = T::typeid->GetElementType();	

	//true: java基本类型对应的对象,C# Nullable<基本类型>, string/JObject继承类
	bool isPrimitiveObject = aryElemType->FullName->StartsWith("System.Nullable");	
	if(JReturnValueHelper::toArrayMethod == nullptr)
		JReturnValueHelper::toArrayMethod = (System::Linq::Enumerable::typeid)->GetMethod("ToArray");

	System::Collections::IList^ gList = nullptr;	
	jobject ary = (jobject)ptr.ToPointer();
	jobjectArray* jobjAry = NULL;
	jobjAry = reinterpret_cast<jobjectArray*>(&ary);

	#pragma region 根据类型转换数组并创建IList
	if(aryElemType == Boolean::typeid || aryElemType == Nullable<Boolean>::typeid){
		//jobjAry = (jobjectArray*)(reinterpret_cast<jbooleanArray*>(&ary));
		gList = isPrimitiveObject ? (System::Collections::IList^)gcnew List<Nullable<Boolean>>() :
									(System::Collections::IList^) gcnew List<Boolean>();
	}
	else if(aryElemType == Byte::typeid || aryElemType == Nullable<Byte>::typeid){
		//jobjAry = (jobjectArray*)(reinterpret_cast<jbyteArray*>(&ary));
		gList = isPrimitiveObject ? (System::Collections::IList^)gcnew List<Nullable<Byte>>() :
									(System::Collections::IList^) gcnew List<Byte>();
	}
	else if(aryElemType == Char::typeid || aryElemType == Nullable<Char>::typeid){
		//jobjAry = (jobjectArray*)(reinterpret_cast<jcharArray*>(&ary));
		gList = isPrimitiveObject ? (System::Collections::IList^)gcnew List<Nullable<Char>>() :
									(System::Collections::IList^) gcnew List<Char>();
	}
	else if(aryElemType == Int16::typeid || aryElemType == Nullable<Int16>::typeid ){
		//jobjAry = (jobjectArray*)(reinterpret_cast<jshortArray*>(&ary));
		gList = isPrimitiveObject ? (System::Collections::IList^)gcnew List<Nullable<Int16>>() :
									(System::Collections::IList^) gcnew List<Int16>();
	}
	else if(aryElemType == Int32::typeid || aryElemType == Nullable<Int32>::typeid){
		//jobjAry = (jobjectArray*)(reinterpret_cast<jintArray*>(&ary));
		gList = isPrimitiveObject ? (System::Collections::IList^)gcnew List<Nullable<Int32>>() :
									(System::Collections::IList^) gcnew List<Int32>();
	}
	else if(aryElemType == Int64::typeid || aryElemType == Nullable<Int64>::typeid){
		//jobjAry = (jobjectArray*)(reinterpret_cast<jlongArray*>(&ary));
		gList = isPrimitiveObject ? (System::Collections::IList^)gcnew List<Nullable<Int64>>() :
									(System::Collections::IList^) gcnew List<Int64>();
	}
	else if(aryElemType == Single::typeid || aryElemType == Nullable<Single>::typeid){
		//jobjAry = (jobjectArray*)(reinterpret_cast<jfloatArray*>(&ary));
		gList = isPrimitiveObject ? (System::Collections::IList^)gcnew List<Nullable<Single>>() :
									(System::Collections::IList^) gcnew List<Single>();
	}
	else if(aryElemType == Double::typeid || aryElemType == Nullable<Double>::typeid)
	{
		//jobjAry = (jobjectArray*)(reinterpret_cast<jdoubleArray*>(&ary));
		gList = isPrimitiveObject ? (System::Collections::IList^)gcnew List<Nullable<Double>>() :
									(System::Collections::IList^) gcnew List<Double>();
	}
	else if(aryElemType == String::typeid || 
		aryElemType == DateTime::typeid || aryElemType == Nullable<DateTime>::typeid || 
		aryElemType == Decimal::typeid || aryElemType == Nullable<Decimal>::typeid || 
		aryElemType->IsEnum || isSubJObject)
	{
		jobjAry = reinterpret_cast<jobjectArray*>(&ary);
		isPrimitiveObject = true; 
		if(aryElemType == String::typeid)
			gList = gcnew List<String^>();
		else{
			//IsEnum, isSubJObject 通用
			Object^ oList = Activator::CreateInstance(List<int>::typeid->GetGenericTypeDefinition()->MakeGenericType(aryElemType));
			gList = (System::Collections::IList^)oList;
		}
	}
	#pragma endregion

	//将数组值,添加到 gList 中
	JReturnValueHelper::addArrayElemToList(*jobjAry, gList, aryElemType, isPrimitiveObject, isSubJObject);
	
	//gList.ToArray()的转换
	array<Object^>^ lstValues = { gList };
	return (T)(JReturnValueHelper::toArrayMethod->MakeGenericMethod(aryElemType)->Invoke(nullptr, lstValues));
}

void JReturnValueHelper::addArrayElemToList(jobjectArray ary, System::Collections::IList^ gList, Type^ aryElemType,bool isPrimitiveObject,  bool isSubJObject){

		int size = jvmrun::getEnv()->GetArrayLength(ary);
		int flag = -1; //0:boolean,1:byte,2:char,3:short,4:int,5:long,6:float,7:double
		void* voidAry = NULL;

		#pragma region 获取基本类型的数组指针便于使用索引获取基本类型的值
		if(aryElemType == Boolean::typeid){
			voidAry = (jobject*)jvmrun::getEnv()->GetBooleanArrayElements((jbooleanArray)ary, NULL);
			flag=0;
		}
		else if(aryElemType == Byte::typeid){
			voidAry = (jobject*)jvmrun::getEnv()->GetByteArrayElements((jbyteArray)ary, NULL);
			flag=1;
		}
		else if(aryElemType == Char::typeid){
			voidAry = (jobject*)jvmrun::getEnv()->GetCharArrayElements((jcharArray)ary, NULL);
			flag=2;
		}
		else if(aryElemType == Int16::typeid){
			voidAry = (jobject*)jvmrun::getEnv()->GetShortArrayElements((jshortArray)ary, NULL);
			flag=3;
		}
		else if(aryElemType == Int32::typeid){
			voidAry = (jobject*)jvmrun::getEnv()->GetIntArrayElements((jintArray)ary, NULL);
			flag=4;
		}
		else if(aryElemType == Int64::typeid){
			voidAry = (jobject*)jvmrun::getEnv()->GetLongArrayElements((jlongArray)ary, NULL);
			flag=5;
		}
		else if(aryElemType == Single::typeid){
			voidAry = (jobject*)jvmrun::getEnv()->GetFloatArrayElements((jfloatArray)ary, NULL);
			flag=6;
		}
		else if(aryElemType == Double::typeid){
			voidAry = (jobject*)jvmrun::getEnv()->GetDoubleArrayElements((jdoubleArray)ary, NULL);
			flag=7;
		}
		#pragma endregion

		
		for (int i = 0; i < size; i++) {
			if(isPrimitiveObject){
				//基本类型对应的对象, string/JObject子类
				jobject data = jvmrun::getEnv()->GetObjectArrayElement(ary, i);
				gList->Add(lconvert::convertToDotnetObject(data, aryElemType, isSubJObject));
			}
			else
			{
				#pragma region 基本类型
				if(flag==0)
					gList->Add( ((jboolean*)voidAry)[i] == JNI_TRUE ? true : false);
				else if(flag==1)
					gList->Add( gcnew Byte(((jbyte*)voidAry)[i]));
				else if(flag==2)
					gList->Add( gcnew Char(((jchar*)voidAry)[i]));
				else if(flag==3)
					gList->Add( ((jshort*)voidAry)[i]);
				else if(flag==4)
					gList->Add( ((jint*)voidAry)[i]);
				else if(flag==5)
					gList->Add( ((jlong*)voidAry)[i]);
				else if(flag==6)
					gList->Add( ((jfloat*)voidAry)[i]);
				else if(flag==7)
					gList->Add( ((jdouble*)voidAry)[i]);
				#pragma endregion
			}
		}
		if(flag>-1)
			jvmrun::getEnv()->ReleasePrimitiveArrayCritical(ary, voidAry, JNI_FALSE); //jvmrun::getEnv()->ReleaseDoubleArrayElements(arr, data, JNI_FALSE); 		
		jvmrun::getEnv()->DeleteLocalRef(ary);
}

int JReturnValueHelper::GetArraySize(IntPtr ptr)
{
	jobject ary = (jobject)ptr.ToPointer();
	jobjectArray* jobjAry = (jobjectArray*)(reinterpret_cast<jbooleanArray*>(&ary));
	int size = jvmrun::getEnv()->GetArrayLength(*jobjAry);
	return size;
}

IntPtr JReturnValueHelper::GetArrayElem(IntPtr ptr, long index){
	if(ptr == IntPtr::Zero)return ptr;
	jobject ary = (jobject)ptr.ToPointer();
	jobjectArray* jobjAry = reinterpret_cast<jobjectArray*>(&ary);

	jobject data = jvmrun::getEnv()->GetObjectArrayElement(*jobjAry, index);
	return IntPtr(data);
}

String^ JReturnValueHelper::GetClassName(IntPtr ptr){
	if(ptr == IntPtr::Zero)
		return String::Empty;
	jobject jr = (jobject)ptr.ToPointer();
	String^ str = lRunCore::GetClassName(jr);
	return str;
}

//String^ JReturnValueHelper::GetDateTime(IntPtr ptr){
//	if(ptr == IntPtr::Zero)
//		return String::Empty;
//	jobject jr = (jobject)ptr.ToPointer();
//	String^ str = lReflection::GetDateString(jr);
//	return str;
//}

bool JReturnValueHelper::CheckIsPrimitive(IntPtr ptr){
	if(ptr == IntPtr::Zero)
		return false;
	jobject jr = (jobject)ptr.ToPointer();
	return lRunCore::CheckIsPrimitive(jr);
}



//array<Object^>^ JReturnValueHelper::GetJObjectBoxArray(IntPtr ptr)
//{
//	array<System::Object^> ary = {};
//	return ary;
//}