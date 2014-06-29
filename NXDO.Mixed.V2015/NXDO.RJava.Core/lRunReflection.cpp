#include "stdafx.h"
#include "lRunReflection.h"
#include "jvmrun.h"
#include "lconvert.h"

jclass lRunReflection::jrClass;
jclass lRunReflection::aryElemClass;
JNIEnv* lRunReflection::jenv;

jmethodID lRunReflection::jm_GetGenericArguments;
jmethodID lRunReflection::jm_GetSuperClass;
jmethodID lRunReflection::jm_GetDeclaringClass;
jmethodID lRunReflection::jm_GetElementClass;
jmethodID lRunReflection::jm_GetInterfaces;

jmethodID lRunReflection::jm_GetIsAssignableFrom;
jmethodID lRunReflection::jm_GetIsInstance;
jmethodID lRunReflection::jm_GetAsSubClass;
jmethodID lRunReflection::jm_GetAsCast;
jmethodID lRunReflection::jm_GetClassName;
jmethodID lRunReflection::jm_GetClassFlag;

jmethodID lRunReflection::jm_GetMethodDeclaringClass;
jmethodID lRunReflection::jm_GetMethodName;
jmethodID lRunReflection::jm_GetAccessModifier;

jmethodID lRunReflection::jm_GetMethod;
jmethodID lRunReflection::jm_GetMethods;

jmethodID lRunReflection::jm_GetCtor;
jmethodID lRunReflection::jm_GetCtors;
jmethodID lRunReflection::jm_GetCtorName;
jmethodID lRunReflection::jm_InvokeCtor;
jmethodID lRunReflection::jm_GetCtorDeclaringClass;
jmethodID lRunReflection::jm_GetCtorModifier;	

jmethodID lRunReflection::jm_InvokeMethod;
jmethodID lRunReflection::jm_CheckArray;


jmethodID lRunReflection::jm_GetField;
jmethodID lRunReflection::jm_GetFields;
jmethodID lRunReflection::jm_GetFieldName;
jmethodID lRunReflection::jm_GetFieldModifier;
jmethodID lRunReflection::jm_GetFieldDeclaringClass;
jmethodID lRunReflection::jm_GetFieldValue;
jmethodID lRunReflection::jm_SetFieldValue;

jmethodID lRunReflection::jm_GetMethodParams;

