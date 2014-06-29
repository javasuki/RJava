#include "stdafx.h"
#include "larray.h"
#include "jvmrun.h"
#include "lconvert.h"

using namespace System::Collections::Generic;
using namespace System::Reflection;

larray::larray(void)
{
}


larray::~larray(void)
{
}

jobjectArray larray::toJBoolArray(System::Array^ ary)
{
	System::Type^ elemType = ary->GetType()->GetElementType();
	bool isNullable = elemType == System::Nullable<System::Boolean>::typeid;
	int size = ary == nullptr ? 0 : ary->Length;

	JNIEnv* env = jvmrun::getEnv();
	jobjectArray jobjAry = isNullable ? createObjectArray(size, "java/lang/Boolean") :  (jobjectArray)env->NewBooleanArray(size);
	if(size ==0)return jobjAry;
	for(int i=0; i<size; i++)
	{
		System::Object^ oElemValue = ary->GetValue(i);
		jobject jElemValue = NULL;
		if(oElemValue != nullptr)
		{
			if(isNullable)
				jElemValue = lconvert::boolean2J((bool)oElemValue);
			else 
			{
				jboolean bval = (bool)oElemValue;
				jvmrun::getEnv()->SetBooleanArrayRegion((jbooleanArray)jobjAry, i, 1, &bval);
				continue;
			}
		}
		env->SetObjectArrayElement(jobjAry,i, jElemValue);
	}
	return jobjAry;
}

jobjectArray larray::toJByteArray(System::Array^ ary)
{
	System::Type^ elemType = ary->GetType()->GetElementType();
	bool isNullable = elemType == System::Nullable<System::Byte>::typeid;
	int size = ary == nullptr ? 0 : ary->Length;

	JNIEnv* env = jvmrun::getEnv();
	jobjectArray jobjAry = isNullable ? createObjectArray(size, "java/lang/Byte") :  (jobjectArray)env->NewByteArray(size);
	if(size ==0)return jobjAry;
	for(int i=0; i<size; i++)
	{
		System::Object^ oElemValue = ary->GetValue(i);
		jobject jElemValue = NULL;
		if(oElemValue != nullptr)
		{
			if(isNullable)
				jElemValue = lconvert::byte2J((byte)oElemValue);
			else 
			{
				jbyte bval = (byte)oElemValue;
				jvmrun::getEnv()->SetByteArrayRegion((jbyteArray)jobjAry, i, 1, &bval);
				continue;
			}
		}
		env->SetObjectArrayElement(jobjAry,i, jElemValue);
	}
	return jobjAry;
}

jobjectArray larray::toJCharArray(System::Array^ ary){
	System::Type^ elemType = ary->GetType()->GetElementType();
	bool isNullable = elemType == System::Nullable<System::Char>::typeid;
	int size = ary == nullptr ? 0 : ary->Length;

	JNIEnv* env = jvmrun::getEnv();
	jobjectArray jobjAry = isNullable ? createObjectArray(size, "java/lang/Character") :  (jobjectArray)env->NewCharArray(size);
	if(size ==0)return jobjAry;
	for(int i=0; i<size; i++)
	{
		System::Object^ oElemValue = ary->GetValue(i);
		jobject jElemValue = NULL;

		if(oElemValue != nullptr)
		{
			if(isNullable)
				jElemValue = lconvert::char2J((System::Char)oElemValue);
			else 
			{
				jchar cval = System::Convert::ToChar(oElemValue);
				jvmrun::getEnv()->SetCharArrayRegion((jcharArray)jobjAry, i, 1, &cval);
				continue;
			}
		}
		env->SetObjectArrayElement(jobjAry,i, jElemValue);
	}
	return jobjAry;
}

jobjectArray larray::toJShortArray(System::Array^ ary){
	System::Type^ elemType = ary->GetType()->GetElementType();
	bool isNullable = elemType == System::Nullable<System::Int16>::typeid;
	int size = ary == nullptr ? 0 : ary->Length;

	JNIEnv* env = jvmrun::getEnv();
	jobjectArray jobjAry = isNullable ? createObjectArray(size, "java/lang/Short") :  (jobjectArray)env->NewShortArray(size);
	if(size ==0)return jobjAry;
	for(int i=0; i<size; i++)
	{
		System::Object^ oElemValue = ary->GetValue(i);
		jobject jElemValue = NULL;

		if(oElemValue != nullptr)
		{
			if(isNullable)
				jElemValue = lconvert::short2J((System::Int16)oElemValue);
			else 
			{
				jshort sval = (System::Int16)oElemValue;
				jvmrun::getEnv()->SetShortArrayRegion((jshortArray)jobjAry, i, 1, &sval);
				continue;
			}
		}
		env->SetObjectArrayElement(jobjAry,i, jElemValue);
	}
	return jobjAry;
}

jobjectArray larray::toJIntArray(System::Array^ ary){
	System::Type^ elemType = ary->GetType()->GetElementType();
	bool isNullable = elemType == System::Nullable<System::Int32>::typeid;
	int size = ary == nullptr ? 0 : ary->Length;

	JNIEnv* env = jvmrun::getEnv();
	jobjectArray jobjAry = isNullable ? createObjectArray(size, "java/lang/Integer") :  (jobjectArray)env->NewIntArray(size);
	if(size ==0)return jobjAry;

	for(int i=0; i<size; i++)
	{
		System::Object^ oElemValue = ary->GetValue(i);
		jobject jElemValue = NULL;

		if(oElemValue != nullptr)
		{
			if(isNullable)
				jElemValue = lconvert::integer2J((long)oElemValue);
			else
			{
				jint sval = (long)oElemValue;
				jvmrun::getEnv()->SetIntArrayRegion((jintArray)jobjAry, i, 1, &sval);
				continue;
			}
		}
		env->SetObjectArrayElement(jobjAry,i, jElemValue);
	}
	return jobjAry;
}

jobjectArray larray::toJLongArray(System::Array^ ary)
{
	System::Type^ elemType = ary->GetType()->GetElementType();
	bool isNullable = elemType == System::Nullable<System::Int64>::typeid;
	int size = ary == nullptr ? 0 : ary->Length;

	JNIEnv* env = jvmrun::getEnv();
	jobjectArray jobjAry = isNullable ? createObjectArray(size, "java/lang/Long") :  (jobjectArray)env->NewLongArray(size);
	if(size ==0)return jobjAry;
	for(int i=0; i<size; i++)
	{
		System::Object^ oElemValue = ary->GetValue(i);
		jobject jElemValue = NULL;

		if(oElemValue != nullptr)
		{
			if(isNullable)
				jElemValue = lconvert::long2J(System::Convert::ToInt64(oElemValue));
			else
			{
				jlong lval = System::Convert::ToInt64(oElemValue);
				jvmrun::getEnv()->SetLongArrayRegion((jlongArray)jobjAry, i, 1, &lval);
				continue;
			}
		}
		env->SetObjectArrayElement(jobjAry,i, jElemValue);
	}
	return jobjAry;
}

jobjectArray larray::toJFloatArray(System::Array^ ary)
{
	System::Type^ elemType = ary->GetType()->GetElementType();
	bool isNullable = elemType == System::Nullable<System::Single>::typeid;
	int size = ary == nullptr ? 0 : ary->Length;

	JNIEnv* env = jvmrun::getEnv();
	jobjectArray jobjAry = isNullable ? createObjectArray(size, "java/lang/Float") :  (jobjectArray)env->NewFloatArray(size);
	if(size ==0)return jobjAry;
	for(int i=0; i<size; i++)
	{
		System::Object^ oElemValue = ary->GetValue(i);
		jobject jElemValue = NULL;

		if(oElemValue != nullptr)
		{
			if(isNullable)
				jElemValue = lconvert::float2J((float)oElemValue);
			else
			{
				jfloat fval = (float)oElemValue;
				jvmrun::getEnv()->SetFloatArrayRegion((jfloatArray)jobjAry, i, 1, &fval);
				continue;
			}
		}
		env->SetObjectArrayElement(jobjAry,i, jElemValue);
	}
	return jobjAry;
}