void lRunReflection::init(jclass jc){
	jenv = jvmrun::getEnv();
	aryElemClass = jenv->FindClass("java/lang/Class");
	jrClass = jc;

	jm_GetGenericArguments = jenv->GetStaticMethodID(jrClass, "getGenericArguments", "(Ljava/lang/Class;)[Ljxdo/rjava/JParamInfo;");
	jm_GetSuperClass = jenv->GetStaticMethodID(jrClass, "getSuperClass", "(Ljava/lang/Class;)Ljava/lang/Class;");
	jm_GetDeclaringClass = jenv->GetStaticMethodID(jrClass, "getDeclaringClass", "(Ljava/lang/Class;)Ljava/lang/Class;");
	jm_GetElementClass = jenv->GetStaticMethodID(jrClass, "getElementClass", "(Ljava/lang/Class;)Ljava/lang/Class;");
	jm_GetInterfaces = jenv->GetStaticMethodID(jrClass, "getInterfaces", "(Ljava/lang/Class;)[Ljava/lang/Class;");

	jm_GetIsAssignableFrom = jenv->GetStaticMethodID(jrClass, "getIsAssignableFrom", "(Ljava/lang/Class;Ljava/lang/Class;)Z");
	jm_GetIsInstance = jenv->GetStaticMethodID(jrClass, "getIsInstance", "(Ljava/lang/Class;Ljava/lang/Object;)Z");
	jm_GetAsSubClass = jenv->GetStaticMethodID(jrClass, "getAsSubClass", "(Ljava/lang/Class;Ljava/lang/Class;)Ljava/lang/Class;");
	jm_GetAsCast = jenv->GetStaticMethodID(jrClass, "getAsCast", "(Ljava/lang/Class;Ljava/lang/Object;)Ljava/lang/Object;");
	jm_GetClassName = jenv->GetStaticMethodID(jrClass, "getClassName", "(Ljava/lang/Class;)Ljava/lang/String;");
	jm_GetClassFlag = jenv->GetStaticMethodID(jrClass, "getClassFlag", "(Ljava/lang/Class;)[Z");	

	jm_GetMethodDeclaringClass = jenv->GetStaticMethodID(jrClass, "getDeclaringClass", "(Ljava/lang/reflect/Method;)Ljava/lang/Class;");
	jm_GetMethodName = jenv->GetStaticMethodID(jrClass, "getMethodName", "(Ljava/lang/reflect/Method;)Ljava/lang/String;");
	jm_GetAccessModifier = jenv->GetStaticMethodID(jrClass, "getAccessModifier", "(Ljava/lang/reflect/Method;)[Z");		

	jm_GetCtor = jenv->GetStaticMethodID(jrClass, "getConstructor", "(Ljava/lang/Class;[Ljava/lang/Class;)Ljava/lang/reflect/Constructor;");
	jm_GetCtors = jenv->GetStaticMethodID(jrClass, "getConstructors", "(Ljava/lang/Class;)[Ljava/lang/reflect/Constructor;");
	jm_GetCtorName = jenv->GetStaticMethodID(jrClass, "getConstructorName", "(Ljava/lang/reflect/Constructor;)Ljava/lang/String;");
	jm_InvokeCtor = jenv->GetStaticMethodID(jrClass, "invokeConstructor", "(Ljava/lang/reflect/Constructor;[Ljava/lang/Object;)Ljava/lang/Object;");
	jm_GetCtorDeclaringClass = jenv->GetStaticMethodID(jrClass, "getDeclaringClass", "(Ljava/lang/reflect/Constructor;)Ljava/lang/Class;");
	jm_GetCtorModifier = jenv->GetStaticMethodID(jrClass, "getCtorModifier", "(Ljava/lang/reflect/Constructor;)[Z");

	jm_GetMethod = jenv->GetStaticMethodID(jrClass, "getMethod", "(Ljava/lang/Class;Ljava/lang/String;[Ljava/lang/Class;)Ljava/lang/reflect/Method;");
	jm_GetMethods = jenv->GetStaticMethodID(jrClass, "getMethods", "(Ljava/lang/Class;)[Ljava/lang/reflect/Method;");

	jm_InvokeMethod = jenv->GetStaticMethodID(jrClass, "invokeMethod", "(Ljava/lang/reflect/Method;Ljava/lang/Object;[Ljava/lang/Object;)Ljava/lang/Object;");
	jm_CheckArray= jenv->GetStaticMethodID(jrClass, "checkReturnIsArray", "(Ljava/lang/Object;)Z");


	jm_GetField = jenv->GetStaticMethodID(jrClass, "getField", "(Ljava/lang/Class;Ljava/lang/String;)Ljava/lang/reflect/Field;");
	jm_GetFields = jenv->GetStaticMethodID(jrClass, "getFields", "(Ljava/lang/Class;)[Ljava/lang/reflect/Field;");
	jm_GetFieldName = jenv->GetStaticMethodID(jrClass, "getFieldName", "(Ljava/lang/reflect/Field;)Ljava/lang/String;");
	jm_GetFieldModifier = jenv->GetStaticMethodID(jrClass, "getFieldModifier", "(Ljava/lang/reflect/Field;)[Z");		
	jm_GetFieldDeclaringClass = jenv->GetStaticMethodID(jrClass, "getDeclaringClass", "(Ljava/lang/reflect/Field;)Ljava/lang/Class;");

	jm_GetFieldValue = jenv->GetStaticMethodID(jrClass, "getFieldValue", "(Ljava/lang/reflect/Field;Ljava/lang/Object;)Ljava/lang/Object;");
	jm_SetFieldValue = jenv->GetStaticMethodID(jrClass, "setFieldValue", "(Ljava/lang/reflect/Field;Ljava/lang/Object;Ljava/lang/Object;)V");
	
	jm_GetMethodParams = jenv->GetStaticMethodID(jrClass, "getMethodParams", "(Ljava/lang/reflect/Method;)[Ljxdo/rjava/JParamInfo;");
}

array<IntPtr>^ lRunReflection::GetGenericArguments(IntPtr classPtr){
	jobject jObj = (jobject)classPtr.ToPointer();
	jobject jr = jenv->CallStaticObjectMethod(jrClass, jm_GetGenericArguments, jObj);
	if(jr == NULL)
		return gcnew array<IntPtr,1>(0);

	jobjectArray ary = *reinterpret_cast<jobjectArray*>(&jr);
	jsize size = jenv->GetArrayLength(ary);
	array<IntPtr,1>^ ndatas = gcnew array<IntPtr,1>(size);
	for (jsize i = 0; i < size; i++) {
		jobject data = jvmrun::getEnv()->GetObjectArrayElement(ary, i);
		ndatas[i] = IntPtr(data);
	}
	return ndatas;
}

IntPtr lRunReflection::GetSuperClass(IntPtr classPtr){
	jclass jc = (jclass)classPtr.ToPointer();
	jclass superClass = (jclass)jenv->CallStaticObjectMethod(jrClass,jm_GetSuperClass, jc);
	return IntPtr(superClass);
}

IntPtr lRunReflection::GetDeclaringClass(IntPtr classPtr){
	jclass jc = (jclass)classPtr.ToPointer();
	jclass declaringClass = (jclass)jenv->CallStaticObjectMethod(jrClass,jm_GetDeclaringClass, jc);
	return IntPtr(declaringClass);
}

IntPtr lRunReflection::GetElementClass(IntPtr classPtr)
{
	jclass jc = (jclass)classPtr.ToPointer();
	jclass elemClass = (jclass)jenv->CallStaticObjectMethod(jrClass,jm_GetElementClass, jc);
	return IntPtr(elemClass);
}

array<IntPtr>^ lRunReflection::GetInterfaces(IntPtr classPtr){
	jclass jc = (jclass)classPtr.ToPointer();
	jobject jr = jenv->CallStaticObjectMethod(jrClass, jm_GetInterfaces, jc);

	jobjectArray ary = *reinterpret_cast<jobjectArray*>(&jr);
	jsize size = jenv->GetArrayLength(ary);
	array<IntPtr,1>^ ndatas = gcnew array<IntPtr,1>(size);
	for (jsize i = 0; i < size; i++) {
		jobject data = jvmrun::getEnv()->GetObjectArrayElement(ary, i);
		ndatas[i] = IntPtr(data);
	}
	return ndatas;
}

bool lRunReflection::GetIsAssignableFrom(IntPtr classPtr, IntPtr classPtr2){
	jclass jc = (jclass)classPtr.ToPointer();
	jclass jc2 = (jclass)classPtr2.ToPointer();
	jboolean jb = jenv->CallStaticBooleanMethod(jrClass,jm_GetIsAssignableFrom, jc, jc2);
	return jb == JNI_TRUE ? true : false;
}

bool lRunReflection::GetIsInstance(IntPtr classPtr, IntPtr objectPtr){
	jclass jc = (jclass)classPtr.ToPointer();
	jobject jo = (jobject)objectPtr.ToPointer();
	jboolean jb = jenv->CallStaticBooleanMethod(jrClass,jm_GetIsInstance, jc, jo);
	return jb == JNI_TRUE ? true : false;
}

IntPtr lRunReflection::GetAsSubClass(IntPtr classPtr, IntPtr parentClassPtr){
	jclass jc = (jclass)classPtr.ToPointer();
	jclass jcParent = (jclass)parentClassPtr.ToPointer();
	jobject jr = jenv->CallStaticObjectMethod(jrClass,jm_GetAsSubClass, jc, jcParent);
	return IntPtr(jr);
}
			
IntPtr lRunReflection::GetAsCast(IntPtr classPtr, IntPtr objectPtr){
	jclass jc = (jclass)classPtr.ToPointer();
	jobject jo = (jobject)objectPtr.ToPointer();
	jobject jr = jenv->CallStaticObjectMethod(jrClass,jm_GetAsCast, jc, jo);
	return IntPtr(jr);
}