jobjectArray larray::toJDoubleArray(System::Array^ ary){
	System::Type^ elemType = ary->GetType()->GetElementType();
	bool isNullable = elemType == System::Nullable<System::Double>::typeid;
	int size = ary == nullptr ? 0 : ary->Length;

	JNIEnv* env = jvmrun::getEnv();
	jobjectArray jobjAry = isNullable ? createObjectArray(size, "java/lang/Double") :  (jobjectArray)env->NewDoubleArray(size);
	if(size ==0)return jobjAry;
	for(int i=0; i<size; i++)
	{
		System::Object^ oElemValue = ary->GetValue(i);
		jobject jElemValue = NULL;

		if(oElemValue != nullptr)
		{
			if(isNullable)
				jElemValue = lconvert::double2J((double)oElemValue);
			else 
			{
				jdouble dval = (double)oElemValue;
				jvmrun::getEnv()->SetDoubleArrayRegion((jdoubleArray)jobjAry, i, 1, &dval);
				continue;
			}
		}
		env->SetObjectArrayElement(jobjAry,i, jElemValue);
	}
	return jobjAry;
}

jobjectArray larray::toJStringArray(System::Array^ ary){
	JNIEnv* env = jvmrun::getEnv();
	int size = ary == nullptr ? 0 : ary->Length;
	jobjectArray jobjAry = createObjectArray(size, "java/lang/String");
	if(size ==0)return jobjAry;

	for(int i=0; i<size; i++)
	{
		System::Object^ oElemValue = ary->GetValue(i);
		jobject jElemValue = NULL;

		if(oElemValue != nullptr)
			jElemValue = lconvert::toJString((System::String^)oElemValue);
		env->SetObjectArrayElement(jobjAry,i, jElemValue);
	}
	return jobjAry;
}


//jobjectArray larray::toJObjWrapperArray(System::Array^ ary, System::String^ jClassName)
//{
//	JNIEnv* env = jvmrun::getEnv();
//	int size = ary == nullptr ? 0 : ary->Length;
//
//	jobjectArray jobjAry = createObjectArray(size, "java/lang/Object");
//}

//jobjectArray larray::toJObjectArray(System::Array^ ary){
//	return larray::toJObjectArray(ary, "java/lang/Object");
//}

jobjectArray larray::toJavaObjectArray(System::Array^ ary, jclass aryElemClass)
{
	JNIEnv* env = jvmrun::getEnv();
	int size = ary == nullptr ? 0 : ary->Length;
	jobjectArray jobjAry = env->NewObjectArray(size, aryElemClass, 0);	

	if(size ==0)return jobjAry;
	for(int i=0; i<size; i++)
	{
		System::Object^ oElemValue = ary->GetValue(i);
		jobject jElemValue = NULL;

		if(oElemValue != nullptr)
		{
			System::IntPtr ptr = (System::IntPtr)oElemValue;
			if(ptr != IntPtr::Zero)
				jElemValue = (jobject)ptr.ToPointer();
		}
		env->SetObjectArrayElement(jobjAry,i, jElemValue);
	}
	return jobjAry;
}

//private
jobjectArray larray::createObjectArray(int size, System::String^ jniClassName){
	JNIEnv* env = jvmrun::getEnv();

	char* cClassName = lconvert::toCString(jniClassName);
	jclass aryElemClass = env->FindClass(cClassName);
	lconvert::freeCString(cClassName);

	return env->NewObjectArray(size, aryElemClass, 0);	
}

array<jobjectArray>^ larray::toJavaMethodArray(array<IParamValue^>^ ary){
	JNIEnv* env = jvmrun::getEnv();
	int size = ary == nullptr ? 0 : ary->Length;

	//invokeObjectMethodValue(Object obj,String methodName,Class<?>[] classes,Object[] Values)

	jobjectArray jTypeAry = createObjectArray(size, "java/lang/Class");
	jobjectArray jValueAry = createObjectArray(size, "java/lang/Object");
	array<jobjectArray>^ result = {jTypeAry, jValueAry };
	if(size ==0)return result;

	for(int i=0;i<size;i++)
	{
		IParamValue^ jcv = ary[i];
		jclass jClass = (jclass)(jcv->JClass.ToPointer());
		jobject jValue = (jobject)(jcv->JValue.ToPointer());
		env->SetObjectArrayElement(jTypeAry,i, jClass);
		env->SetObjectArrayElement(jValueAry,i, jValue);
	}
	return result;
}