String^ lRunReflection::GetClassName(IntPtr classPtr){
	jclass jc = (jclass)classPtr.ToPointer();
	jstring sClsName = (jstring)jenv->CallStaticObjectMethod(jrClass,jm_GetClassName, jc);
	return lconvert::toNString(sClsName);
}

array<Boolean>^ lRunReflection::GetClassFlag(IntPtr classPtr){
	jclass jc = (jclass)classPtr.ToPointer();
	jobject ary = jenv->CallStaticObjectMethod(jrClass,jm_GetClassFlag,jc);
	jbooleanArray* arr = reinterpret_cast<jbooleanArray*>(&ary);

	int size = jenv->GetArrayLength(*arr);
	array<System::Boolean,1>^ ndatas = gcnew array<System::Boolean,1>(size);
	jboolean * data = jenv->GetBooleanArrayElements(*arr, NULL);
	for (int i = 0; i < size; i++) {
		ndatas[i] = data[i] == 0 ? false : true;
	}
	jenv->ReleaseBooleanArrayElements(*arr, data, JNI_FALSE); 
	jenv->DeleteLocalRef(ary);
	return ndatas;
}

IntPtr lRunReflection::GetMethodDeclaringClass(IntPtr methodPtr){
	jmethodID jm = (jmethodID)methodPtr.ToPointer();
	jclass declaringClass = (jclass)jenv->CallStaticObjectMethod(jrClass,jm_GetMethodDeclaringClass, jm);
	return IntPtr(declaringClass);
}

String^ lRunReflection::GetMethodName(IntPtr methodPtr){
	jmethodID jm = (jmethodID)methodPtr.ToPointer();
	jstring jstr = (jstring)jenv->CallStaticObjectMethod(jrClass,jm_GetMethodName,jm);
	return lconvert::toNString(jstr);
}

array<Boolean>^ lRunReflection::GetAccessModifier(IntPtr methodPtr){
	jmethodID jm = (jmethodID)methodPtr.ToPointer();
	jobject ary = jenv->CallStaticObjectMethod(jrClass,jm_GetAccessModifier,jm);
	jbooleanArray* arr = reinterpret_cast<jbooleanArray*>(&ary);

	int size = jenv->GetArrayLength(*arr);
	array<System::Boolean,1>^ ndatas = gcnew array<System::Boolean,1>(size);
	jboolean * data = jenv->GetBooleanArrayElements(*arr, NULL);
	for (int i = 0; i < size; i++) {
		ndatas[i] = data[i] == 0 ? false : true;
	}
	jenv->ReleaseBooleanArrayElements(*arr, data, JNI_FALSE); 
	jenv->DeleteLocalRef(ary);
	return ndatas;
}

IntPtr lRunReflection::GetConstructor(IntPtr classPtr, array<IntPtr>^ paramTypes){
	jclass jclazzOfCtor = (jclass)classPtr.ToPointer();

	jint size = paramTypes->Length;
	jobjectArray jTypeAry = jenv->NewObjectArray(size, aryElemClass, 0);	

	for(long long i=0; i<size; i++){
		IntPtr ptr = (IntPtr)paramTypes->GetValue(i);
		jenv->SetObjectArrayElement(jTypeAry, i, (jclass)ptr.ToPointer());
	}

	jobject jr = jenv->CallStaticObjectMethod(jrClass,jm_GetCtor,jclazzOfCtor, jTypeAry);
	return IntPtr(jr);
}

array<IntPtr>^ lRunReflection::GetConstructors(IntPtr classPtr){
	jclass jclazzOfCtor = (jclass)classPtr.ToPointer();
	jobject jr = jenv->CallStaticObjectMethod(jrClass,jm_GetCtors,jclazzOfCtor);
	jobjectArray ary = *reinterpret_cast<jobjectArray*>(&jr);

	jsize size = jenv->GetArrayLength(ary);
	array<IntPtr,1>^ ndatas = gcnew array<IntPtr,1>(size);
	for (jsize i = 0; i < size; i++) {
		jobject data = jvmrun::getEnv()->GetObjectArrayElement(ary, i);
		ndatas[i] = IntPtr(data);
	}
	return ndatas;
}

String^ lRunReflection::GetConstructorName(IntPtr ctorPtr){
	jobject jCtor = (jobject)ctorPtr.ToPointer();
	jstring jr = (jstring)jenv->CallStaticObjectMethod(jrClass,jm_GetCtorName,jCtor);
	return lconvert::toNString(jr);
}

array<Boolean>^ lRunReflection::GetCtorModifier(IntPtr ctorPtr){
	jobject jCtor = (jobject)ctorPtr.ToPointer();
	jobject ary = jenv->CallStaticObjectMethod(jrClass,jm_GetCtorModifier,jCtor);
	jbooleanArray* arr = reinterpret_cast<jbooleanArray*>(&ary);

	int size = jenv->GetArrayLength(*arr);
	array<System::Boolean,1>^ ndatas = gcnew array<System::Boolean,1>(size);
	jboolean * data = jenv->GetBooleanArrayElements(*arr, NULL);
	for (int i = 0; i < size; i++) {
		ndatas[i] = data[i] == 0 ? false : true;
	}
	jenv->ReleaseBooleanArrayElements(*arr, data, JNI_FALSE); 
	jenv->DeleteLocalRef(ary);
	return ndatas;
}

jobject lRunReflection::InvokeConstructor(IntPtr ctorPtr, jobjectArray paramValues){
	jobject jCtor = (jobject)ctorPtr.ToPointer();
	return jenv->CallStaticObjectMethod(jrClass, jm_InvokeCtor,  jCtor, paramValues);
}

IntPtr lRunReflection::GetConstructorDeclaringClass(IntPtr ctorPtr){
	jobject jCtor = (jobject)ctorPtr.ToPointer();
	jobject jr = jenv->CallStaticObjectMethod(jrClass, jm_GetCtorDeclaringClass,  jCtor);
	return IntPtr(jr);
}

jobject lRunReflection::GetMethod(IntPtr classPtr, String^ methodName, array<IntPtr>^ paramTypes){
	jclass jclazzOfMethod = (jclass)classPtr.ToPointer();

	jint size = paramTypes->Length;
	jobjectArray jTypeAry = jenv->NewObjectArray(size, aryElemClass, 0);	

	for(long long i=0; i<size; i++){
		IntPtr ptr = (IntPtr)paramTypes->GetValue(i);
		jenv->SetObjectArrayElement(jTypeAry, i, (jclass)ptr.ToPointer());
	}

	jobject jm = jenv->CallStaticObjectMethod(jrClass, jm_GetMethod, jclazzOfMethod, lconvert::toJString(methodName), jTypeAry);
	return jm;
}

array<IntPtr>^ lRunReflection::GetMethods(IntPtr classPtr){
	jclass jclazzOfMethod = (jclass)classPtr.ToPointer();
	jobject jr = jenv->CallStaticObjectMethod(jrClass,jm_GetMethods,jclazzOfMethod);
	jobjectArray ary = *reinterpret_cast<jobjectArray*>(&jr);

	jsize size = jenv->GetArrayLength(ary);
	array<IntPtr,1>^ ndatas = gcnew array<IntPtr,1>(size);
	for (jsize i = 0; i < size; i++) {
		jobject data = jvmrun::getEnv()->GetObjectArrayElement(ary, i);
		ndatas[i] = IntPtr(data);
	}
	return ndatas;
}

jobject lRunReflection::InvokeMethod(IntPtr methodPtr, IntPtr jObjectPtr, jobjectArray paramValues)
{
	jobject jo = (jobject)jObjectPtr.ToPointer();
	jmethodID jm = (jmethodID)methodPtr.ToPointer();
	return jenv->CallStaticObjectMethod(jrClass, jm_InvokeMethod,  jm, jo, paramValues);
}

jboolean lRunReflection::CheckIsArray(IntPtr objectPtr){
	jobject jo = (jobject)objectPtr.ToPointer();
	return jenv->CallStaticBooleanMethod(jrClass, jm_CheckArray, jo);
}