System::Array^ larray::toNBoolArray(jobject ary, bool elemIsObject)
{
	if(!elemIsObject)
	{
		jbooleanArray* arr = reinterpret_cast<jbooleanArray*>(&ary);
		int size = jvmrun::getEnv()->GetArrayLength(*arr);
		array<System::Boolean,1>^ ndatas = gcnew array<System::Boolean,1>(size);

		jboolean * data = jvmrun::getEnv()->GetBooleanArrayElements(*arr, NULL);
		for (int i = 0; i < size; i++) {
			ndatas[i] = data[i] == 0 ? false : true;
		}
		jvmrun::getEnv()->ReleaseBooleanArrayElements(*arr, data, JNI_FALSE); 
		jvmrun::getEnv()->DeleteLocalRef(ary);
		return ndatas;
	}
	else{
		List<Nullable<Boolean>>^ gList = gcnew List<Nullable<Boolean>>();
		addRefObjectToList(ary, gList, System::Boolean::typeid);
		return System::Linq::Enumerable::ToArray(gList);
	}
}

System::Array^ larray::toNByteArray(jobject ary, bool elemIsObject)
{
	if(!elemIsObject)
	{
		jbyteArray* arr = reinterpret_cast<jbyteArray*>(&ary);
		int size = jvmrun::getEnv()->GetArrayLength(*arr);
		array<System::Byte,1>^ ndatas = gcnew array<System::Byte,1>(size);

		jbyte * data = jvmrun::getEnv()->GetByteArrayElements(*arr, NULL);
		for (int i = 0; i < size; i++) {
			ndatas[i] = data[i];
		}
		jvmrun::getEnv()->ReleaseByteArrayElements(*arr, data, JNI_FALSE); 
		jvmrun::getEnv()->DeleteLocalRef(ary);
		return ndatas;
	}
	else{
		List<Nullable<Byte>>^ gList = gcnew List<Nullable<Byte>>();
		addRefObjectToList(ary, gList, System::Byte::typeid);
		return System::Linq::Enumerable::ToArray(gList);
	}
}

System::Array^ larray::toNCharArray(jobject ary, bool elemIsObject)
{
	if(!elemIsObject)
	{
		jcharArray* arr = reinterpret_cast<jcharArray*>(&ary);
		int size = jvmrun::getEnv()->GetArrayLength(*arr);
		array<System::Char,1>^ ndatas = gcnew array<System::Char,1>(size);

		jchar * data = jvmrun::getEnv()->GetCharArrayElements(*arr, NULL);
		for (int i = 0; i < size; i++) {
			ndatas[i] = data[i];
		}
		jvmrun::getEnv()->ReleaseCharArrayElements(*arr, data, JNI_FALSE); 
		jvmrun::getEnv()->DeleteLocalRef(ary);
		return ndatas;
	}
	else{
		List<Nullable<Char>>^ gList = gcnew List<Nullable<Char>>();
		addRefObjectToList(ary, gList, System::Char::typeid);
		return System::Linq::Enumerable::ToArray(gList);
	}
}

System::Array^ larray::toNShortArray(jobject ary, bool elemIsObject)
{
	if(!elemIsObject)
	{
		jshortArray* arr = reinterpret_cast<jshortArray*>(&ary);
		int size = jvmrun::getEnv()->GetArrayLength(*arr);
		array<System::Int16,1>^ ndatas = gcnew array<System::Int16,1>(size);

		jshort * data = jvmrun::getEnv()->GetShortArrayElements(*arr, NULL);
		for (int i = 0; i < size; i++) {
			ndatas[i] = data[i];
		}
		jvmrun::getEnv()->ReleaseShortArrayElements(*arr, data, JNI_FALSE); 
		jvmrun::getEnv()->DeleteLocalRef(ary);
		return ndatas;
	}
	else{
		List<Nullable<Int16>>^ gList = gcnew List<Nullable<Int16>>();
		addRefObjectToList(ary, gList, System::Int16::typeid);
		return System::Linq::Enumerable::ToArray(gList);
	}
}