IntPtr lRunReflection::GetField(IntPtr classPtr, String^ fieldName){
	jclass jclazzOfField = (jclass)classPtr.ToPointer();
	jobject jr = jenv->CallStaticObjectMethod(jrClass, jm_GetField, jclazzOfField, lconvert::toJString(fieldName));
	return IntPtr(jr);
}

array<IntPtr>^ lRunReflection::GetFields(IntPtr classPtr){
	jclass jclazzOfField = (jclass)classPtr.ToPointer();
	jobject jr = jenv->CallStaticObjectMethod(jrClass,jm_GetFields,jclazzOfField);
	jobjectArray ary = *reinterpret_cast<jobjectArray*>(&jr);

	jsize size = jenv->GetArrayLength(ary);
	array<IntPtr,1>^ ndatas = gcnew array<IntPtr,1>(size);
	for (jsize i = 0; i < size; i++) {
		jobject data = jvmrun::getEnv()->GetObjectArrayElement(ary, i);
		ndatas[i] = IntPtr(data);
	}
	return ndatas;
}

String^ lRunReflection::GetFieldName(IntPtr fldPtr){
	jobject jFld = (jobject)fldPtr.ToPointer();
	jstring jr = (jstring)jenv->CallStaticObjectMethod(jrClass,jm_GetFieldName,jFld);
	return lconvert::toNString(jr);
}

array<Boolean>^ lRunReflection::GetFieldModifier(IntPtr fldPtr){
	jobject jFld = (jobject)fldPtr.ToPointer();
	jobject ary = jenv->CallStaticObjectMethod(jrClass,jm_GetFieldModifier,jFld);
	jbooleanArray* arr = reinterpret_cast<jbooleanArray*>(&ary);

	int size = jenv->GetArrayLength(*arr);
	array<System::Boolean,1>^ ndatas = gcnew array<System::Boolean,1>(size);
	jboolean * data = jenv->GetBooleanArrayElements(*arr, NULL);
	for (int i = 0; i < size; i++) {
		ndatas[i] = data[i] == 0 ? false : true;
	}
	jenv->ReleaseBooleanArrayElements(*arr, data, JNI_FALSE); 
	jenv->DeleteLocalRef(ary);
	return ndatas;
}

IntPtr lRunReflection::GetFieldDeclaringClass(IntPtr fldPtr){
	jobject jFld = (jobject)fldPtr.ToPointer();
	jobject jr = jenv->CallStaticObjectMethod(jrClass, jm_GetFieldDeclaringClass,  jFld);
	return IntPtr(jr);
}

IntPtr lRunReflection::GetFieldValue(IntPtr fldPtr, IntPtr objPtr){
	jobject jFld = (jobject)fldPtr.ToPointer();
	jobject jOwner = (jobject)objPtr.ToPointer();
	jobject jr = jenv->CallStaticObjectMethod(jrClass, jm_GetFieldValue,  jFld, jOwner);
	return IntPtr(jr);
}

void lRunReflection::SetFieldValue(IntPtr fldPtr, IntPtr objPtr, jobject value){
	jobject jFld = (jobject)fldPtr.ToPointer();
	jobject jOwner = (jobject)objPtr.ToPointer();
	jenv->CallStaticObjectMethod(jrClass, jm_SetFieldValue,  jFld, jOwner, value);
}


array<IntPtr>^ lRunReflection::GetMethodParams(IntPtr methodPtr){
	jobject jMethodObj = (jobject)methodPtr.ToPointer();
	jobject jr = jenv->CallStaticObjectMethod(jrClass, jm_GetMethodParams, jMethodObj);

	jobjectArray ary = *reinterpret_cast<jobjectArray*>(&jr);
	jsize size = jenv->GetArrayLength(ary);
	array<IntPtr,1>^ ndatas = gcnew array<IntPtr,1>(size);
	for (jsize i = 0; i < size; i++) {
		jobject data = jvmrun::getEnv()->GetObjectArrayElement(ary, i);
		ndatas[i] = IntPtr(data);
	}
	return ndatas;
}