System::Array^ larray::toNIntArray(jobject ary, bool elemIsObject){
	if(!elemIsObject)
	{
		jintArray* arr = reinterpret_cast<jintArray*>(&ary);
		int size = jvmrun::getEnv()->GetArrayLength(*arr);
		array<System::Int32,1>^ ndatas = gcnew array<System::Int32,1>(size);

		jint * data = jvmrun::getEnv()->GetIntArrayElements(*arr, NULL);
		for (int i = 0; i < size; i++) {
			ndatas[i] = data[i];
		}
		jvmrun::getEnv()->ReleaseIntArrayElements(*arr, data, 0); 
		jvmrun::getEnv()->DeleteLocalRef(ary);
		return ndatas;
	}
	else{
		List<Nullable<Int32>>^ gList = gcnew List<Nullable<Int32>>();
		addRefObjectToList(ary, gList, System::Int32::typeid);
		return System::Linq::Enumerable::ToArray(gList);
	}
}
System::Array^ larray::toNLongArray(jobject ary, bool elemIsObject){
	if(!elemIsObject)
	{
		jlongArray* arr = reinterpret_cast<jlongArray*>(&ary);
		int size = jvmrun::getEnv()->GetArrayLength(*arr);
		array<System::Int64,1>^ ndatas = gcnew array<System::Int64,1>(size);

		jlong * data = jvmrun::getEnv()->GetLongArrayElements(*arr, NULL);
		for (int i = 0; i < size; i++) {
			ndatas[i] = data[i];
		}
		jvmrun::getEnv()->ReleaseLongArrayElements(*arr, data, JNI_FALSE); 
		jvmrun::getEnv()->DeleteLocalRef(ary);
		return ndatas;
	}
	else{
		List<Nullable<Int64>>^ gList = gcnew List<Nullable<Int64>>();
		addRefObjectToList(ary, gList, System::Int64::typeid);
		return System::Linq::Enumerable::ToArray(gList);
	}
}
System::Array^ larray::toNFloatArray(jobject ary, bool elemIsObject){
	if(!elemIsObject)
	{
		jfloatArray* arr = reinterpret_cast<jfloatArray*>(&ary);
		int size = jvmrun::getEnv()->GetArrayLength(*arr);
		array<System::Single,1>^ ndatas = gcnew array<System::Single,1>(size);

		float * data = jvmrun::getEnv()->GetFloatArrayElements(*arr, NULL);
		for (int i = 0; i < size; i++) {
			ndatas[i] = data[i];
		}
		jvmrun::getEnv()->ReleaseFloatArrayElements(*arr, data, JNI_FALSE); 
		jvmrun::getEnv()->DeleteLocalRef(ary);
		return ndatas;
	}
	else{
		List<Nullable<Single>>^ gList = gcnew List<Nullable<Single>>();
		addRefObjectToList(ary, gList, System::Single::typeid);
		return System::Linq::Enumerable::ToArray(gList);
	}
}
System::Array^ larray::toNDoubleArray(jobject ary, bool elemIsObject){
	if(!elemIsObject)
	{
		jdoubleArray* arr = reinterpret_cast<jdoubleArray*>(&ary);
		int size = jvmrun::getEnv()->GetArrayLength(*arr);
		array<System::Double,1>^ ndatas = gcnew array<System::Double,1>(size);

		double * data = jvmrun::getEnv()->GetDoubleArrayElements(*arr, NULL);
		for (int i = 0; i < size; i++) {
			ndatas[i] = data[i];
		}
		jvmrun::getEnv()->ReleaseDoubleArrayElements(*arr, data, JNI_FALSE); 
		jvmrun::getEnv()->DeleteLocalRef(ary);
		return ndatas;
	}
	else{
		List<Nullable<Double>>^ gList = gcnew List<Nullable<Double>>();
		addRefObjectToList(ary, gList, System::Double::typeid);
		return System::Linq::Enumerable::ToArray(gList);
	}
}
System::Array^ larray::toNStringArray(jobject ary){
	List<String^>^ gList = gcnew List<String^>();
	addRefObjectToList(ary, gList, System::String::typeid);
	return System::Linq::Enumerable::ToArray(gList);
}

System::Array^ larray::toNSubofJObjectArray(jobject ary, Type^ JObjType)
{
	JNIEnv* jenv = jvmrun::getEnv();
	jobjectArray* arr = reinterpret_cast<jobjectArray*>(&ary);
	int size = jenv->GetArrayLength(*arr);
	
	Array^ jNObjArray = Array::CreateInstance( JObjType, size );
	for (int i = 0; i < size; i++) {
		jobject data = jenv->GetObjectArrayElement(*arr, i);
		if(data == NULL)continue;
		Object^ v = lconvert::j2NSubofJObject(data, JObjType);
		jNObjArray->SetValue(v, i);
	}
	return jNObjArray;

	//获取泛型类型
	//Object^ objList = Activator::CreateInstance( List<Int32>::typeid->GetGenericTypeDefinition()->MakeGenericType( JObjType));
	//System::Collections::IList^ gList = (System::Collections::IList^)objList;
	//addRefObjectToList(ary, gList, JObjType);
	//return nullptr; //System::Linq::Enumerable::ToArray(gList);
}

System::Array^ larray::toNBoxPtrArray(jobject ary){
	JNIEnv* jenv = jvmrun::getEnv();
	jobjectArray* arr = reinterpret_cast<jobjectArray*>(&ary);
	int size = jenv->GetArrayLength(*arr);
	Array^ jNObjArray = Array::CreateInstance(IntPtr::typeid , size );

	for (int i = 0; i < size; i++) {
		jobject data = jenv->GetObjectArrayElement(*arr, i);
		if(data == NULL)
			jNObjArray->SetValue(IntPtr::Zero, i);
		else
			jNObjArray->SetValue(IntPtr(data), i);
	}
	return jNObjArray;
}

void larray::addRefObjectToList(jobject ary, System::Collections::IList^ gList, Type^ type)
{
	JNIEnv* jenv = jvmrun::getEnv();
	jobjectArray* arr = reinterpret_cast<jobjectArray*>(&ary);
	int size = jenv->GetArrayLength(*arr);
	for (int i = 0; i < size; i++) {

		jobject data = jenv->GetObjectArrayElement(*arr, i);
		Object^ v = nullptr;

		if(data == NULL){
			gList->Add(v);
			continue;
		}

		
		if(type == Boolean::typeid)
			v = lconvert::j2boolean(data);
		else if(type == Byte::typeid)
			v = lconvert::j2byte(data);
		else if(type == Char::typeid)
			v = gcnew Char(lconvert::j2char(data));

		else if(type == Int16::typeid)
			v = lconvert::j2short(data);
		else if(type == Int32::typeid)
			v = lconvert::j2int(data);
		else if(type == Int64::typeid)
			v = gcnew Int64(lconvert::j2long(data));

		else if(type == Single::typeid)
			v = lconvert::j2float(data);
		else if(type == Double::typeid)
			v = lconvert::j2double(data);
		else if(type == String::typeid)
			v= lconvert::toNString((jstring)data);
		/*else if(type->Name->StartsWith("Demo"))		
			v = lconvert::j2NJObject(data, type);*/
		

		if(data!= NULL)
			jenv->DeleteLocalRef(data);
		gList->Add(v);
	}
	jvmrun::getEnv()->DeleteLocalRef(ary);
}


jobjectArray larray::CreateObjectParamsArray(int size)
{
	return createObjectArray(size, "java/lang/Object");
}

void larray::AddObjectParamsArray(jobjectArray ary, jobject obj, int index){

	JNIEnv* jenv = jvmrun::getEnv();
	jenv->SetObjectArrayElement(ary, index, obj);
}